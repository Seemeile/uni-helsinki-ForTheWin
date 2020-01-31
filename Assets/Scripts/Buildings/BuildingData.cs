using System;
using System.Collections.Generic;

public class BuildingData
{
    private static Dictionary<int, string[]> buildableUnits = new Dictionary<int, string[]> {
        {28, new string[] { "skelet_idle_anim_f0" }}, // HQ
        {26, new string[] { "knight_f_idle_anim_f0", "elf_f_idle_anim_f0" }}, // barracks
        {0, new string[] {}} // farm
    };
    
    public static string[] getBuildableUnits(int buildingNumber)
    {
        return buildableUnits[buildingNumber];
    }
}
