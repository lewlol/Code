using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "World Tile")]
public class Tile : ScriptableObject
{
    [Header("Tile Enum Information")]
    public TileNames tileName;
    public TileTypes tileType;

    [Header("Rock / Ore Sprite")]
    public Sprite tileSprite;

    [Header("Gemstone Sprites")]
    [Description("Order: Normal, Upside Down, Facing Left, Facing Right")]
    public Sprite[] gemSprite;

    [Header("Gemstone Misc")]
    public Color gemstoneGlow;

    [Header("Tile Mining Data")]
    public int breakingpower;
    public int coinWorth;
    public Color breakingParticlesColor;
    public AudioClip miningSound;
}
