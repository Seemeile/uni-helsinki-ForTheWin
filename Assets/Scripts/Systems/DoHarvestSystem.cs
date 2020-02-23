using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class DoHarvestSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity unitEntity, ref Translation translation, ref DoHarvestComponent doHarvestComponent) => 
        {
            Vector3Int currentCellPosition = GameHandler.instance.tilemap.WorldToCell(translation.Value);
            Debug.Log(doHarvestComponent.state);
            switch(doHarvestComponent.state) 
            {
                case DoHarvestComponent.STATE.INIT:
                    Translation harvestableTranslation = EntityManager.GetComponentData<Translation>(doHarvestComponent.target);
                    Vector3Int targetCellPosition = GameHandler.instance.tilemap.WorldToCell(harvestableTranslation.Value);
                    targetCellPosition.y = targetCellPosition.y + 1; // TODO: improve

                    PostUpdateCommands.RemoveComponent<PathfindingParamsComponent>(unitEntity);
                    PostUpdateCommands.AddComponent(unitEntity, new PathfindingParamsComponent
                    {
                        startPosition = new int2(currentCellPosition.x, currentCellPosition.y),
                        endPosition = new int2(targetCellPosition.x, targetCellPosition.y)
                    });                    
                    EntityManager.AddBuffer<PathPosition>(unitEntity);
                    doHarvestComponent.targetCell = targetCellPosition;
                    doHarvestComponent.state = DoHarvestComponent.STATE.GO;
                    break;
                case DoHarvestComponent.STATE.GO:
                    if (currentCellPosition.Equals(doHarvestComponent.targetCell)) 
                    {
                        doHarvestComponent.state = DoHarvestComponent.STATE.HARVEST;
                    }
                    break;
                case DoHarvestComponent.STATE.HARVEST:
                    doHarvestComponent.time += Time.deltaTime;
                    HarvestableComponent harvestableData = EntityManager.GetComponentData<HarvestableComponent>(doHarvestComponent.target);
                    if (doHarvestComponent.time >= harvestableData.harvestTime)
                    {
                        Entity hq = doHarvestComponent.target;
                        Entities.ForEach((Entity hqEntity, ref StructureComponent structureComponent) => 
                        {
                            if (BuildingType.HQ.Equals(structureComponent.type))
                            {
                                hq = hqEntity;
                            }
                        });
                        Translation hqTranslation = EntityManager.GetComponentData<Translation>(hq);
                        Vector3Int hqCellPosition = GameHandler.instance.tilemap.WorldToCell(hqTranslation.Value);
                        hqCellPosition.y = hqCellPosition.y - 1; // TODO: improve

                        PostUpdateCommands.RemoveComponent<PathfindingParamsComponent>(unitEntity);
                        PostUpdateCommands.AddComponent(unitEntity, new PathfindingParamsComponent
                        {
                            startPosition = new int2(currentCellPosition.x, currentCellPosition.y),
                            endPosition = new int2(hqCellPosition.x, hqCellPosition.y)
                        });                    
                        EntityManager.AddBuffer<PathPosition>(unitEntity);

                        doHarvestComponent.hq = hq;
                        doHarvestComponent.hqCell = hqCellPosition;
                        doHarvestComponent.carryType = harvestableData.type;
                        doHarvestComponent.carryAmount = 50; // TODO: dynamic
                        EntityManager.SetComponentData(doHarvestComponent.target, new HarvestableComponent {
                            type = harvestableData.type,
                            harvestTime = harvestableData.harvestTime,
                            ressourceAmount = harvestableData.ressourceAmount -= 50 // TODO: dynamic
                        });
                        doHarvestComponent.state = DoHarvestComponent.STATE.RETURN;
                    }
                    break;
                case DoHarvestComponent.STATE.RETURN:
                    if (currentCellPosition.Equals(doHarvestComponent.hqCell)) 
                    {
                        if (HarvestableType.GOLDMINE.Equals(doHarvestComponent.carryType))
                        {
                            UI.instance.addGold(doHarvestComponent.carryAmount);
                        } 
                        else if (HarvestableType.WOOD.Equals(doHarvestComponent.carryType))
                        {
                            UI.instance.addWood(doHarvestComponent.carryAmount);
                        }
                        PostUpdateCommands.RemoveComponent<DoHarvestComponent>(unitEntity);
                    }
                    break;
                default:
                    break;
            }
        });
    }
}