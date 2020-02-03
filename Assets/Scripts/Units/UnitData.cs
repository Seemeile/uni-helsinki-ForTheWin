using System;
using System.Collections.Generic;

public class UnitData
{
    private static Dictionary<UnitType, string> unitSprites = new Dictionary<UnitType, string> {
        {UnitType.PEASANT, "skelet_idle_anim_f0" },
        {UnitType.KNIGHT, "knight_f_idle_anim_f0" },
        {UnitType.ELF, "elf_f_idle_anim_f0" },
    };

    public static string getUnitSprite(UnitType unitType)
    {
        return unitSprites[unitType];
    }
}
