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
        public Dictionary<string, float> m_floats = new Dictionary<string, float>();
        public Dictionary<string, int> m_ints = new Dictionary<string, int>();
        public Dictionary<string, long> m_longs = new Dictionary<string, long>();
        public Dictionary<string, string> m_strings = new Dictionary<string, string>();


        public Boolean contains(string key)
        {
            return !m_floats.ContainsKey(key) && !m_ints.ContainsKey(key) && !m_longs.ContainsKey(key) && !m_strings.ContainsKey(key);
        }

        public void Set(string key, float value)
        {
            m_floats[key] = value;
        }

        public void Set(string key, int value)
        {
            m_ints[key] = value;
        }

        public void Set(string key, long value)
        {
            m_longs[key] = value;
        }

        public void Set(string key, string value)
        {
            m_strings[key] = value;
        }

        public float GetFloat(string key)
        {
            return m_floats[key];
        }

        public int GetInt(string key)
        {
            return m_ints[key];
        }

        public long GetLong(string key)
        {
            return m_longs[key];
        }

        public string GetString(string key)
        {
            return m_strings[key];
        }

        public void From(TyrData other)
        {
            foreach (var item in other.m_floats)
            {
                m_floats[item.Key] = item.Value;
            }

            foreach (var item in other.m_ints)
            {
                m_ints[item.Key] = item.Value;
            }

            foreach (var item in other.m_longs)
            {
                m_longs[item.Key] = item.Value;
            }

            foreach (var item in other.m_strings)
            {
                m_strings[item.Key] = item.Value;
            }
        }

        public Boolean isEmpty()
        {
            return m_floats.Count == 0 && m_ints.Count == 0 && m_longs.Count == 0 && m_strings.Count == 0;
        }

        public void Save(ZPackage pkg)
        {
            pkg.Write(m_floats.Count);
            foreach (var item in m_floats)
            {
                pkg.Write(item.Key);
                pkg.Write(item.Value);
            }

            pkg.Write(m_ints.Count);
            foreach (var item in m_ints)
            {
                pkg.Write(item.Key);
                pkg.Write(item.Value);
            }

            pkg.Write(m_longs.Count);
            foreach (var item in m_longs)
            {
                pkg.Write(item.Key);
                pkg.Write(item.Value);
            }

            pkg.Write(m_strings.Count);
            foreach (var item in m_strings)
            {
                pkg.Write(item.Key);
                pkg.Write(item.Value);
            }
        }

        public void Load(ZPackage pkg)
        {
            int floatCount = pkg.ReadInt();
            for (int i = 0; i < floatCount; i++)
            {
                var key = pkg.ReadString();
                var value = pkg.ReadSingle();
                m_floats[key] = value;
            }

            int intCount = pkg.ReadInt();
            for (int i = 0; i < intCount; i++)
            {
                var key = pkg.ReadString();
                var value = pkg.ReadInt();
                m_ints[key] = value;
            }

            int longCount = pkg.ReadInt();
            for (int i = 0; i < longCount; i++)
            {
                var key = pkg.ReadString();
                var value = pkg.ReadLong();
                m_longs[key] = value;
            }

            int stringCount = pkg.ReadInt();
            for (int i = 0; i < stringCount; i++)
            {
                var key = pkg.ReadString();
                var value = pkg.ReadString();
                m_strings[key] = value;
            }
        }

        public String GetTyrTooltipText()
        {
            if (isEmpty())
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("\n");
            sb.Append("\n");
            sb.Append("TyrData: ");
            sb.Append("\n");

            foreach (var item in m_floats)
            {
                sb.AppendFormat("\n{0}: <color=orange>{1}</color>", item.Key, item.Value);
            }
            foreach (var item in m_ints)
            {
                sb.AppendFormat("\n{0}: <color=orange>{1}</color>", item.Key, item.Value);
            }
            foreach (var item in m_longs)
            {
                sb.AppendFormat("\n{0}: <color=orange>{1}</color>", item.Key, item.Value);
            }
            foreach (var item in m_strings)
            {
                sb.AppendFormat("\n{0}: <color=orange>{1}</color>", item.Key, item.Value);
            }
            return sb.ToString();
        }

    }

}
