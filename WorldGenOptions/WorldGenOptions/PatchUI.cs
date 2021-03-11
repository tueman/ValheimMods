﻿using HarmonyLib;
using UnityEngine;

namespace WorldGenOptions
{
    [HarmonyPatch(typeof(FejdStartup), "ShowStartGame")]
    class PatchInitUI
    {
        public static GameObject genUI = new GameObject("GenOptionsUI");

        static void Postfix(ref FejdStartup __instance)
        {
            ZLog.Log(genUI);
            if (!GenOptionsUI.instance)
            {
                ZLog.Log("ShowStartGame GenOptionsUI.instance == false");
                GenOptionsUI.instance = genUI.AddComponent<GenOptionsUI>();    
            }
            ZLog.Log("ShowStartGame");
            GameObject currentSelectedGameObject = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            int index = __instance.FindSelectedWorld(currentSelectedGameObject);
            GenOptionsUI.instance.worldName = __instance.m_world.m_name;
            genUI.SetActive(true);
        }  
    }

    [HarmonyPatch(typeof(FejdStartup), "OnWorldNew")]
    class PatchUI
    {
        static void Postfix()
        {
            if (GenOptionsUI.instance)
            {
                PatchInitUI.genUI.SetActive(true);
            }
        }
    }

    [HarmonyPatch(typeof(FejdStartup), "OnNewWorldBack")]
    class PatchUICancel
    {
        static void Postfix()
        {
            if (GenOptionsUI.instance)
            {
                PatchInitUI.genUI.SetActive(false);
            }
        }
    }

    //
    [HarmonyPatch(typeof(FejdStartup), "SetSelectedWorld")]
    class PatchUIUpdate
    {
        static void Postfix(ref FejdStartup __instance, ref bool centerSelection)
        {
            if (GenOptionsUI.instance)
            {
                ZLog.Log("SetSelectedWorld GenOptionsUI.instance == true");
                if (!centerSelection)
                {
                    ZLog.Log("centerSelection == false || should activate UI");
                    //__instance.m_joinGameButton.interactable = (__instance.m_joinServer != null);
                    PatchInitUI.genUI.SetActive(true);
                    GenOptionsUI.instance.worldName = __instance.m_world.m_name;
                }
                else
                {
                    ZLog.Log("centerSelection == true || should deactivate UI");
                    PatchInitUI.genUI.SetActive(false);
                }
            }
        }
    }

    //Close UI when clicking Start button to join a world
    [HarmonyPatch(typeof(FejdStartup), "OnWorldStart")]
    class PatchUIClose
    {
        static void Postfix(ref FejdStartup __instance)
        {
            if (GenOptionsUI.instance)
            {
                PatchInitUI.genUI.SetActive(false);
                GenOptionsUI.Destroy(PatchInitUI.genUI);
            }
        }
    }

    //Close UI when clicking back to go back to Character Selection screen
    [HarmonyPatch(typeof(FejdStartup), "ShowCharacterSelection")]
    class PatchUIBack
    {
        static void Postfix()
        {
            if (GenOptionsUI.instance)
            {
                PatchInitUI.genUI.SetActive(false);
            }
        }
    }
    /*
    //Close UI when clicking on Join Game tab **Only work once for some reason** Might be a good idea to remove this if people want to change worldgen on servers
    [HarmonyPatch(typeof(FejdStartup), "Update")]
    class PatchUIJoinGame
    {
        static void Postfix(ref FejdStartup __instance)
        {
            if (GenOptionsUI.instance)
            {
                if (__instance.m_joinGameButton.interactable == false)
                {
                    PatchInitUI.genUI.SetActive(false);
                }
                if (__instance.m_worldStart.interactable == true)
                {
                    PatchInitUI.genUI.SetActive(true);
                }
            }
        }
    } 
    */
}
