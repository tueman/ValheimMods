using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace WorldGenOptions
{
    public class GenOptionsUI : BaseUnityPlugin
    {
        public static GenOptionsUI instance;

        private Rect windowRect, lastRect;

        public string worldName;
        
        private void Awake()
        {

            windowRect.x = 1230;
            windowRect.y = 170;
            windowRect.width = 500;
            windowRect.height = 720;
 
        }



        private void OnGUI()
        {
            windowRect = GUILayout.Window(1586463, windowRect, DrawConnectWindow, "World Generation Options");
            if (!lastRect.Equals(windowRect))
            {
                float variablex = windowRect.x;
                float variabley = windowRect.y;
                lastRect = windowRect;
            }
        }


        void DrawConnectWindow(int windowID)
        {
            {
                GUILayout.Label(worldName);
                GUILayout.BeginHorizontal();
                GUILayout.Label("River Multiple Max Distance");
                GUILayout.TextField(GenOptions.savedData.riverMultipleMaxDistance.ToString());
                GenOptions.savedData.riverMultipleMaxDistance = GUILayout.HorizontalSlider(GenOptions.savedData.riverMultipleMaxDistance, 0f, 30000f);
                GUILayout.EndHorizontal();
            }
        }

        
       
    }
}


/* 
        private static void ApplyConfig()
        {
            windowRect = new Rect(100, 100, 500, 500);
            backStyle = new GUIStyle();

        }


private void WindowBuilder(int id)
        {
            GUILayout.BeginVertical();
            GUI.DragWindow(new Rect(0, 0, 200, 200));

            GUILayout.BeginHorizontal(backStyle, new GUILayoutOption[] { GUILayout.Height(30), GUILayout.Width(50) });

            if (!GUILayout.Button("Test", new GUILayoutOption[]{
                    GUILayout.Width(15),
                    GUILayout.Height(15)
                }))
                return;

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }



 [HarmonyPatch(typeof(Console), "InputText")]
        static class InputText_Patch
        {
            static bool Prefix(Console __instance)
            {
                string text = __instance.m_input.text;
                if (text.ToLower().Equals("nexusupdate reset"))
                {
                    context.Config.Reload();
                    context.Config.Save();
                    ApplyConfig();

                    Traverse.Create(__instance).Method("AddString", new object[] { text }).GetValue();
                    Traverse.Create(__instance).Method("AddString", new object[] { "Nexus Update Check config reloaded" }).GetValue();
                    return false;
                }
                return true;
            }
        }







*/
