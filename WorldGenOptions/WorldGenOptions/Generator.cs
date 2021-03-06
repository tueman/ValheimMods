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

namespace Generator
{
    
    [HarmonyPatch(typeof(WorldGenerator))]
    [HarmonyPatch(nameof(WorldGenerator.GetBiome))]
    [HarmonyPatch(new Type[] { typeof(float), typeof(float) })]
    public static class BiomeGenPrefixPatch
    {
        public static bool Prefix(ref WorldGenerator __instance, ref Heightmap.Biome __result, float wx, float wy)
        {
            if (__instance.m_world.m_menu)
            {
                if (__instance.GetBaseHeight(wx, wy, true) >= 0.4f)
                {
                    __result = Heightmap.Biome.Mountain;
                    return false;
                }
                __result = Heightmap.Biome.BlackForest;
                return false;
            }
            else
            {
                float magnitude = new UnityEngine.Vector2(wx, wy).magnitude;
                float baseHeight = __instance.GetBaseHeight(wx, wy, false);
                float num = __instance.WorldAngle(wx, wy) * 100f;
                if (new UnityEngine.Vector2(wx, wy + WorldGenOptions.GenOptions.usingData.minAshlandsDist).magnitude > 12000f + num)
                {
                    __result = WorldGenOptions.GenOptions.usingData.ashlandsSwitch;
                    return false;
                }
                if ((double)baseHeight <= 0.02)
                {
                    __result = Heightmap.Biome.Ocean;
                    return false;
                }
                if (new UnityEngine.Vector2(wx, wy + WorldGenOptions.GenOptions.usingData.minDeepNorthDist).magnitude > 12000f + num)
                {
                    if (baseHeight > WorldGenOptions.GenOptions.usingData.minMountainHeight)
                    {
                        __result = WorldGenOptions.GenOptions.usingData.ashlandsSwitch;
                        return false;
                    }
                    __result = WorldGenOptions.GenOptions.usingData.deepNorthSwitch;
                    return false;
                }
                else
                {
                    if (baseHeight > WorldGenOptions.GenOptions.usingData.minMountainHeight)
                    {
                        __result = WorldGenOptions.GenOptions.usingData.mountainSwitch;
                        return false;
                    }
                    if (UnityEngine.Mathf.PerlinNoise((__instance.m_offset0 + wx) * WorldGenOptions.GenOptions.usingData.swampBiomeScaleX, (__instance.m_offset0 + wy) * WorldGenOptions.GenOptions.usingData.swampBiomeScaleY) > WorldGenOptions.GenOptions.usingData.minSwampNoise && magnitude > WorldGenOptions.GenOptions.usingData.minSwampDist && magnitude < WorldGenOptions.GenOptions.usingData.maxSwampDist && baseHeight > WorldGenOptions.GenOptions.usingData.minSwampHeight && baseHeight < WorldGenOptions.GenOptions.usingData.maxSwampHeight)
                    {
                        __result = WorldGenOptions.GenOptions.usingData.swampSwitch;
                        return false;
                    }
                    if (UnityEngine.Mathf.PerlinNoise((__instance.m_offset4 + wx) * WorldGenOptions.GenOptions.usingData.mistlandsBiomeScaleX, (__instance.m_offset4 + wy) * WorldGenOptions.GenOptions.usingData.mistlandsBiomeScaleY) > WorldGenOptions.GenOptions.usingData.minMistlandsNoise && magnitude > WorldGenOptions.GenOptions.usingData.minMistlandsDist + num && magnitude < WorldGenOptions.GenOptions.usingData.maxMistlandsDist)
                    {
                        __result = WorldGenOptions.GenOptions.usingData.mistlandsSwitch;
                        return false;
                    }
                    if (UnityEngine.Mathf.PerlinNoise((__instance.m_offset1 + wx) * WorldGenOptions.GenOptions.usingData.plainsBiomeScaleX, (__instance.m_offset1 + wy) * WorldGenOptions.GenOptions.usingData.plainsBiomeScaleY) > WorldGenOptions.GenOptions.usingData.minPlainsNoise && magnitude > WorldGenOptions.GenOptions.usingData.minPlainsDist + num && magnitude < WorldGenOptions.GenOptions.usingData.maxPlainsDist)
                    {
                        __result = WorldGenOptions.GenOptions.usingData.plainsSwitch;
                        return false;
                    }
                    if (UnityEngine.Mathf.PerlinNoise((__instance.m_offset2 + wx) * WorldGenOptions.GenOptions.usingData.plainsBiomeScaleX, (__instance.m_offset2 + wy) * WorldGenOptions.GenOptions.usingData.plainsBiomeScaleY) > WorldGenOptions.GenOptions.usingData.minPlainsNoise && magnitude > WorldGenOptions.GenOptions.usingData.minBlackForestDist + num && magnitude < WorldGenOptions.GenOptions.usingData.maxBlackForestDist)
                    {
                        __result = WorldGenOptions.GenOptions.usingData.blackForestSwitch;
                        return false;
                    }
                    if (magnitude > 5000f + num)
                    {
                        __result = WorldGenOptions.GenOptions.usingData.blackForestSwitch;
                        return false;
                    }
                    __result = WorldGenOptions.GenOptions.usingData.meadowsSwitch;
                    return false;
                }
            }
        }
    }
    
    //temp fix for StartTemple not spawning
    [HarmonyPatch(typeof(ZoneSystem))]
    [HarmonyPatch(nameof(ZoneSystem.GenerateLocations))]
    [HarmonyPatch(new Type[] { typeof(ZoneSystem.ZoneLocation) })]
    public static class StartTemple_SpawnFix
    {
        public static void Prefix(ref ZoneSystem.ZoneLocation location)
        {
            if(location.m_prefabName == "StartTemple")
            {
                location.m_biome = WorldGenOptions.GenOptions.usingData.meadowsSwitch;
            }
        }
    }
}