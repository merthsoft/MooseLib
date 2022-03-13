﻿namespace Merthsoft.Moose.Dungeon.Tiles;
public enum ItemTile
{
    None = -1,
    POTION_START = 0,
    PurplePotion = POTION_START,
    OrangePotion,
    BluePotion,
    GreenPotion,
    RedPotion,
    POTION_END,

    WEAPON_START = 10,
    SilverAxe = 10,
    GoldenAxe,
    SilverHammer,
    GoldenHammer,
    SilverBow,
    GoldenBow,
    SilverSword,
    GoldenSword,
    SilverWand,
    GoldenWand,
    WEAPON_END,

    ARMOR_START = 20,
    SilverArmor = ARMOR_START,
    GoldenArmor,
    GreenArmor,
    GreenCloak,
    PurpleCloak,
    RedCloak,
    ARMOR_END,

    TREASURE_START = 30,
    GoldenRing = TREASURE_START,
    BronzeRing,
    SilverRing,
    LargeSilverPile,
    MediumSilverPile,
    SmallSilverPile,
    LargeGoldPile,
    MediumGoldPile,
    SmallGoldPile,
    JadeNecklace,
    EmeraldNecklace,
    FoxNecklace,
    TopazNecklace,
    DiamondNecklace,
    TREASURE_END,
    
    KEY_START = 50,
    SilverKey = 50,
    GoldenKey,
    ClosedLock,
    OpenLock,
    ClosedChest,
    OpenChest,
    KEY_END,

    LIGHT_START = 60,
    Torch = LIGHT_START,
    Candle,
    LIGHT_END,

    SCROLL_START = 70,
    Scroll1 = SCROLL_START,
    Scroll2,
    Scroll3,
    Scroll4,
    Scroll5,
    Scroll6,
    SROLL_END,

    FOOD_START = 80,
    Bread = FOOD_START,
    Meat,
    Cheese,
    FOOD_END,


    GraySpikes = 90,
    RedSpikes,
    Drawers,
    Shelf1,
    Shelf2,
    Shelf3,
}