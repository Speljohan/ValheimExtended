using HarmonyLib;
using UnityEngine;


namespace ValheimExtended
{

    /// <summary>
    /// Forces the chat window to become visible when a new message is received.
    /// </summary>
    [HarmonyPatch(typeof(Chat), "OnNewChatMessage")]
    public static class ChatFixer
    {
        static void Prefix(ref Chat __instance, ref GameObject go, ref long senderID, ref Vector3 pos, ref Talker.Type type, ref string user, ref string text)
        {
            __instance.m_hideTimer = 0f;
        }
    }
}
