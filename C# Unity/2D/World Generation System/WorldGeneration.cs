using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WorldGeneration : MonoBehaviour
{
    public Biome[] biomes;
    int activeBiome;

    Tile activeTile;
    public List<GameObject> tiles;
    public GameObject baseTile;
    public GameObject lootTile;
    public GameObject gemstoneTile;
    public GameObject[] trapTile;
    int gemSprite;
    public int worldLength;
    public int worldHeight;
    Vector3 spawnPosition;

    public Texture2D noiseTexture;
    public float caveFrequency;
    public float seed;
    public int currentYlevel;

    public List<Vector2> blockedSpawns;
    void Start()
    {
        seed = Random.Range(-10000, 10000);
        GenerateNoiseTexture();
        GenerateMineShaft();
        GenerateWorld();
        GenerateLoot();
        GenerateGemstones();
        SpawnTrap();
    }

    private void GenerateMineShaft()
    {
        yLevelCheck();
        for(int i = -20; i < worldLength; i++)
        {
            activeTile = biomes[activeBiome].rockTile;
            spawnPosition = new Vector2(i, 0);
            SpawnTile();
        }
    }
    private void GenerateWorld()
    {
        for(int x = -20; x < worldLength; x++)
        {
            for (int y = 1; y < worldHeight; y++)
            {
                if(noiseTexture.GetPixel(x,y).r < 0.5)
                {
                    currentYlevel = y;
                    yLevelCheck();
                    ChooseBlock();
                    spawnPosition = new Vector3(x, -y, 0);
                    SpawnTile();
                }
            }
        }
    }

    public void SpawnTile()
    {
        GameObject newTile = Instantiate(baseTile, spawnPosition, Quaternion.identity);
        newTile.transform.parent = gameObject.transform;
        newTile.name = activeTile.name;
        newTile.GetComponent<SpriteRenderer>().sprite = activeTile.tileSprite;
        newTile.GetComponent<TileDataPlaceholder>().thisTile = activeTile;
        tiles.Add(newTile);

        ParticleSystem ps = newTile.GetComponentInChildren<ParticleSystem>();
        var main = ps.main;
        main.startColor = activeTile.breakingParticlesColor;
    }

    public void GenerateNoiseTexture()
    {
        noiseTexture = new Texture2D(worldLength * 2, worldHeight * 2);

        for(int x = 0; x < noiseTexture.width; x++)
        {
            for(int y = 0; y < noiseTexture.height; y++)
            {
                float v = Mathf.PerlinNoise((x + seed) * caveFrequency, (y + seed) * caveFrequency);
                noiseTexture.SetPixel(x, y, new Color(v, v, v));
            }
        }
        noiseTexture.Apply();
    }

    public void NewDay()
    {
        currentYlevel = 0;
        tiles.Clear();
        blockedSpawns.Clear();
        seed = Random.Range(-10000, 10000);
        GenerateNoiseTexture();
        GenerateMineShaft();
        GenerateWorld();
        GenerateLoot();
        GenerateGemstones();
        SpawnTrap();
    }

    public void yLevelCheck()
    {
        //Compare to Biome Level (if above minimum and below max)
        if (currentYlevel <= 100)
        {
            activeBiome = 0;
        }
        if (currentYlevel > 100 && currentYlevel <= 200)
        {
            activeBiome = 1;
        }
        if (currentYlevel > 200 && currentYlevel <= 300)
        {
            activeBiome = 2;
        }
    }

    public void ChooseBlock()
    {
        int num = Random.Range(0, 101);
        if (num <= 90)
        {
            activeTile = biomes[activeBiome].rockTile;
        }
        if (num > 90 && num <= 96)
        {
            activeTile = biomes[activeBiome].commonOreTiles[0];
        }
        if (num > 96 && num <= 99)
        {
            activeTile = biomes[activeBiome].rareOreTiles[0];
        }
        if (num > 99 && num <= 100)
        {
            activeTile = biomes[activeBiome].uniqueOreTiles[0];
        }
    }

    public void GenerateLoot()
    {
        currentYlevel = 0;

        for (int x = -20; x < worldLength; x++)
        {
            for (int y = 1; y < worldHeight; y++)
            {
                if (noiseTexture.GetPixel(x, y).r > 0.5)
                {
                    currentYlevel = y;
                    yLevelCheck();
                    spawnPosition = new Vector3(x, -y, 0);

                    //Loot Tile
                    if(noiseTexture.GetPixel(x, y + 1).r < 0.5)
                    {
                        int ran = Random.Range(0, 101);
                        if (ran > 97) //3% Chance
                        {
                            GameObject loot = Instantiate(lootTile, spawnPosition, Quaternion.identity);
                            TileDataPlaceholder tt = loot.GetComponent<TileDataPlaceholder>();
                            tt.thisTile = biomes[activeBiome].loot;
                            loot.GetComponent<SpriteRenderer>().sprite = tt.thisTile.tileSprite;
                            tiles.Add(loot);
                            loot.transform.parent = gameObject.transform;
                            blockedSpawns.Add(new Vector2(x, y));

                            ParticleSystem ps = loot.GetComponentInChildren<ParticleSystem>();
                            var main = ps.main;
                            main.startColor = activeTile.breakingParticlesColor;
                        }
                    }
                }
            }
        }
    }

    public void GenerateGemstones()
    {
        currentYlevel = 0;

        for (int x = -20; x < worldLength; x++)
        {
            for (int y = 1; y < worldHeight; y++)
            {
                if (noiseTexture.GetPixel(x, y).r > 0.5)
                {
                    currentYlevel = y;
                    yLevelCheck();
                    spawnPosition = new Vector3(x, -y, 0);

                    foreach(Vector2 bs in blockedSpawns)
                    {
                        if(bs == new Vector2(x, y))
                        {
                            break;
                        }
                    }

                    //DeterminePossibleSpawn
                    if (noiseTexture.GetPixel(x + 1, y).r < 0.5)
                    {
                        gemSprite = 2; //Right
                        SpawnGem();
                    }
                    else if (noiseTexture.GetPixel(x - 1, y).r < 0.5)
                    {
                        gemSprite = 3; //Left
                        SpawnGem();
                    }
                    else if (noiseTexture.GetPixel(x, y + 1).r < 0.5)
                    {
                        gemSprite = 0; //Normal
                        SpawnGem();
                    }
                    else if (noiseTexture.GetPixel(x, y - 1).r < 0.5)
                    {
                        gemSprite = 1; //Down
                        SpawnGem();
                    }
                }
            }
        }
    }

    public void SpawnGem()
    {
        int spawnChance = Random.Range(0, 101);
        if (spawnChance > 99)
        {    
            GameObject gem = Instantiate(gemstoneTile, spawnPosition, Quaternion.identity);
            TileDataPlaceholder tt = gem.GetComponent<TileDataPlaceholder>();

            int ran = Random.Range(0, biomes[activeBiome].gemstones.Length);
            tt.thisTile = biomes[activeBiome].gemstones[ran];
            gem.GetComponentInChildren<Light2D>().color = tt.thisTile.gemstoneGlow;
            gem.GetComponent<SpriteRenderer>().sprite = tt.thisTile.gemSprite[gemSprite];
            tiles.Add(gem);
            gem.transform.parent = gameObject.transform;

            ParticleSystem ps = gem.GetComponentInChildren<ParticleSystem>();
            var main = ps.main;
            main.startColor = activeTile.breakingParticlesColor;
        }
    }

    public void SpawnTrap()
    {
        for (int x = -20; x < worldLength; x++)
        {
            for (int y = 1; y < worldHeight; y++)
            {
                if (noiseTexture.GetPixel(x, y).r > 0.5)
                {
                    currentYlevel = y;
                    yLevelCheck();
                    spawnPosition = new Vector3(x, -y, 0);

                    foreach (Vector2 bs in blockedSpawns)
                    {
                        if (bs == new Vector2(x, y))
                        {
                            break;
                        }
                    }

                    //Trap Tile
                    if (noiseTexture.GetPixel(x, y + 1).r < 0.5)
                    {
                        int ran = Random.Range(0, 201);
                        if (ran > 198) //1% Chance
                        {
                            int t = Random.Range(0, trapTile.Length);
                            GameObject trap = Instantiate(trapTile[t], spawnPosition, Quaternion.identity);
                            tiles.Add(trap);
                            trap.transform.parent = gameObject.transform;
                            blockedSpawns.Add(new Vector2(x, y));
                        }
                    }
                }
            }
        }
    }
}
