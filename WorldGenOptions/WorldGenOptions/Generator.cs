using System;
using System.Collections.Generic;
using System.Linq;
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
                        __result = WorldGenOptions.GenOptions.usingData.mountainSwitch;
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
    
    //fix for StartTemple not spawning
    [HarmonyPatch(typeof(ZoneSystem))]
    [HarmonyPatch(nameof(ZoneSystem.GenerateLocations))]
    [HarmonyPatch(new Type[] { typeof(ZoneSystem.ZoneLocation) })]
    [HarmonyDebug]
    public static class StartTemple_SpawnFix
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            int numAnds = 0;
            int check = 0;
            Label correctBiome = il.DefineLabel();

            // find second usage of & operator
            for(int i = 0; i < codes.Count; ++i)
            {
                if(codes[i].opcode == OpCodes.And)
                {
                    ++numAnds;
                    if(numAnds == 2)
                    {
                        // take it back now y'all
                        check = i - 3;
                        break;
                    }
                }
            }

            codes[check + 10].labels.Add(correctBiome);

            // add check if __instance.prefabName == "StartTemple"; if so, skip biome checking
            List<CodeInstruction> addCodes = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldfld, typeof(ZoneSystem.ZoneLocation).GetField(nameof(ZoneSystem.ZoneLocation.m_prefabName))),
                new CodeInstruction(OpCodes.Ldstr, "StartTemple"),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(String), "op_Equality")),
                new CodeInstruction(OpCodes.Brtrue, correctBiome)
            };
        
            codes.InsertRange(check, addCodes);

            return codes.AsEnumerable();
        }
    }

    [HarmonyPatch(typeof(WorldGenerator))]
    [HarmonyPatch(nameof(WorldGenerator.PlaceRivers))]
    public static class PlaceRiverPrefix
    {
        public static bool Prefix(ref WorldGenerator __instance, ref List<WorldGenerator.River> __result)
        {
            UnityEngine.Random.State state = UnityEngine.Random.state;
            UnityEngine.Random.InitState(__instance.m_riverSeed);
            DateTime now = DateTime.Now;
            List<WorldGenerator.River> list = new List<WorldGenerator.River>();
            List<UnityEngine.Vector2> list2 = new List<UnityEngine.Vector2>(__instance.m_lakes);
            while (list2.Count > 1)
            {
                UnityEngine.Vector2 vector = list2[0];
                int num = __instance.FindRandomRiverEnd(list, __instance.m_lakes, vector, WorldGenOptions.GenOptions.usingData.riverMultipleMaxDistance, 0.4f, 128f);
                if (num == -1 && !__instance.HaveRiver(list, vector))
                {
                    num = __instance.FindRandomRiverEnd(list, __instance.m_lakes, vector, 5000f, 0.4f, 128f);
                }
                if (num != -1)
                {
                    WorldGenerator.River river = new WorldGenerator.River();
                    river.p0 = vector;
                    river.p1 = __instance.m_lakes[num];
                    river.center = (river.p0 + river.p1) * 0.5f;
                    river.widthMax = UnityEngine.Random.Range(WorldGenOptions.GenOptions.usingData.riverWidthMaxLowerRange, WorldGenOptions.GenOptions.usingData.riverWidthMaxUpperRange);
                    river.widthMin = UnityEngine.Random.Range(WorldGenOptions.GenOptions.usingData.riverWidthMinLowerRange, river.widthMax);
                    float num2 = UnityEngine.Vector2.Distance(river.p0, river.p1);
                    river.curveWidth = num2 / WorldGenOptions.GenOptions.usingData.riverCurveWidth;
                    river.curveWavelength = num2 / WorldGenOptions.GenOptions.usingData.riverWavelength;
                    list.Add(river);
                }
                else
                {
                    list2.RemoveAt(0);
                }
            }
            ZLog.Log("Rivers:" + list.Count);
            __instance.RenderRivers(list);
            ZLog.Log("River Calc time " + (DateTime.Now - now).TotalMilliseconds + " ms");
            UnityEngine.Random.state = state;
            __result = list;
            return false;
        }
    }
}