using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ValheimExtended.LibTyr
{
    [Serializable]
    class TyrData : ItemDrop.ItemData
    {
        public Dictionary<string, TyrProperty> m_properties = new Dictionary<string, TyrProperty>();

        public void Set(string key, object value, bool mutable = true)
        {
            m_properties[key] = new TyrProperty(key, value, mutable);
        }

        public TyrProperty Get(string key)
        {
            return m_properties[key];
        }

        public void Copy(TyrData other)
        {
            m_properties = other.m_properties;
        }

        public Boolean IsEmpty()
        {
            return m_properties.Count == 0;
        }

        public void Save(ZPackage pkg)
        {
            var mutable = m_properties.Where((i) => i.Value.m_mutable);

            pkg.Write(mutable.Count());
            foreach (var item in mutable)
            {
                pkg.Write(item.Key);

                var val = item.Value.m_value;
                if (val is float)
                {
                    pkg.Write((int)TyrDataType.FLOAT);
                    pkg.Write(item.Value.GetValue<float>());
                } else if (val is int)
                {
                    pkg.Write((int)TyrDataType.INT);
                    pkg.Write(item.Value.GetValue<int>());
                } else if (val is long)
                {
                    pkg.Write((int)TyrDataType.LONG);
                    pkg.Write(item.Value.GetValue<long>());
                }
                else if (val is string)
                {
                    pkg.Write((int)TyrDataType.STRING);
                    pkg.Write(item.Value.GetValue<string>());
                }
            }

        }

        public void Load(ZPackage pkg)
        {
            int propertyCount = pkg.ReadInt();
            for (int i = 0; i < propertyCount; i++)
            {
                var key = pkg.ReadString();
                var dataType = (TyrDataType)Enum.ToObject(typeof(TyrDataType), pkg.ReadInt());
                if (dataType == TyrDataType.FLOAT)
                {
                    var val = pkg.ReadSingle();
                    m_properties[key] = new TyrProperty(key, val, true);
                }
                else if (dataType == TyrDataType.INT)
                {
                    var val = pkg.ReadInt();
                    m_properties[key] = new TyrProperty(key, val, true);
                }
                else if (dataType == TyrDataType.LONG)
                {
                    var val = pkg.ReadLong();
                    m_properties[key] = new TyrProperty(key, val, true);
                }
                else if (dataType == TyrDataType.STRING)
                {
                    var val = pkg.ReadString();
                    m_properties[key] = new TyrProperty(key, val, true);
                }
            }
        }

        public String GetTyrTooltipText()
        {
            if (IsEmpty())
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("\n");
            sb.Append("\n");
            sb.Append("TyrData: ");
            sb.Append("\n");

            foreach (var item in m_properties)
            {
                sb.AppendFormat("\n{0}: <color=orange>{1}</color>", item.Key, item.Value.m_value);
            }
            return sb.ToString();
        }

    }

    class TyrProperty : TyrValue
    {
        public String m_name;
        public bool m_mutable;

        public TyrProperty(string name, object value, bool mutable)
        {
            this.m_name = name;
            this.m_value = value;
            this.m_mutable = mutable;
        }
    }

    public enum TyrDataType
    {
        FLOAT = 0,
        INT = 1,
        LONG = 2,
        STRING = 3
    }
        

    class TyrValue
    {
        public object m_value;

        public T GetValue<T>()
        {
            return (T)m_value;
        }

        public void SetValue<T>(T value)
        {
            this.m_value = value;
        }

        public TyrValue WithValue<T>(T value)
        {
            this.m_value = value;
            return this;
        }
    }

}
