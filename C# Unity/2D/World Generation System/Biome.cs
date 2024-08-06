using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Biome")]
public class Biome : ScriptableObject
{
    public string biomeName;

    public Tile rockTile;

    public Tile[] commonOreTiles;
    public Tile[] rareOreTiles;
    public Tile[] uniqueOreTiles;

    public Tile loot;
    public Tile[] gemstones;
}
