using System;
using System.Collections.Generic;

public class UnitData
{
    private static Dictionary<UnitType, string> unitSprites = new Dictionary<UnitType, string> {
        {UnitType.PEASANT, "skelet_idle_anim_f0" },
        {UnitType.KNIGHT, "knight_f_idle_anim_f0" },
        {UnitType.ELF, "elf_f_idle_anim_f0" },
    };

    private static Dictionary<UnitType, string[]> unitAnimations = new Dictionary<UnitType, string[]> {
        {UnitType.PEASANT, new string[]{ "skelet_idle_anim_f0"} },
        {UnitType.KNIGHT, new string[]{ "knight_f_idle_anim_f0", "knight_f_idle_anim_f1", "knight_f_idle_anim_f2", "knight_f_idle_anim_f3"} },
        {UnitType.ELF, new string[]{ "elf_f_idle_anim_f0"} },
    };

    public static string getUnitAnimation(UnitType unitType, int animationIndex)
    {
        return unitAnimations[unitType][animationIndex];
    }

    public static string getUnitSprite(UnitType unitType)
    {
        return unitSprites[unitType];
    }
}
