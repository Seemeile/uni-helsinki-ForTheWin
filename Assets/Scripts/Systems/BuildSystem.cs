using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class BuildSystem : ComponentSystem
{
    protected override void OnUpdate() 
    {
        EntityManager entityManager = World.Active.EntityManager;

        Entities.ForEach((Entity clickedEventEntity, ref SlotClickedEvent UnitSlotClickedEvent) => 
        {
            SlotClickedEvent clickEvent = UnitSlotClickedEvent;
            Entities.WithAll<EntitySelectedComponent>().ForEach((Entity contextEntity, ref Translation translation) => 
            {
                if (entityManager.HasComponent<StructureComponent>(contextEntity)) 
                {
                    StructureComponent structureComponent = entityManager.GetComponentData<StructureComponent>(contextEntity);
                    UnitType[] buildableUnits = BuildingData.getBuildableUnits(structureComponent.type);
                    int2 spawnPosition = findFreeNeighborTile(translation.Value);
                    if (!spawnPosition.Equals(new int2(0, 0))) 
                    {
                        buildUnitWhenPossible(buildableUnits[clickEvent.slotNumber - 1], spawnPosition.x, spawnPosition.y);
                    }
                } 
                else if (entityManager.HasComponent<UnitComponent>(contextEntity))
                {
                    UnitComponent unitComponent = entityManager.GetComponentData<UnitComponent>(contextEntity);
                    BuildingType[] buildableStructures = UnitData.getUnitActions(unitComponent.unitType);
                    Entity chooseToPlaceEntity = entityManager.CreateEntity(typeof(ChooseToPlaceComponent));
                    entityManager.SetComponentData(chooseToPlaceEntity, new ChooseToPlaceComponent {
                        buildingType = buildableStructures[clickEvent.slotNumber - 1]
                    });
                }
            });
            entityManager.DestroyEntity(clickedEventEntity);
        });
    }

    private void buildUnitWhenPossible(UnitType unitType, int gridPosX, int gridPosY)
    {
        int[] costs = UnitData.getUnitCosts(unitType);
        if (UI.instance.getGoldAmount() >= costs[0] && UI.instance.getWoodAmount() >= costs[1])
        {
            UI.instance.subGold(costs[0]);
            UI.instance.subWood(costs[1]);
            UnitData.spawnUnit(unitType, gridPosX, gridPosY);
        } 
        else 
        {
            Debug.Log("not enough resources");
        }
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