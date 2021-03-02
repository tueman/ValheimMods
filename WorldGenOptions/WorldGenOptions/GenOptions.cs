using System;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace WorldGenOptions
{
    [BepInPlugin(GUID, "WorldGenOptions", VERSION)]
    [BepInProcess("valheim.exe")]
    [BepInProcess("valheim_server.exe")]
    public class GenOptions : BaseUnityPlugin
    {
        private const string GUID = "org.github.spacedrive.worldgen";
        private const string VERSION = "0.1.0.0";

        // default, unmodded gen data
        public static WorldGenData defaultData = new WorldGenData();

        // data stored in config file
        public static WorldGenData savedData = new WorldGenData();

        // data loaded from biome data
        public static WorldGenData usingData = new WorldGenData();

        public static bool hasBiomeData = false;

        // config entries
        ConfigFile configFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "SpaceDrive-WorldGenOptions.cfg"), true);

        ConfigEntry<float> minMountainHeight;
        ConfigEntry<float> minAshlandsDist;
        ConfigEntry<float> minDeepNorthDist;

        ConfigEntry<float> swampBiomeScaleX;
        ConfigEntry<float> swampBiomeScaleY;
        ConfigEntry<float> minSwampNoise;
        ConfigEntry<float> minSwampDist;
        ConfigEntry<float> maxSwampDist;
        ConfigEntry<float> minSwampHeight;
        ConfigEntry<float> maxSwampHeight;

        ConfigEntry<float> mistlandsBiomeScaleX;
        ConfigEntry<float> mistlandsBiomeScaleY;
        ConfigEntry<float> minMistlandsNoise;
        ConfigEntry<float> minMistlandsDist;
        ConfigEntry<float> maxMistlandsDist;

        ConfigEntry<float> plainsBiomeScaleX;
        ConfigEntry<float> plainsBiomeScaleY;
        ConfigEntry<float> minPlainsNoise;
        ConfigEntry<float> minPlainsDist;
        ConfigEntry<float> maxPlainsDist;

        ConfigEntry<float> blackForestBiomeScaleX;
        ConfigEntry<float> blackForestBiomeScaleY;
        ConfigEntry<float> minBlackForestNoise;
        ConfigEntry<float> minBlackForestDist;
        ConfigEntry<float> maxBlackForestDist;

        ConfigEntry<string> meadowsSwitch;
        ConfigEntry<string> blackForestSwitch;
        ConfigEntry<string> swampSwitch;
        ConfigEntry<string> mountainSwitch;
        ConfigEntry<string> plainsSwitch;
        ConfigEntry<string> mistlandsSwitch;
        ConfigEntry<string> ashlandsSwitch;
        ConfigEntry<string> deepNorthSwitch;

        void InitializeConfig()
        {
            minAshlandsDist = configFile.Bind("Biomes.Ashlands", "AshlandsDistFromEdge", -4000f,
                "Distance from edge for ashlands to generate; negative values will generate " +
                "on south edge of map.");

            blackForestBiomeScaleX = configFile.Bind("Biomes.BlackForest", "BlackForestBiomeScaleX", 0.001f,
                "X scale of the black forest biome; can make the biome look more \"scattered\".");

            blackForestBiomeScaleY = configFile.Bind("Biomes.BlackForest", "BlackForestBiomeScaleY", 0.001f,
                "Y scale of the black forest biome; can make the biome look more \"scattered\".");

            minBlackForestNoise = configFile.Bind("Biomes.BlackForest", "MinBlackForestNoise", 0.4f,
                "Minimum noise for black forest to generate in; higher number will make the biome rare or " +
                "not spawn at all.");

            minBlackForestDist = configFile.Bind("Biomes.BlackForest", "MinBlackForestDist", 600f,
                "Closest distance to the center of the map that black forest can generate.");

            maxBlackForestDist = configFile.Bind("Biomes.BlackForest", "MaxBlackForestDist", 6000f,
                "Farthest distance from the center of the map that black forest can generate.");

            minDeepNorthDist = configFile.Bind("Biomes.DeepNorth", "DeepNorthDistFromEdge", 4000f,
                "Distance from edge for deep north to generate; negative values will generate " +
                "on south edge of map.");

            mistlandsBiomeScaleX = configFile.Bind("Biomes.Mistlands", "MistlandsBiomeScaleX", 0.001f,
                "X scale of the mistlands biome; can make the biome look more \"scattered\".");

            mistlandsBiomeScaleY = configFile.Bind("Biomes.Mistlands", "MistlandsBiomeScaleY", 0.001f,
                "Y scale of the mistlands biome; can make the biome look more \"scattered\".");

            minMistlandsNoise = configFile.Bind("Biomes.Mistlands", "MinMistlandsNoise", 0.5f,
                "Minimum noise for mistlands to generate in; higher number will make the biome rare or " +
                "not spawn at all.");

            minMistlandsDist = configFile.Bind("Biomes.Mistlands", "MinMistlandsDist", 6000f,
                "Closest distance to the center of the map that mistlands can generate.");

            maxMistlandsDist = configFile.Bind("Biomes.Mistlands", "MaxMistlandsDist", 10000f,
                "Farthest distance from the center of the map that mistlands can generate.");

            minMountainHeight = configFile.Bind("Biomes.Mountain", "MinMountainHeight", 0.4f,
                "Minimum height for mountains to spawn. Setting to below 0.2 may cause " +
                "game to hang while creating a new world.");

            plainsBiomeScaleX = configFile.Bind("Biomes.Plains", "PlainsBiomeScaleX", 0.001f,
                "X scale of the plains biome; can make the biome look more \"scattered\".");

            plainsBiomeScaleY = configFile.Bind("Biomes.Plains", "PlainsBiomeScaleY", 0.001f,
                "Y scale of the plains biome; can make the biome look more \"scattered\".");

            minPlainsNoise = configFile.Bind("Biomes.Plains", "MinPlainsNoise", 0.4f,
                "Minimum noise for plains to generate in; higher number will make the biome rare or " +
                "not spawn at all.");

            minPlainsDist = configFile.Bind("Biomes.Plains", "MinPlainsDist", 3000f,
                "Closest distance to the center of the map that plains can generate.");

            maxPlainsDist = configFile.Bind("Biomes.Plains", "MaxPlainsDist", 8000f,
                "Farthest distance from the center of the map that plains can generate.");

            swampBiomeScaleX = configFile.Bind("Biomes.Swamp", "SwampBiomeScaleX", 0.001f,
                "X scale of the swamp biome; can make the biome look more \"scattered\".");

            swampBiomeScaleY = configFile.Bind("Biomes.Swamp", "SwampBiomeScaleY", 0.001f,
                "Y scale of the swamp biome; can make the biome look more \"scattered\".");

            minSwampNoise = configFile.Bind("Biomes.Swamp", "MinSwampNoise", 0.6f,
                "Minimum noise for swamp to generate in; higher number will make the biome rare or " +
                "not spawn at all.");

            minSwampDist = configFile.Bind("Biomes.Swamp", "MinSwampDist", 2000f,
                "Closest distance to the center of the map that swamps can generate.");

            maxSwampDist = configFile.Bind("Biomes.Swamp", "MaxSwampDist", 8000f, 
                "Farthest distance from the center of the map that swamps can generate.");

            minSwampHeight = configFile.Bind("Biomes.Swamp", "MinSwampHeight", 0.05f,
                "Lowest height biomes can generate at.");

            maxSwampHeight = configFile.Bind("Biomes.Swamp", "MaxSwampHeight", 0.25f,
                "Highest height swamps can generate at.");

            meadowsSwitch = configFile.Bind("Switchers", "MeadowsSwitch", "meadows",
                "Replaces all meadows biomes with given biome. May cause game to be unable " +
                "to generate a new world.");

            blackForestSwitch = configFile.Bind("Switchers", "BlackForestSwitch", "blackforest",
                "Replaces all black forest biomes with given biome.");

            swampSwitch = configFile.Bind("Switchers", "SwampSwitch", "swamp",
                "Replaces all swamp biomes with given biome.");

            mountainSwitch = configFile.Bind("Switchers", "MountainSwitch", "mountain",
                "Replaces all mountain biomes with given biome.");

            plainsSwitch = configFile.Bind("Switchers", "PlainsSwitch", "plains",
                "Replaces all plains biomes with given biome.");

            mistlandsSwitch = configFile.Bind("Switchers", "MistlandsSwitch", "mistlands",
                "Replaces all mistlands biomes with given biome.");

            ashlandsSwitch = configFile.Bind("Switchers", "AshlandsSwitch", "ashlands",
                "Replaces all ashlands biomes with given biome.");

            deepNorthSwitch = configFile.Bind("Switchers", "DeepNorthSwitch", "deepnorth",
                "Replaces all deep north biomes with given biome.");
        }

        void AssignConfigData(ref WorldGenData data)
        {
            data.minMountainHeight = minMountainHeight.Value;
            data.minAshlandsDist = minAshlandsDist.Value;
            data.minDeepNorthDist = minDeepNorthDist.Value;

            data.swampBiomeScaleX = swampBiomeScaleX.Value;
            data.swampBiomeScaleY = swampBiomeScaleY.Value;
            data.minSwampNoise = minSwampNoise.Value;
            data.minSwampDist = minSwampDist.Value;
            data.maxSwampDist = maxSwampDist.Value;
            data.minSwampHeight = minSwampHeight.Value;
            data.maxSwampHeight = maxSwampHeight.Value;

            data.mistlandsBiomeScaleX = mistlandsBiomeScaleX.Value;
            data.mistlandsBiomeScaleY = mistlandsBiomeScaleY.Value;
            data.minMistlandsNoise = minMistlandsNoise.Value;
            data.minMistlandsDist = minMistlandsDist.Value;
            data.maxMistlandsDist = maxMistlandsDist.Value;

            data.plainsBiomeScaleX = plainsBiomeScaleX.Value;
            data.plainsBiomeScaleY = plainsBiomeScaleY.Value;
            data.minPlainsNoise = minPlainsNoise.Value;
            data.minPlainsDist = minPlainsDist.Value;
            data.maxPlainsDist = maxPlainsDist.Value;

            data.blackForestBiomeScaleX = blackForestBiomeScaleX.Value;
            data.blackForestBiomeScaleY = blackForestBiomeScaleY.Value;
            data.minBlackForestNoise = minBlackForestNoise.Value;
            data.minBlackForestDist = minBlackForestDist.Value;
            data.maxBlackForestDist = maxBlackForestDist.Value;

            data.meadowsSwitch = AssignSwitch(meadowsSwitch.Value);
            data.blackForestSwitch = AssignSwitch(blackForestSwitch.Value);
            data.swampSwitch = AssignSwitch(swampSwitch.Value);
            data.mountainSwitch = AssignSwitch(mountainSwitch.Value);
            data.plainsSwitch = AssignSwitch(plainsSwitch.Value);
            data.mistlandsSwitch = AssignSwitch(mistlandsSwitch.Value);
            data.ashlandsSwitch = AssignSwitch(ashlandsSwitch.Value);
            data.deepNorthSwitch = AssignSwitch(deepNorthSwitch.Value);
        }

        Heightmap.Biome AssignSwitch(string biome)
        {
            switch(biome.ToLower())
            {
                case "ashlands":
                case "ash lands":
                    return Heightmap.Biome.AshLands;
                case "blackforest":
                case "black forest":
                    return Heightmap.Biome.BlackForest;
                case "deepnorth":
                case "deep north":
                    return Heightmap.Biome.DeepNorth;
                case "meadows":
                case "meadow":
                    return Heightmap.Biome.Meadows;
                case "plains":
                case "plain":
                    return Heightmap.Biome.Plains;
                case "swamp":
                case "swamps":
                    return Heightmap.Biome.Swamp;
                case "mistlands":
                case "mistland":
                case "mist lands":
                case "mist land":
                    return Heightmap.Biome.Mistlands;
                case "mountain":
                case "mountains":
                    return Heightmap.Biome.Mountain;
                default:
                    ZLog.LogError(biome + " is an incorrect biome name!");
                    return Heightmap.Biome.Meadows;
            }
        }

        void Awake()
        {
            Harmony harmony = new Harmony(GUID);

            InitializeConfig();
            AssignConfigData(ref savedData);

            harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(WorldGenerator))]
    [HarmonyPatch(nameof(WorldGenerator.GetBiome))]
    [HarmonyPatch(new Type[] {typeof(float), typeof(float)})]
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
                if (new UnityEngine.Vector2(wx, wy + GenOptions.usingData.minAshlandsDist).magnitude > 12000f + num)
                {
                    __result = GenOptions.usingData.ashlandsSwitch;
                    return false;
                }
                if ((double)baseHeight <= 0.02)
                {
                    __result = Heightmap.Biome.Ocean;
                    return false;
                }
                if (new UnityEngine.Vector2(wx, wy + GenOptions.usingData.minDeepNorthDist).magnitude > 12000f + num)
                {
                    if (baseHeight > GenOptions.usingData.minMountainHeight)
                    {
                        __result = GenOptions.usingData.ashlandsSwitch;
                        return false;
                    }
                    __result = GenOptions.usingData.deepNorthSwitch;
                    return false;
                }
                else
                {
                    if (baseHeight > GenOptions.usingData.minMountainHeight)
                    {
                        __result = GenOptions.usingData.mountainSwitch;
                        return false;
                    }
                    if (UnityEngine.Mathf.PerlinNoise((__instance.m_offset0 + wx) * GenOptions.usingData.swampBiomeScaleX, (__instance.m_offset0 + wy) * GenOptions.usingData.swampBiomeScaleY) > GenOptions.usingData.minSwampNoise && magnitude > GenOptions.usingData.minSwampDist && magnitude < GenOptions.usingData.maxSwampDist && baseHeight > GenOptions.usingData.minSwampHeight && baseHeight < GenOptions.usingData.maxSwampHeight)
                    {
                        __result = GenOptions.usingData.swampSwitch;
                        return false;
                    }
                    if (UnityEngine.Mathf.PerlinNoise((__instance.m_offset4 + wx) * GenOptions.usingData.mistlandsBiomeScaleX, (__instance.m_offset4 + wy) * GenOptions.usingData.mistlandsBiomeScaleY) > GenOptions.usingData.minMistlandsNoise && magnitude > GenOptions.usingData.minMistlandsDist + num && magnitude < GenOptions.usingData.maxMistlandsDist)
                    {
                        __result = GenOptions.usingData.mistlandsSwitch;
                        return false;
                    }
                    if (UnityEngine.Mathf.PerlinNoise((__instance.m_offset1 + wx) * GenOptions.usingData.plainsBiomeScaleX, (__instance.m_offset1 + wy) * GenOptions.usingData.plainsBiomeScaleY) > GenOptions.usingData.minPlainsNoise && magnitude > GenOptions.usingData.minPlainsDist + num && magnitude < GenOptions.usingData.maxPlainsDist)
                    {
                        __result = GenOptions.usingData.plainsSwitch;
                        return false;
                    }
                    if (UnityEngine.Mathf.PerlinNoise((__instance.m_offset2 + wx) * GenOptions.usingData.plainsBiomeScaleX, (__instance.m_offset2 + wy) * GenOptions.usingData.plainsBiomeScaleY) > GenOptions.usingData.minPlainsNoise && magnitude > GenOptions.usingData.minBlackForestDist + num && magnitude < GenOptions.usingData.maxBlackForestDist)
                    {
                        __result = GenOptions.usingData.blackForestSwitch;
                        return false;
                    }
                    if (magnitude > 5000f + num)
                    {
                        __result = GenOptions.usingData.blackForestSwitch;
                        return false;
                    }
                    __result = GenOptions.usingData.meadowsSwitch;
                    return false;
                }
            }
        }
    }
}
