using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;

namespace SaveData
{
    public static class SavingData
    {
        public const string saveFileSuffix = "_biomeData";

        public static string GetBiomeSavePath(World world)
        {
            return world.m_worldSavePath + "/" + world.m_name + saveFileSuffix + ".fwl";
        }
        
        public static string GetBiomeSavePath(string name)
        {
            return World.GetWorldSavePath() + "/" + name + saveFileSuffix + ".fwl";
        }
    }

    [HarmonyPatch(typeof(World))]
    [HarmonyPatch(MethodType.Constructor)]
    [HarmonyPatch(new Type[] { typeof(string), typeof(string)})]
    public static class SaveData_BiomeDataPatch
    {
        public static void Postfix(ref World __instance, string name)
        {
            if(name == "menu")
            {
                return;
            }
            ZPackage biomePackage = new ZPackage();

            WorldGenOptions.GenOptions.savedData.WriteBiomeData(ref biomePackage);

            string biomePath = SavingData.GetBiomeSavePath(__instance);
            byte[] biomeArray = biomePackage.GetArray();
            FileStream biomeStream = File.Create(biomePath);
            BinaryWriter biomeBinaryWriter = new BinaryWriter(biomeStream);
            biomeBinaryWriter.Write(biomeArray.Length);
            biomeBinaryWriter.Write(biomeArray);
            biomeBinaryWriter.Flush();
            biomeStream.Flush(true);
            biomeStream.Close();
            biomeStream.Dispose();
        }
    }

    [HarmonyPatch(typeof(WorldGenerator))]
    [HarmonyPatch(MethodType.Constructor)]
    [HarmonyPatch(new Type[] { typeof(World)})]
    public static class LoadData_BiomeDataPatch
    {
        public static void Prefix(World world)
        {
            if(world.m_menu)
            {
                return;
            }

            FileStream biomeStream = null;
            try
            {
                biomeStream = File.OpenRead(SavingData.GetBiomeSavePath(world));
            }
            catch 
            {
                if(biomeStream == null)
                {
                    UnityEngine.Debug.Log("No biome data found.");
                    WorldGenOptions.GenOptions.hasBiomeData = false;
                    WorldGenOptions.GenOptions.usingData = WorldGenOptions.GenOptions.defaultData;
                    return;
                }
            }
            UnityEngine.Debug.Log("Biome data found for " + world.m_name + ".");
            WorldGenOptions.GenOptions.hasBiomeData = true;
            WorldGenData data = new WorldGenData();
            try
            {
                BinaryReader reader = new BinaryReader(biomeStream);
                int count = reader.ReadInt32();
                ZPackage package = new ZPackage(reader.ReadBytes(count));
                data.ReadBiomeData(ref package);
            }
            catch
            {
                ZLog.LogWarning("Incomplete biome data for " + world.m_name);
            }
            finally
            {
                if(biomeStream != null)
                {
                    biomeStream.Dispose();
                }
                WorldGenOptions.GenOptions.usingData = data;
            }
        }
    }

    [HarmonyPatch(typeof(World))]
    [HarmonyPatch(nameof(World.RemoveWorld))]
    public static class RemoveWorld_BiomePatch
    {
        public static void Postfix(string name)
        {
            try
            {
                File.Delete(SavingData.GetBiomeSavePath(name));
            }
            catch
            {
            }
        }
    }

    //[HarmonyPatch(typeof(World))]
    //[HarmonyPatch(nameof(World.GetWorldList))]
    public static class ClearBrokenData_BiomePatch
    {
        // FOR CLEARING BROKEN BIOME DATA. DO NOT IMPLEMENT IN RELEASE BUILDS
        public static void Prefix()
        {
            UnityEngine.Debug.Log("Clearing all biome data. If you see this message, please submit a bug report to the mod author.");
            string[] array = Directory.GetFiles(World.GetWorldSavePath(), "*" + SavingData.saveFileSuffix);
            foreach (string file in array)
            {
                File.Delete(file);
            }
        }
    }

    [HarmonyPatch(typeof(World))]
    [HarmonyPatch(nameof(World.GetWorldList))]
    public static class GetWorldList_BiomePatch
    {
        public static void Postfix(ref List<World> __result)
        {
            UnityEngine.Debug.Log("Getting world list.");
            List<World> worlds = new List<World>(__result);
            foreach(World world in __result)
            {
                if(world.m_name.Contains(SavingData.saveFileSuffix))
                {
                    UnityEngine.Debug.Log(world.m_name + " is biome data. Skipping.");
                    worlds.Remove(world);
                }
            }
            __result = worlds;
        }
    }
}