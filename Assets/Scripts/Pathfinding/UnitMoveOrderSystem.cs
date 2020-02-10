using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

public class UnitMoveOrderSystem : ComponentSystem
{
    private float3 startPosition;
    private float3 endPosition;
    private float3 selectionAreaSize;
    private float3 currentMousePosition;
    private Vector3Int currentMouseCell;
    private float3 lowerLeftPosition;
    private float3 upperRightPosition;


    protected override void OnUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            handleLeftMousePressed();
        }
        if (Input.GetMouseButton(0))
        {
            handleLeftMouseHeldDown();
        }
        if (Input.GetMouseButtonUp(0))
        {
            handleLeftMouseReleased();
        }
        if (Input.GetMouseButtonDown(1))
        {
            handleRightMousePressed();
        }
    }

    private void handleLeftMousePressed()
    {
        GameHandler.instance.selectionAreaTransform.gameObject.SetActive(true);
        currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        startPosition.x = currentMousePosition.x;
        startPosition.y = currentMousePosition.y;
        GameHandler.instance.selectionAreaTransform.position = startPosition;
    }

    private void handleLeftMouseHeldDown()
    {
        currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        selectionAreaSize.x = currentMousePosition.x - startPosition.x;
        selectionAreaSize.y = currentMousePosition.y - startPosition.y;
        GameHandler.instance.selectionAreaTransform.localScale = selectionAreaSize;
    }

    private void handleLeftMouseReleased()
    {
        // Deselect all previous selected entities
        Entities.WithAll<EntitySelectedComponent>().ForEach((Entity entity) =>
        {
            PostUpdateCommands.RemoveComponent<EntitySelectedComponent>(entity);

            if (EntityManager.HasComponent<StructureSelectedComponent>(entity))
            {
                PostUpdateCommands.RemoveComponent<StructureSelectedComponent>(entity);
                UI.instance.hideStructureOverlay();
            }
        });

        GameHandler.instance.selectionAreaTransform.gameObject.SetActive(false);
        currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        endPosition.x = currentMousePosition.x;
        endPosition.y = currentMousePosition.y;

        // Entities detection
        float3 lowerLeftPosition = new float3(math.min(startPosition.x, endPosition.x), math.min(startPosition.y, endPosition.y), 0);
        float3 upperRightPosition = new float3(math.max(startPosition.x, endPosition.x), math.max(startPosition.y, endPosition.y), 0);
        bool selectOnlyOneEntity = false;
        float selectionAreaMinSize = 1f;
        float smallSelectionAreaSize = math.distance(lowerLeftPosition, upperRightPosition);

        if (smallSelectionAreaSize < selectionAreaMinSize)
        {
            //Selection too small, meaning if there is only one click
            lowerLeftPosition += new float3(-1f, -1f, 0f) * (selectionAreaMinSize - selectionAreaSize) * .5f;
            upperRightPosition += new float3(+1f, +1f, 0f) * (selectionAreaMinSize - selectionAreaSize) * .5f;
            selectOnlyOneEntity = true;
        }
        //Selection
        int selectEntityCount = 0;
        Entities.ForEach((Entity entity, ref Translation translation) =>
        {
            if (selectOnlyOneEntity == false || selectEntityCount < 1)
            {
                float3 entityPosition = translation.Value;

                if (entityPosition.x >= lowerLeftPosition.x &&
                    entityPosition.y >= lowerLeftPosition.y &&
                    entityPosition.x <= upperRightPosition.x &&
                    entityPosition.y <= upperRightPosition.y)
                {
                    //Entity inside the selection area
                    PostUpdateCommands.AddComponent(entity, new EntitySelectedComponent());
                    selectEntityCount++;
                }
            }
        });
    }

    private void handleRightMousePressed()
    {
        //Right mouse button down
        float3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int targetCellPosition = GameHandler.instance.tilemap.WorldToCell(targetPosition);
        Vector3Int finalTargetCellPosition=Vector3Int.zero;

        //if the target is a ressource, then only one entity will go to harvest it
        if (IsHarvestable(targetCellPosition))
        {

            int entityCount = 0;
            Entities.WithAll<EntitySelectedComponent, UnitComponent>().ForEach((Entity entity, ref Translation translation) =>
            {
                Vector3Int currentCellPosition = GameHandler.instance.tilemap.WorldToCell(translation.Value);
                if (entityCount ==0)
                {
                    finalTargetCellPosition.x = targetCellPosition.x;
                    finalTargetCellPosition.y = targetCellPosition.y +1;

                    EntityManager.AddComponentData(entity, new PathfindingParamsComponent
                    {
                        startPosition = new int2(currentCellPosition.x, currentCellPosition.y),
                        endPosition = new int2(finalTargetCellPosition.x, finalTargetCellPosition.y)
                    });
                    EntityManager.AddBuffer<PathPosition>(entity);
                    //The units start harvesting and becomes unselected
                    PostUpdateCommands.RemoveComponent<EntitySelectedComponent>(entity);
                }
                
                entityCount++;

            });
        }
        //If the target is an enemy
        if (IsAnEnemy(targetCellPosition))
        {
            int index = 1;
            List<Vector3Int> positionListAround = GetListOfAdjacentCells(targetCellPosition);
            int entityCount = 0;
            Debug.Log(positionListAround.Count);
            Debug.Log("celltarget " + targetCellPosition);
            Debug.Log("positionlistAround " + positionListAround[1]);
            Debug.Log("positionlistAround " + positionListAround[2]);
            Debug.Log("positionlistAround " + positionListAround[3]);
            Debug.Log("positionlistAround " + positionListAround[4]);
            Debug.Log("positionlistAround " + positionListAround[5]);
            Debug.Log("positionlistAround " + positionListAround[6]);
            Debug.Log("positionlistAround " + positionListAround[7]);
            Debug.Log("positionlistAround " + positionListAround[8]);
            Debug.Log("positionlistAround " + positionListAround[9]);



            Entities.WithAll<EntitySelectedComponent, UnitComponent>().ForEach((Entity entity, ref Translation translation) =>
            {
                if (entityCount <positionListAround.Count-1)
                { 
                    Vector3Int currentCellPosition = GameHandler.instance.tilemap.WorldToCell(translation.Value);
                    finalTargetCellPosition = positionListAround[index];

                    EntityManager.AddComponentData(entity, new PathfindingParamsComponent
                    {
                        startPosition = new int2(currentCellPosition.x, currentCellPosition.y),
                        endPosition = new int2(finalTargetCellPosition.x, finalTargetCellPosition.y)
                    });
                    EntityManager.AddBuffer<PathPosition>(entity);
                    index++;
                }
                entityCount++;
            });
        }
        else
        {
            List<Vector3Int> positionListAround = GetPositionListAround(targetCellPosition, armySize());
            int index = 0;

            Entities.WithAll<EntitySelectedComponent, UnitComponent>().ForEach((Entity entity, ref Translation translation) =>
            {
                Vector3Int currentCellPosition = GameHandler.instance.tilemap.WorldToCell(translation.Value);
                finalTargetCellPosition = positionListAround[index];

                EntityManager.AddComponentData(entity, new PathfindingParamsComponent
                {
                    startPosition = new int2(currentCellPosition.x, currentCellPosition.y),
                    endPosition = new int2(finalTargetCellPosition.x, finalTargetCellPosition.y)
                });
                EntityManager.AddBuffer<PathPosition>(entity);
                index++;
            });


        }

    }


    // Return the list of possible cells around the target cell
    private List<Vector3Int> GetPositionListAround(Vector3Int startPosition, int positionCount)
    {
        List<Vector3Int> listPosition = new List<Vector3Int>();
        List<Vector3Int> listPositionFinal = new List<Vector3Int>();
        listPosition.Add(startPosition);
        listPositionFinal.Add(startPosition);
        int givenPositionCount = 1;
        int count = 1;
        int intermediaryCount = 0;
        Vector3Int dir = Vector3Int.down;
        Vector3Int potentialCell;
        while (givenPositionCount < positionCount)
        {
            if (intermediaryCount == 2)
            {
                count++;
                intermediaryCount = 0;
            }
            dir = NewDir(dir);
            for (int k = 0; k < count; k++)
            {
                potentialCell = listPosition[listPosition.Count - 1] + dir;
                listPosition.Add(potentialCell);
                if (ThisCellIsFree(potentialCell))
                {
                    listPositionFinal.Add(potentialCell);
                    givenPositionCount++;
                }
            }
            intermediaryCount++;
        }

        return listPositionFinal;
    }

    //Return the next direction
    private Vector3Int NewDir(Vector3Int dir)
    {
        Vector3Int futurDir = Vector3Int.zero;

        if (dir == Vector3Int.up)
        {
            futurDir = Vector3Int.left;
        }
        if (dir == Vector3Int.left)
        {
            futurDir = Vector3Int.down;
        }
        if (dir == Vector3Int.down)
        {
            futurDir = Vector3Int.right;
        }
        if (dir == Vector3Int.right)
        {
            futurDir = Vector3Int.up;
        }
        return futurDir;
    }

    //Check if one cell is free or available

    private bool ThisCellIsFree(Vector3Int cell)
    {
        bool isFree = true;

        Entities.WithAll<Translation>().ForEach((Entity entity, ref Translation translation) =>
        {
            Vector3Int structurCell = GameHandler.instance.tilemap.WorldToCell(translation.Value);
            if (structurCell.x == cell.x && structurCell.y == cell.y)
            {
                isFree = false;
            }
        });
        if (cell.x < -9 || cell.x > 10 || cell.y < -7 || cell.y > 6)
        {
            isFree = false;
        }

        return isFree;
    }
    //return the number of selected entities
    private int armySize()
    {
        int number = 0;
        Entities.WithAll<EntitySelectedComponent, UnitComponent>().ForEach((Entity entity) =>
        {
            number++;
        });
        return number;
    }

    //Check if the mouse is above an harvestable entity
    private bool IsHarvestable(Vector3Int currentMouseCell)
    {
        bool harvestable = false;
        Vector3Int entityCell;
        Entities.WithAll<HarvestableComponent>().ForEach((Entity entity, ref Translation translation) =>
        {
            entityCell = GameHandler.instance.tilemap.WorldToCell(translation.Value);

            if (entityCell.x == currentMouseCell.x && entityCell.y == currentMouseCell.y)
            {
                harvestable = true;
            }
        });
        return harvestable;
    }

    //Check if the mouse is above an enemy entity

    private bool IsAnEnemy(Vector3Int currentMouseCell)
    {
        bool enemy = false;
        Vector3Int entityCell;
        Entities.WithAll<UnitComponent>().ForEach((Entity entity, ref Translation translation) =>
        {
            entityCell = GameHandler.instance.tilemap.WorldToCell(translation.Value);

            if (entityCell.x == currentMouseCell.x && entityCell.y == currentMouseCell.y)
            {
                enemy = true;
            }
        });
        return enemy;
    }

    //Return le list of free adjacent cells around one position
    private List<Vector3Int> GetListOfAdjacentCells(Vector3Int targetCell)
    {
        List<Vector3Int> listPosition = new List<Vector3Int>();
        Vector3Int dir = Vector3Int.down;
        Vector3Int potentialCell;
        listPosition.Add(targetCell);
        int intermediaryCount = 1;
        int triedPosition = 0;
        int count = 0;

        while (triedPosition < 8)
        {
            if (intermediaryCount == 2)
            {
                count++;
                intermediaryCount = 0;
            }
            dir = NewDir(dir);
            for (int k = 0; k < count; k++)
            {
                potentialCell = listPosition[listPosition.Count - 1] + dir;

                if (ThisCellIsFree(potentialCell))
                {
                    listPosition.Add(potentialCell);
                }
                triedPosition++;
            }
            intermediaryCount++;
        }
            return listPosition;
    }
}