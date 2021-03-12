public class WorldGenData
{
    // Config Values

    // increasing noise values too high may prevent biome from spawning

    // scale options are a bit odd but from testing increasing the values can 
    // make the biome look very scattered

    public float minMountainHeight = 0.4f; // 0.4

    // for ashlands/deep north values high number = closer to spawn
    public float minAshlandsDist = -4000f; // -4000
    public float minDeepNorthDist = 4000f; // 4000

    // swamp values
    public float swampBiomeScaleX = 0.001f; // 0.001
    public float swampBiomeScaleY = 0.001f; // 0.001
    public float minSwampNoise = 0.6f; // 0.6
    public float minSwampDist = 2000f; // 2000
    public float maxSwampDist = 8000f; // 8000
    public float minSwampHeight = 0.05f; // 0.05
    public float maxSwampHeight = 0.25f; // 0.25

    // mistlands values
    public float mistlandsBiomeScaleX = 0.001f; // 0.001
    public float mistlandsBiomeScaleY = 0.001f; // 0.001
    public float minMistlandsNoise = 0.5f; // 0.5
    public float minMistlandsDist = 6000f; // 6000
    public float maxMistlandsDist = 10000f; // 10000

    // plains values
    public float plainsBiomeScaleX = 0.001f; // 0.001
    public float plainsBiomeScaleY = 0.001f; // 0.001
    public float minPlainsNoise = 0.4f; // 0.4
    public float minPlainsDist = 3000f; // 3000
    public float maxPlainsDist = 8000f; // 8000

    // black forest values
    public float blackForestBiomeScaleX = 0.001f; // 0.001
    public float blackForestBiomeScaleY = 0.001f; // 0.001
    public float minBlackForestNoise = 0.4f; // 0.4
    public float minBlackForestDist = 600f; // 600
    public float maxBlackForestDist = 6000f; // 6000

    // biome switches
    public Heightmap.Biome meadowsSwitch = Heightmap.Biome.Meadows;
    public Heightmap.Biome blackForestSwitch = Heightmap.Biome.BlackForest;
    public Heightmap.Biome swampSwitch = Heightmap.Biome.Swamp;
    public Heightmap.Biome mountainSwitch = Heightmap.Biome.Mountain;
    public Heightmap.Biome plainsSwitch = Heightmap.Biome.Plains;
    public Heightmap.Biome mistlandsSwitch = Heightmap.Biome.Mistlands;
    public Heightmap.Biome ashlandsSwitch = Heightmap.Biome.AshLands;
    public Heightmap.Biome deepNorthSwitch = Heightmap.Biome.DeepNorth;

    // river
    public float riverMultipleMaxDistance = 2000f; // 2000
    public float riverExtremeMaxDistance = 5000f; // 5000
    public float riverMaxHeight = 0.4f; // 0.4
    public float riverWidthMaxLowerRange = 60f; // 60
    public float riverWidthMaxUpperRange = 100f; // 100
    public float riverWidthMinLowerRange = 60f; // 60 No upper range as river.widthMax is it
    public float riverCurveWidth = 15f; // 15
    public float riverWavelength = 20f; // 20

    public void WriteBiomeData(ref ZPackage package)
    {
        //mountain
        package.Write(minMountainHeight);

        // ashlands/deepnorth
        package.Write(minAshlandsDist);
        package.Write(minDeepNorthDist);

        // swamp
        package.Write(swampBiomeScaleX);
        package.Write(swampBiomeScaleY);
        package.Write(minSwampNoise);
        package.Write(minSwampDist);
        package.Write(maxSwampDist);
        package.Write(minSwampHeight);
        package.Write(maxSwampHeight);

        // mistlands
        package.Write(mistlandsBiomeScaleX);
        package.Write(mistlandsBiomeScaleY);
        package.Write(minMistlandsNoise);
        package.Write(minMistlandsDist);
        package.Write(maxMistlandsDist);

        // plains
        package.Write(plainsBiomeScaleX);
        package.Write(plainsBiomeScaleY);
        package.Write(minPlainsNoise);
        package.Write(minPlainsDist);
        package.Write(maxPlainsDist);

        // black forest
        package.Write(blackForestBiomeScaleX);
        package.Write(blackForestBiomeScaleY);
        package.Write(minBlackForestNoise);
        package.Write(minBlackForestDist);
        package.Write(maxBlackForestDist);

        // switches
        package.Write((int)meadowsSwitch);
        package.Write((int)blackForestSwitch);
        package.Write((int)swampSwitch);
        package.Write((int)mountainSwitch);
        package.Write((int)plainsSwitch);
        package.Write((int)mistlandsSwitch);
        package.Write((int)ashlandsSwitch);
        package.Write((int)deepNorthSwitch);

        // river
        package.Write(riverMultipleMaxDistance);
        package.Write(riverExtremeMaxDistance);
        package.Write(riverMaxHeight);
        package.Write(riverWidthMaxLowerRange);
        package.Write(riverWidthMaxUpperRange);
        package.Write(riverWidthMinLowerRange);
        package.Write(riverCurveWidth);
        package.Write(riverWavelength);
    }

    public void ReadBiomeData(ref ZPackage package)
    {
        // mountain
        minMountainHeight = package.ReadSingle();

        // ashlands/deepnorth
        minAshlandsDist = package.ReadSingle();
        minDeepNorthDist = package.ReadSingle();

        // swamp
        swampBiomeScaleX = package.ReadSingle();
        swampBiomeScaleY = package.ReadSingle();
        minSwampNoise = package.ReadSingle();
        minSwampDist = package.ReadSingle();
        maxSwampDist = package.ReadSingle();
        minSwampHeight = package.ReadSingle();
        maxSwampHeight = package.ReadSingle();

        // mistlands
        mistlandsBiomeScaleX = package.ReadSingle();
        mistlandsBiomeScaleY = package.ReadSingle();
        minMistlandsNoise = package.ReadSingle();
        minMistlandsDist = package.ReadSingle();
        maxMistlandsDist = package.ReadSingle();

        // plains
        plainsBiomeScaleX = package.ReadSingle();
        plainsBiomeScaleY = package.ReadSingle();
        minPlainsNoise = package.ReadSingle();
        minPlainsDist = package.ReadSingle();
        maxPlainsDist = package.ReadSingle();

        // black forest
        blackForestBiomeScaleX = package.ReadSingle();
        blackForestBiomeScaleY = package.ReadSingle();
        minBlackForestNoise = package.ReadSingle();
        minBlackForestDist = package.ReadSingle();
        maxBlackForestDist = package.ReadSingle();

        // switches
        meadowsSwitch = (Heightmap.Biome)package.ReadInt();
        blackForestSwitch = (Heightmap.Biome)package.ReadInt();
        swampSwitch = (Heightmap.Biome)package.ReadInt();
        mountainSwitch = (Heightmap.Biome)package.ReadInt();
        plainsSwitch = (Heightmap.Biome)package.ReadInt();
        mistlandsSwitch = (Heightmap.Biome)package.ReadInt();
        ashlandsSwitch = (Heightmap.Biome)package.ReadInt();
        deepNorthSwitch = (Heightmap.Biome)package.ReadInt();

        // river
        riverMultipleMaxDistance = package.ReadSingle();
        riverExtremeMaxDistance = package.ReadSingle();
        riverMaxHeight = package.ReadSingle();
        riverWidthMaxLowerRange = package.ReadSingle();
        riverWidthMaxUpperRange = package.ReadSingle();
        riverWidthMinLowerRange = package.ReadSingle();
        riverCurveWidth = package.ReadSingle();
        riverWavelength = package.ReadSingle();

    }
}