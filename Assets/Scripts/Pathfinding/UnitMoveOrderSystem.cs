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
    //public int[] tilemapCellBoundsX = GameHandler.instance.tilemapCellBoundsX;
    //public int[] tilemapCellBoundsY = GameHandler.instance.tilemapCellBoundsY;


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
        // Deselect all previous selected entities oly if the key ctrl is not pressed
        if (!Input.GetKey(KeyCode.LeftShift))
        { 
            Entities.WithAll<EntitySelectedComponent>().ForEach((Entity entity) =>
            {
                PostUpdateCommands.RemoveComponent<EntitySelectedComponent>(entity);

                if (EntityManager.HasComponent<StructureSelectedComponent>(entity))
                {
                    PostUpdateCommands.RemoveComponent<StructureSelectedComponent>(entity);
                    UI.instance.hideStructureOverlay();
                }
            });
        }

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
        //Selection of entities
        int selectEntityCount = 0;
        Entities.WithAll<TeamComponent>().ForEach((Entity entity, ref Translation translation, ref TeamComponent team) =>
        {

            if (selectOnlyOneEntity == false || selectEntityCount < 1)
            {
                float3 entityPosition = translation.Value;

                if (entityPosition.x >= lowerLeftPosition.x &&
                    entityPosition.y >= lowerLeftPosition.y &&
                    entityPosition.x <= upperRightPosition.x &&
                    entityPosition.y <= upperRightPosition.y &&
                    team.number !=1 )
                {
                    //Entity inside the selection area
                    PostUpdateCommands.AddComponent(entity, new EntitySelectedComponent());
                    selectEntityCount++;
                }
            }
        });

        Entities.WithAny<StructureComponent, HarvestableComponent>().ForEach((Entity entity, ref Translation translation) =>
        {
            if (selectOnlyOneEntity == false || selectEntityCount < 1)
            {
                float3 entityPosition = translation.Value;

                if (entityPosition.x >= lowerLeftPosition.x &&
                    entityPosition.y >= lowerLeftPosition.y &&
                    entityPosition.x <= upperRightPosition.x &&
                    entityPosition.y <= upperRightPosition.y )
                {
                    //Entity inside the selection area
                    PostUpdateCommands.AddComponent(entity, new EntitySelectedComponent());
                    selectEntityCount++;
                }
            }
        });


    }

    //Right mouse button down
    private void handleRightMousePressed()
    {
        GameHandler.instance.selectionAreaTransform.gameObject.SetActive(false);
        currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentMouseCell = GameHandler.instance.tilemap.WorldToCell(currentMousePosition);

        //First deselect all the building entities
        Entities.WithAll<StructureSelectedComponent>().ForEach((Entity entity) =>
        {
            PostUpdateCommands.RemoveComponent<EntitySelectedComponent>(entity);
        });
        
        float3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int targetCellPosition = GameHandler.instance.tilemap.WorldToCell(targetPosition);
        Vector3Int finalTargetCellPosition=Vector3Int.zero;

        //if the target is a ressource
        Entities.WithAll<HarvestableComponent>().ForEach((Entity harvestableEntity, ref Translation translation) =>
        {
            Vector3Int harvestableCell = GameHandler.instance.tilemap.WorldToCell(translation.Value);
            if (harvestableCell.x == currentMouseCell.x && harvestableCell.y == currentMouseCell.y)
            {
                int entityCount = 0;
                Entities.WithAll<EntitySelectedComponent>().ForEach((Entity unitEntity, ref UnitComponent unitComponent) =>
                {
                    //only go harvest if the unit is a peasant
                    if (UnitType.PEASANT == unitComponent.unitType && entityCount == 0)
                    {
                        EntityManager.RemoveComponent<DoHarvestComponent>(unitEntity);
                        EntityManager.AddComponentData(unitEntity, new DoHarvestComponent {
                            target = harvestableEntity,
                            state = DoHarvestComponent.STATE.INIT,
                            time = 0f
                        });
                        entityCount++;
                    }
                });
            }
        });

        if (IsAnAlly(targetCellPosition)) //If the target is an ally : do nothing
        {
            
        }
        else if (IsAnEnemy(targetCellPosition)) //If the target is an enemy
        {
            int nbrPositionAdj = 0;
            int nbrPositionAdjRange = 0;
            List<Vector3Int> positionListAround = GetListOfUnits(targetCellPosition); //the list of every enemy unit close to the target
            List<Vector3Int> positionAdjaccent = GetListOfAdjacentCells(positionListAround);// the list of every adjacent cell to these enemies
            List<Vector3Int> positionAdjaccent_two = GetListOfAdjacentCells(positionAdjaccent); // the list of the first line cells ( for the elfs)
            List<Vector3Int> positionAdjaccent_three = GetListOfAdjacentCells(positionAdjaccent_two); // the list of the second line cells ( for the elfs)
            List<Vector3Int> positionFinal = new List<Vector3Int>();
            int closeRangeCount = 0;
            int longRangeCount = 0;
   ;

            // Keep only the free cells
            for (int k =0; k < positionAdjaccent.Count ; k++)
            {
                if (ThisCellIsFree(positionAdjaccent[k]))
                {
                    positionFinal.Add(positionAdjaccent[k]);
                    nbrPositionAdj++;
                }
            }
            for (int k = 0; k < positionAdjaccent_two.Count; k++)
            {
                if (ThisCellIsFree(positionAdjaccent_two[k]))
                {
                    positionFinal.Add(positionAdjaccent_two[k]);
                    nbrPositionAdjRange++;
                }
            }
            for (int k = 0; k < positionAdjaccent_three.Count; k++)
            {
                if (ThisCellIsFree(positionAdjaccent_three[k]))
                {
                    positionFinal.Add(positionAdjaccent_three[k]);
                    nbrPositionAdjRange++;
                }
            }


             Entities.WithAll<EntitySelectedComponent, UnitComponent>().ForEach((Entity entity, ref Translation translation, ref UnitComponent unitComponent) =>
             {
                 //If the unit is a close range fighter
                 if (unitComponent.unitType == UnitType.KNIGHT)
                 {

                     if (closeRangeCount < nbrPositionAdj)
                     {
                         Vector3Int currentCellPosition = GameHandler.instance.tilemap.WorldToCell(translation.Value);
                         finalTargetCellPosition = positionFinal[closeRangeCount];
                         EntityManager.AddComponentData(entity, new PathfindingParamsComponent
                         {
                             startPosition = new int2(currentCellPosition.x, currentCellPosition.y),
                             endPosition = new int2(finalTargetCellPosition.x, finalTargetCellPosition.y)
                         });
                         EntityManager.AddBuffer<PathPosition>(entity);
                         PostUpdateCommands.AddComponent(entity, new FightComponent());
                         closeRangeCount++;

                     }
                 }

                 //If the unit is a long range fighter
                 else if (unitComponent.unitType == UnitType.ELF)
                 {

                     if (longRangeCount < nbrPositionAdj + nbrPositionAdjRange)
                     {
                         Vector3Int currentCellPosition = GameHandler.instance.tilemap.WorldToCell(translation.Value);
                         finalTargetCellPosition = positionFinal[longRangeCount + nbrPositionAdj];
                         EntityManager.AddComponentData(entity, new PathfindingParamsComponent
                         {
                             startPosition = new int2(currentCellPosition.x, currentCellPosition.y),
                             endPosition = new int2(finalTargetCellPosition.x, finalTargetCellPosition.y)
                         });
                         EntityManager.AddBuffer<PathPosition>(entity);
                         PostUpdateCommands.AddComponent(entity, new FightComponent());
                         longRangeCount++;

                     }
                 }
                 
                 //Unselect all the entities who cannot fight
                 else
                 {
                     PostUpdateCommands.RemoveComponent<EntitySelectedComponent>(entity);
                 }

             });
        }
        else 
        {
            // the target in a free cell
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

        Entities.WithAll<BlockableEntityComponent>().ForEach((Entity entity, ref Translation translation) =>
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
        Entities.WithAll<TeamComponent>().ForEach((Entity entity, ref Translation translation, ref TeamComponent team) =>
        {
            entityCell = GameHandler.instance.tilemap.WorldToCell(translation.Value);

            if (entityCell.x == currentMouseCell.x && entityCell.y == currentMouseCell.y && team.number ==1)
            {
                enemy = true;
            }
        });
        return enemy;
    }
    // Check is TerrainHeightmapSyncControl mouse is above an ally entity

    private bool IsAnAlly(Vector3Int currentMouseCell)
    {
        bool ally = false;
        Vector3Int entityCell;
        Entities.WithAll<TeamComponent>().ForEach((Entity entity, ref Translation translation, ref TeamComponent team) =>
        {
            entityCell = GameHandler.instance.tilemap.WorldToCell(translation.Value);

            if (entityCell.x == currentMouseCell.x && entityCell.y == currentMouseCell.y && team.number == 0)
            {
                ally = true;
            }
        });
        return ally;
    }

    //Return le list of free adjacent cells around one position
    private List<Vector3Int> GetListOfAdjacentCells(List<Vector3Int> listEnemy)
    {
        
        List<Vector3Int> listPosition = new List<Vector3Int>();
        List<int> listExtremum = GetListOfExtremum(listEnemy);
        List<Vector3Int> potentialCells = new List<Vector3Int>();
        potentialCells.Add(listEnemy[0]);
        potentialCells.Add(listEnemy[0]);
        potentialCells.Add(listEnemy[0]);

        for (int k=0; k<listEnemy.Count;k++)
        {
            
            if (listEnemy[k].x == listExtremum[0])
            {
                potentialCells[0] = new Vector3Int(listEnemy[k].x - 1, listEnemy[k].y, -20);
                potentialCells[1] = new Vector3Int(listEnemy[k].x , listEnemy[k].y + 1, -20);
                potentialCells[2] = new Vector3Int(listEnemy[k].x, listEnemy[k].y - 1, -20);
                if(listEnemy.Contains(potentialCells[0]) == false && listPosition.Contains(potentialCells[0]) == false)
                {
                    listPosition.Add(potentialCells[0]);
                }
                if (listEnemy.Contains(potentialCells[1]) == false && listPosition.Contains(potentialCells[1]) == false)
                {
                    listPosition.Add(potentialCells[1]);
                }
                if (listEnemy.Contains(potentialCells[2]) == false && listPosition.Contains(potentialCells[2]) == false)
                {
                    listPosition.Add(potentialCells[2]);
                }

            }
            if (listEnemy[k].x == listExtremum[1])
            {
                potentialCells[0] = new Vector3Int(listEnemy[k].x + 1, listEnemy[k].y, -20);
                potentialCells[1] = new Vector3Int(listEnemy[k].x, listEnemy[k].y + 1, -20);
                potentialCells[2] = new Vector3Int(listEnemy[k].x, listEnemy[k].y - 1, -20);
                if (listEnemy.Contains(potentialCells[0]) == false && listPosition.Contains(potentialCells[0]) == false)
                {
                    listPosition.Add(potentialCells[0]);
                }
                if (listEnemy.Contains(potentialCells[1]) == false && listPosition.Contains(potentialCells[1]) == false)
                {
                    listPosition.Add(potentialCells[1]);
                }
                if (listEnemy.Contains(potentialCells[2]) == false && listPosition.Contains(potentialCells[2]) == false)
                {
                    listPosition.Add(potentialCells[2]);
                }
            }
            if (listEnemy[k].y == listExtremum[2] )
            {
                potentialCells[0] = new Vector3Int(listEnemy[k].x , listEnemy[k].y - 1, -20);
                potentialCells[1] = new Vector3Int(listEnemy[k].x - 1, listEnemy[k].y , -20);
                potentialCells[2] = new Vector3Int(listEnemy[k].x + 1, listEnemy[k].y, -20);
                if (listEnemy.Contains(potentialCells[0]) == false && listPosition.Contains(potentialCells[0]) == false)
                {
                    listPosition.Add(potentialCells[0]);
                }
                if (listEnemy.Contains(potentialCells[1]) == false && listPosition.Contains(potentialCells[1]) == false)
                {
                    listPosition.Add(potentialCells[1]);
                }
                if (listEnemy.Contains(potentialCells[2]) == false && listPosition.Contains(potentialCells[2]) == false)
                {
                    listPosition.Add(potentialCells[2]);
                }
            }
            if (listEnemy[k].y == listExtremum[3])
            {
                potentialCells[0] = new Vector3Int(listEnemy[k].x, listEnemy[k].y + 1, -20);
                potentialCells[1] = new Vector3Int(listEnemy[k].x - 1, listEnemy[k].y, -20);
                potentialCells[2] = new Vector3Int(listEnemy[k].x + 1, listEnemy[k].y, -20);
                if (listEnemy.Contains(potentialCells[0]) == false && listPosition.Contains(potentialCells[0]) == false)
                {
                    listPosition.Add(potentialCells[0]);
                }
                if (listEnemy.Contains(potentialCells[1]) == false && listPosition.Contains(potentialCells[1]) == false)
                {
                    listPosition.Add(potentialCells[1]);
                }
                if (listEnemy.Contains(potentialCells[2]) == false && listPosition.Contains(potentialCells[2]) == false)
                {
                    listPosition.Add(potentialCells[2]);
                }
            }
        }
        
            return listPosition;
    }

    // Return the list of every units next to the targetcell
    private List<Vector3Int> GetListOfUnits(Vector3Int targetCell)
    {
        List<Vector3Int> listPosition = new List<Vector3Int>();
        Vector3Int dir = Vector3Int.down;
        Vector3Int potentialCell = targetCell;
        listPosition.Add(targetCell);
        int intermediaryCount = 0;
        int triedPosition = 1;
        int count = 1;

        while (triedPosition < 26)
        {
            if (intermediaryCount == 2)
            {
                count++;
                intermediaryCount = 0;
            }
            dir = NewDir(dir);
            for (int k = 0; k < count; k++)
            {
                potentialCell = potentialCell + dir;
                triedPosition++;
                if (IsAnEnemy(potentialCell))
                {
                    listPosition.Add(potentialCell);
                }
                
            }
            intermediaryCount++;
        }

        return listPosition;

    }

    //Return the maximum and minimum values of x and y cells
    private List<int> GetListOfExtremum(List<Vector3Int> listPosition)
    {
        List<int> listExtremum = new List<int> { listPosition[0].x, listPosition[0].x, listPosition[0].y, listPosition[0].y };
        for (int k=0; k<listPosition.Count;k++)
        {
            if (listPosition[k].x < listExtremum[0])
            {
                listExtremum[0] = listPosition[k].x;
            }
            if (listPosition[k].x > listExtremum[1])
            {
                listExtremum[1] = listPosition[k].x;
            }
            if (listPosition[k].y < listExtremum[2])
            {
                listExtremum[2] = listPosition[k].y;
            }
            if (listPosition[k].y > listExtremum[3])
            {
                listExtremum[3] = listPosition[k].y;
            }
        }
        return listExtremum;
    }

    //Return the list of range cells around a target (3 cells range)

    private List<Vector3Int> GetListOfRangeCells(List<Vector3Int> listEnemy)
    {
        List<Vector3Int> listPosition = new List<Vector3Int>();
        List<int> listExtremum = GetListOfExtremum(listEnemy);
        List<Vector3Int> potentialCells = new List<Vector3Int>();

        potentialCells.Add(listEnemy[0]);
        potentialCells.Add(listEnemy[0]);
        potentialCells.Add(listEnemy[0]);
        potentialCells.Add(listEnemy[0]);

        for (int k = 0; k < listEnemy.Count; k++)
        {
            potentialCells[0] = new Vector3Int(listEnemy[k].x - 1, listEnemy[k].y, -20);
            potentialCells[1] = new Vector3Int(listEnemy[k].x + 1, listEnemy[k].y, -20);
            potentialCells[2] = new Vector3Int(listEnemy[k].x, listEnemy[k].y - 1, -20);
            potentialCells[3] = new Vector3Int(listEnemy[k].x, listEnemy[k].y + 1, -20);

            if (listEnemy[k].x == listExtremum[0] && listEnemy.Contains(potentialCells[0]) == false)
            {
                listPosition.Add(potentialCells[0]);
            }
            if (listEnemy[k].x == listExtremum[1] && listEnemy.Contains(potentialCells[1]) == false)
            {
                listPosition.Add(potentialCells[1]);
            }
            if (listEnemy[k].y == listExtremum[2] && listEnemy.Contains(potentialCells[2]) == false)
            {
                listPosition.Add(potentialCells[2]);
            }
            if (listEnemy[k].y == listExtremum[3] && listEnemy.Contains(potentialCells[3]) == false)
            {
                listPosition.Add(potentialCells[3]);
            }
        }

        return listPosition;
    }

}