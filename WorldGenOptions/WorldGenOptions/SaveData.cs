using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;

namespace SaveData
{
    public static class SavingData
    {
        public const string saveFileSuffix = "_genData";

        public static string GetGenSavePathRaw()
        {
            return World.GetWorldSavePath() + "/GenData/";
        }
        
        public static string GetGenSavePath(string name)
        {
            return GetGenSavePathRaw() + name + saveFileSuffix + ".fwl";
        }

        // deprecated; only implemented temporarily
        public const string saveFileSuffixOld = "_biomeData";

        public static string GetBiomeSavePath(string name)
        {
            return World.GetWorldSavePath() + "/" + name + saveFileSuffixOld + ".fwl";
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

            string biomePath = SavingData.GetGenSavePath(__instance.m_name);
            byte[] biomeArray = biomePackage.GetArray();

            if(!File.Exists(SavingData.GetGenSavePathRaw()))
            {
                Directory.CreateDirectory(SavingData.GetGenSavePathRaw());
            }

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
                biomeStream = File.OpenRead(SavingData.GetGenSavePath(world.m_name));
            }
            catch 
            {
                if(biomeStream == null)
                {
                    UnityEngine.Debug.Log("No gen data found.");
                    WorldGenOptions.GenOptions.hasBiomeData = false;
                    WorldGenOptions.GenOptions.usingData = WorldGenOptions.GenOptions.defaultData;
                    return;
                }
            }
            UnityEngine.Debug.Log("Gen data found for " + world.m_name + ".");
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
                ZLog.LogWarning("Incomplete gen data for " + world.m_name);
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
                File.Delete(SavingData.GetGenSavePath(name));
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
        // FOR CLEARING BROKEN GEN DATA. DO NOT IMPLEMENT IN RELEASE BUILDS
        public static void Prefix()
        {
            UnityEngine.Debug.Log("Clearing all gen data. If you see this message, please submit a bug report to the mod author.");
            string[] array = Directory.GetFiles(SavingData.GetGenSavePathRaw(), "*.fwl");
            foreach (string file in array)
            {
                File.Delete(file);
            }
            Directory.Delete(SavingData.GetGenSavePathRaw());
        }
    }

    [HarmonyPatch(typeof(World))]
    [HarmonyPatch(nameof(World.GetWorldList))]
    public static class GetWorldList_BiomePatch
    {
        // implement temporarily while users load worlds with outdated save data
        public static void Postfix(ref List<World> __result)
        {
            UnityEngine.Debug.Log("Getting world list.");
            List<World> worlds = new List<World>(__result);

            if (!File.Exists(SavingData.GetGenSavePathRaw()))
            {
                Directory.CreateDirectory(SavingData.GetGenSavePathRaw());
            }

            foreach (World world in __result)
            {
                if(world.m_name.Contains(SavingData.saveFileSuffixOld))
                {
                    UnityEngine.Debug.Log(world.m_name + " is biome data. Moving file to new location.");

                    string orig = World.GetWorldSavePath() + "/" + world.m_name + ".fwl";
                    string worldName = world.m_name.Replace(SavingData.saveFileSuffixOld, "");
                    string newFile = SavingData.GetGenSavePathRaw() + worldName + SavingData.saveFileSuffix + ".fwl";
                    File.Move(orig, newFile);

                    worlds.Remove(world);
                }
            }

            __result = worlds;
        }
    }
}