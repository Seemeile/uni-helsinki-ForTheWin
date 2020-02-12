using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class BuildUnitSystem : ComponentSystem
{
    protected override void OnUpdate() 
    {
        EntityManager entityManager = World.Active.EntityManager;

        Entities.ForEach((Entity clickedEventEntity, ref UnitSlotClickedEvent UnitSlotClickedEvent) => 
        {
            UnitSlotClickedEvent clickEvent = UnitSlotClickedEvent;
            Entities.WithAll<StructureSelectedComponent>()
                .ForEach((ref StructureComponent structureComponent, ref Translation translation) => 
            {
                UnitType[] buildableUnits = BuildingData.getBuildableUnits(structureComponent.type);
                
                int2 spawnPosition = findFreeNeighborTile(translation.Value);

                if (!spawnPosition.Equals(new int2(0, 0))) 
                {
                    UnitData.spawnUnit(buildableUnits[clickEvent.slotNumber - 1], spawnPosition.x, spawnPosition.y);
                }
            });
            entityManager.DestroyEntity(clickedEventEntity);
        });
    }

    private int2 findFreeNeighborTile(float3 buildingTranslation) 
    {
        for (int radius = 1; radius <= 5; radius++) 
        {
            for (float x = buildingTranslation.x - radius; x <= buildingTranslation.x + radius; x++)
            {
                for (float y = buildingTranslation.y - radius; y <= buildingTranslation.y + radius; y++)
                {
                    bool foundEntity = false;
                    Entities.ForEach((ref Translation translation) => 
                    {
                        if (translation.Value.x > x - 0.1f 
                            && translation.Value.x < x + 0.1f
                            && translation.Value.y < y + 0.1f
                            && translation.Value.y > y - 0.1f) 
                        {
                            foundEntity = true;
                        }
                    });
                    if (!foundEntity) {
                        return new int2((int) x, (int) y);
                    }
                }
            }
        }
        return 0;
    }
}