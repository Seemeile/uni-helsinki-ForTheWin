using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.SceneManagement;

public class DoHarvestSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        if (SceneManager.GetActiveScene().name == "test")
        {
            Entities.ForEach((Entity unitEntity, ref Translation translation, ref DoHarvestComponent doHarvestComponent) =>
            {
                Vector3Int currentCellPosition = GameHandler.instance.tilemap.WorldToCell(translation.Value);
                switch (doHarvestComponent.state)
                {
                    case DoHarvestComponent.STATE.INIT:
                        Translation harvestableTranslation = EntityManager.GetComponentData<Translation>(doHarvestComponent.target);
                        Vector3Int targetCellPosition = GameHandler.instance.tilemap.WorldToCell(harvestableTranslation.Value);
                        Vector3Int freeSpaceAround = getFreeNeighborSpace(targetCellPosition);

                        PostUpdateCommands.RemoveComponent<PathfindingParamsComponent>(unitEntity);
                        PostUpdateCommands.AddComponent(unitEntity, new PathfindingParamsComponent
                        {
                            startPosition = new int2(currentCellPosition.x, currentCellPosition.y),
                            endPosition = new int2(freeSpaceAround.x, freeSpaceAround.y)
                        });
                        EntityManager.AddBuffer<PathPosition>(unitEntity);
                        doHarvestComponent.targetCell = freeSpaceAround;
                        doHarvestComponent.state = DoHarvestComponent.STATE.GO;
                        break;
                    case DoHarvestComponent.STATE.GO:
                        if (currentCellPosition.x.Equals(doHarvestComponent.targetCell.x)
                            && currentCellPosition.y.Equals(doHarvestComponent.targetCell.y))
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
                            Vector3Int hqFreeSpaceAround = getFreeNeighborSpace(hqCellPosition);

                            PostUpdateCommands.RemoveComponent<PathfindingParamsComponent>(unitEntity);
                            PostUpdateCommands.AddComponent(unitEntity, new PathfindingParamsComponent
                            {
                                startPosition = new int2(currentCellPosition.x, currentCellPosition.y),
                                endPosition = new int2(hqFreeSpaceAround.x, hqFreeSpaceAround.y)
                            });
                            EntityManager.AddBuffer<PathPosition>(unitEntity);

                            doHarvestComponent.hq = hq;
                            doHarvestComponent.hqCell = hqFreeSpaceAround;
                            doHarvestComponent.carryType = harvestableData.type;
                            doHarvestComponent.carryAmount = 50; // TODO: dynamic
                        EntityManager.SetComponentData(doHarvestComponent.target, new HarvestableComponent
                            {
                                type = harvestableData.type,
                                harvestTime = harvestableData.harvestTime,
                                ressourceAmount = harvestableData.ressourceAmount -= 50 // TODO: dynamic
                        });
                            doHarvestComponent.state = DoHarvestComponent.STATE.RETURN;
                        }
                        break;
                    case DoHarvestComponent.STATE.RETURN:
                        if (currentCellPosition.x.Equals(doHarvestComponent.hqCell.x)
                            && currentCellPosition.y.Equals(doHarvestComponent.hqCell.y))
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

    private Vector3Int getFreeNeighborSpace(Vector3Int targetCellPosition)
    {
        for (float x = targetCellPosition.x - 1; x <= targetCellPosition.x + 1; x++) 
        {
            for (float y = targetCellPosition.y - 1; y <= targetCellPosition.y + 1; y++)
            {
                bool cellIsFree = true;
                Entities.WithAll<BlockableEntityComponent>().ForEach((Entity entity, ref Translation translation) => {
                    if (translation.Value.x.Equals(x) && translation.Value.y.Equals(y)) {
                        cellIsFree = false;
                    }
                });
                if (cellIsFree) {
                    return new Vector3Int((int) x, (int) y, -2);
                }
            }
        }
        return targetCellPosition;
    }
}