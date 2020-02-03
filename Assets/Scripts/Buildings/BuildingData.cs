using System;
using System.Collections.Generic;

public class BuildingData
{
    private static Dictionary<int, BuildingType> typeToTileNo = new Dictionary<int, BuildingType> {
        {0, BuildingType.FARM},
        {26, BuildingType.BARRACK},
        {28, BuildingType.HQ}
    };

    private static Dictionary<BuildingType, UnitType[]> buildableUnits = new Dictionary<BuildingType, UnitType[]> {
        {BuildingType.HQ, new UnitType[] { UnitType.PEASANT }},
        {BuildingType.BARRACK, new UnitType[] { UnitType.KNIGHT, UnitType.ELF }},
        {BuildingType.FARM, new UnitType[] {}}
    };

    public static UnitType[] getBuildableUnits(BuildingType buildingType)
    {
        return buildableUnits[buildingType];
    }

    public static BuildingType getBuildingType(int tileNo)
    {
        return typeToTileNo[tileNo];
    }

    public static int getTileNumber(BuildingType buildingType)
    {
        foreach(KeyValuePair<int, BuildingType> entry in typeToTileNo)
        {
            if (buildingType.Equals(entry.Value)) 
            {
                return entry.Key;
            }
        }
        return 0;
    }
}
