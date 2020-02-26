using System;
using System.Collections.Generic;

public class HarvestableData
{
    private static Dictionary<int, HarvestableType> tileToHarvestType = new Dictionary<int, HarvestableType> {
        {1, HarvestableType.WOOD},
        {103, HarvestableType.WOOD},
        {104, HarvestableType.WOOD},
        {105, HarvestableType.WOOD},
        {32, HarvestableType.GOLDMINE}
    };

    public static HarvestableType getHarvestableType(int tileNo)
    {
        return tileToHarvestType[tileNo];
    }
}
