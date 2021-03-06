using System.Linq;
using HarmonyLib;
using UnityEngine;

/// <summary>
/// Adds support for custom properties on Items.
/// </summary>
namespace ValheimExtended.LibTyr
{

    [HarmonyPatch(typeof(ItemDrop))]
    public static class ItemDropPatcher
    {

        /// <summary>
        /// Test code
        /// TODO: To be replaced 
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch("Start")]
        static void Start(ref ItemDrop __instance)
        {
            if (__instance.m_nview && __instance.m_nview.IsValid())
            {
                if (__instance.m_itemData is TyrData)
                {
                    var tyrData = (TyrData)__instance.m_itemData;

                    if (tyrData.m_shared.m_name == "$item_mushroomyellow")
                    {
                        tyrData.Set("dye_color", 15190091);
                        /*var renderer = __instance.gameObject.GetComponentInChildren<MeshRenderer>();
                        renderer.material.SetColor("_EmissionColor", Utils.ToColor(tyrData.GetInt("dye_color")));*/
                    }
                }
            }

        }

        [HarmonyPrefix]
        [HarmonyPatch("DropItem")]
        static bool DropItem(ref ItemDrop __result, ref ItemDrop.ItemData item, ref int amount, ref Vector3 position, ref Quaternion rotation)
        {
            ItemDrop component = Object.Instantiate(item.m_dropPrefab, position, rotation).GetComponent<ItemDrop>();
            component.m_itemData = item.Clone();
            if (item is TyrData)
            {
                var otherData = (TyrData)item;
                var thisData = (TyrData)component.m_itemData;
                thisData.From(otherData);

                component.m_itemData = thisData;
            }
            if (amount > 0)
            {
                component.m_itemData.m_stack = amount;
            }
            component.Save();
            __result = component;
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch("Save")]
        static void SaveToZDO(ref ItemDrop __instance)
        {
            if (__instance.m_nview == null || !__instance.m_nview.IsValid())
            {
                return;
            }

            if (__instance.m_nview.IsOwner())
            {
                if (__instance.m_itemData is TyrData)
                {
                    var tyrData = (TyrData)__instance.m_itemData;

                    if (tyrData.isEmpty())
                    {
                        return;
                    }

                        foreach (var item in tyrData.m_floats)
                    {
                        __instance.m_nview.m_zdo.Set("libtyr_" + item.Key, item.Value);
                    }
                    foreach (var item in tyrData.m_ints)
                    {
                        __instance.m_nview.m_zdo.Set("libtyr_" + item.Key, item.Value);
                    }
                    foreach (var item in tyrData.m_longs)
                    {
                        __instance.m_nview.m_zdo.Set("libtyr_" + item.Key, item.Value);
                    }
                    foreach (var item in tyrData.m_strings)
                    {
                        __instance.m_nview.m_zdo.Set("libtyr_" + item.Key, item.Value);
                    }
                    Debug.Log("[TyrData] Data written to ZDO");
                }
            }

        }

        [HarmonyPostfix]
        [HarmonyPatch("Load")]
        static void LoadFromZDO(ref ItemDrop __instance)
        {
            if (__instance.m_itemData is TyrData)
            {
                var tyrData = (TyrData)__instance.m_itemData;

                foreach (var item in tyrData.m_floats.Keys.ToList())
                {
                    tyrData.m_floats[item] = __instance.m_nview.m_zdo.GetFloat("libtyr_" + item);
                }
                foreach (var item in tyrData.m_ints.Keys.ToList())
                {
                    tyrData.m_ints[item] = __instance.m_nview.m_zdo.GetInt("libtyr_" + item);
                }
                foreach (var item in tyrData.m_longs.Keys.ToList())
                {
                    tyrData.m_longs[item] = __instance.m_nview.m_zdo.GetLong("libtyr_" + item);
                }
                foreach (var item in tyrData.m_strings.Keys.ToList())
                {
                    tyrData.m_strings[item] = __instance.m_nview.m_zdo.GetString("libtyr_" + item);
                }
                Debug.Log("[TyrData] Data retrieved from ZDO");
            }

        }

        [HarmonyPostfix]
        [HarmonyPatch("GetHoverText")]
        static void GetHoverText(ref ItemDrop __instance, ref string __result)
        {
            if (__instance.m_itemData is TyrData)
            {
                var tyrData = (TyrData)__instance.m_itemData;

                __result = __result + tyrData.GetTyrTooltipText();
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(MethodType.Constructor)]
        static void ItemDrop_Constructor(ref ItemDrop __instance)
        {
            __instance.m_itemData = new TyrData();

        }


    }

    [HarmonyPatch(typeof(ItemDrop.ItemData))]
    public static class ItemDataPatcher
    {

        [HarmonyPostfix]
        [HarmonyPatch("GetTooltip")]
        [HarmonyPatch(new System.Type[] { typeof(ItemDrop.ItemData), typeof(int), typeof(bool) })]
        static void GetTooltip(ref ItemDrop.ItemData item, ref int qualityLevel, ref bool crafting, ref string __result)
        {
            if (item is TyrData)
            {
                var tyrData = (TyrData)item;

                __result = __result + tyrData.GetTyrTooltipText();
            }

        }

    }

    [HarmonyPatch(typeof(Inventory))]
    public static class InventoryPatcher
    {

        [HarmonyPrefix]
        [HarmonyPatch("Save")]
        static bool Save(ref Inventory __instance, ref ZPackage pkg)
        {
            pkg.Write(10000);
            pkg.Write(__instance.m_inventory.Count);
            foreach (ItemDrop.ItemData item in __instance.m_inventory)
            {
                if (item.m_dropPrefab == null)
                {
                    ZLog.Log("Item missing prefab " + item.m_shared.m_name);
                    pkg.Write("");
                }
                else
                {
                    pkg.Write(item.m_dropPrefab.name);
                }
                pkg.Write(item.m_stack);
                pkg.Write(item.m_durability);
                pkg.Write(item.m_gridPos);
                pkg.Write(item.m_equiped);
                pkg.Write(item.m_quality);
                pkg.Write(item.m_variant);
                pkg.Write(item.m_crafterID);
                pkg.Write(item.m_crafterName);

                if (item is TyrData)
                {
                    var tyrData = (TyrData)item;
                    tyrData.Save(pkg);
                }
            }
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("Load")]
        static bool Load(ref Inventory __instance, ref ZPackage pkg)
        {
            int num = pkg.ReadInt();
            int num2 = pkg.ReadInt();
            __instance.m_inventory.Clear();
            for (int i = 0; i < num2; i++)
            {
                string text = pkg.ReadString();
                int stack = pkg.ReadInt();
                float durability = pkg.ReadSingle();
                Vector2i pos = pkg.ReadVector2i();
                bool equiped = pkg.ReadBool();
                int quality = 1;
                if (num >= 101)
                {
                    quality = pkg.ReadInt();
                }
                int variant = 0;
                if (num >= 102)
                {
                    variant = pkg.ReadInt();
                }
                long crafterID = 0L;
                string crafterName = "";
                if (num >= 103)
                {
                    crafterID = pkg.ReadLong();
                    crafterName = pkg.ReadString();
                }
                
                if (text != "")
                {
                    GameObject itemPrefab = ObjectDB.instance.GetItemPrefab(text);
                    if (itemPrefab == null)
                    {
                        ZLog.Log("Failed to find item prefab " + text);
                        continue;
                    }
                    ZNetView.m_forceDisableInit = true;
                    GameObject gameObject = UnityEngine.Object.Instantiate(itemPrefab);
                    ZNetView.m_forceDisableInit = false;
                    ItemDrop component = gameObject.GetComponent<ItemDrop>();
                    if (component == null)
                    {
                        ZLog.Log("Missing itemdrop in " + text);
                        UnityEngine.Object.Destroy(gameObject);
                        continue;
                    }
                    component.m_itemData.m_stack = Mathf.Min(stack, component.m_itemData.m_shared.m_maxStackSize);
                    component.m_itemData.m_durability = durability;
                    component.m_itemData.m_equiped = equiped;
                    component.m_itemData.m_quality = quality;
                    component.m_itemData.m_variant = variant;
                    component.m_itemData.m_crafterID = crafterID;
                    component.m_itemData.m_crafterName = crafterName;
                    
                    if (num == 10000 && component.m_itemData is TyrData)
                    {
                        var tyrData = (TyrData)component.m_itemData;
                        tyrData.Load(pkg);
                    }

                    __instance.AddItem(component.m_itemData, component.m_itemData.m_stack, pos.x, pos.y);
                    UnityEngine.Object.Destroy(gameObject);
                }
            }
            __instance.Changed();
            return false;
        }
    }
}
