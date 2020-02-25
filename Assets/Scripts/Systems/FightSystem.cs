using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class FightSystem : ComponentSystem
{
    float time = 1;
    List<Vector3Int> listEnemyPositions = new List<Vector3Int>();
    List<Vector3Int> listAdjacentCells =  new List<Vector3Int>();
    List<Vector3Int> listFinalPositions = new List<Vector3Int>();

    protected override void OnUpdate()
    {
        Entities.ForEach((ref Translation translation, ref TeamComponent team, ref UnitComponent unitComponent, ref FightComponent fightComponent, ref AnimationComponent animationComponent) =>
        {  //Detection
            // if overlapping
            if (team.number == 1 && !ThisCellIsFreeBis(GameHandler.instance.tilemap.WorldToCell(translation.Value)))
            {
                fightComponent.hasToMove = true;
                fightComponent.isFighting = true;
            }
            if (ThereIsAnEnemy(team.number, GameHandler.instance.tilemap.WorldToCell(translation.Value)))
            {
                fightComponent.isFighting = true;
                fightComponent.target = EnemyTarget(team.number, GameHandler.instance.tilemap.WorldToCell(translation.Value));

            }
            else if (ThereIsARangeEnemy(team.number, GameHandler.instance.tilemap.WorldToCell(translation.Value)))
            {
                fightComponent.isFighting = true;
                fightComponent.target = RangeEnemyTarget(team.number, GameHandler.instance.tilemap.WorldToCell(translation.Value));
                if (unitComponent.unitType == UnitType.KNIGHT && team.number == 1)
                {
                    fightComponent.hasToMove = true;
                }
            }
            else
            {
                fightComponent.isFighting = false;
            }

            if (fightComponent.isFighting)
            {
                if (UnitAnimation.FIGHT != animationComponent.animationType)
                {
                    animationComponent.animationType = UnitAnimation.FIGHT;
                    animationComponent.currentFrame = 0;
                    animationComponent.frameCount = UnitData.getUnitAnimationCount(unitComponent.unitType, UnitAnimation.FIGHT);
                    animationComponent.frameTimer = 0f;
                    animationComponent.frameTimerMax = 0.1f;
                }
            }
            else if (!fightComponent.isFighting)
            {
                if (UnitAnimation.FIGHT == animationComponent.animationType)
                {
                    animationComponent.animationType = UnitAnimation.IDLE;
                    animationComponent.currentFrame = 0;
                    animationComponent.frameCount = UnitData.getUnitAnimationCount(unitComponent.unitType, UnitAnimation.IDLE);
                    animationComponent.frameTimer = 0f;
                    animationComponent.frameTimerMax = 0.30f;
                }
            }
        });

        time += Time.deltaTime;
        if (time > 1)
        {
            listEnemyPositions.Clear();
            listFinalPositions.Clear();
            // Create the list of enemiy positions that have to be reached
            Entities.WithAll<UnitComponent>().ForEach((Entity entity, ref FightComponent fightComponent) =>
            {
                if (fightComponent.hasToMove == true)
                {
                    listEnemyPositions.Add(Position(fightComponent.target));
                }

            });

            // Get the list of every adjacent cell that has to be reached
            listAdjacentCells = GetAdjacentListOfCells(listEnemyPositions);

            // Get rid of useless cells
            for (int k = 0; k < listAdjacentCells.Count; k++)
            {
                if (ThisCellIsFree(listAdjacentCells[k]))
                {
                    listFinalPositions.Add(listAdjacentCells[k]);
                }
            }

            Entities.WithAll<UnitComponent>().ForEach((Entity entity, ref HealthComponent health, ref TeamComponent team, ref FightComponent fightComponent, ref Translation translation, ref UnitComponent unitComponent) =>
            {
                if (health.health <= 0)
                {
                    PostUpdateCommands.DestroyEntity(entity);
                    PostUpdateCommands.DestroyEntity(health.healthBar);
                }
                if (fightComponent.isFighting == true)
                {
                    if (fightComponent.hasToMove == false)
                    {
                        Fight(fightComponent.target);
                    }
                    else
                    {
                        if (Norme(GameHandler.instance.tilemap.WorldToCell(translation.Value), Position(fightComponent.target)) <= 1 && (Norme(GameHandler.instance.tilemap.WorldToCell(translation.Value), Position(fightComponent.target)) > 0))
                        {
                            //Then the unit is in range so it does not need to move
                            Fight(fightComponent.target);
                        }
                        else
                        {
                            if (listFinalPositions.Count > 0)
                            {
                            // The unit has to move to get in range
                            Vector3Int currentCellPosition = GameHandler.instance.tilemap.WorldToCell(translation.Value);
                            Vector3Int finalTargetCellPosition = listFinalPositions[ClosestPosition(currentCellPosition, listFinalPositions)];
                            EntityManager.AddComponentData(entity, new PathfindingParamsComponent
                            {
                                startPosition = new int2(currentCellPosition.x, currentCellPosition.y),
                                endPosition = new int2(finalTargetCellPosition.x, finalTargetCellPosition.y)
                            });
                            EntityManager.AddBuffer<PathPosition>(entity);
                            listFinalPositions.Remove(finalTargetCellPosition);
                            fightComponent.hasToMove = false;
                            }
                        }
                    }
                }
            });
            time = 0;
        }
    }

    //return the closest enemy entity
    private Entity EnemyTarget(int teamNumber , Vector3Int position)
    {
        Entity closestEnemy = new Entity();
        Entities.WithAll<TeamComponent>().ForEach((Entity entity, ref Translation translation, ref TeamComponent team) =>
        {
            Vector3Int currentCellPosition = GameHandler.instance.tilemap.WorldToCell(translation.Value);
            if (team.number != teamNumber)
            {
                if (currentCellPosition.x == position.x - 1 && currentCellPosition.y == position.y)
                {
                    closestEnemy = entity;
                }
                else if (currentCellPosition.x == position.x + 1 && currentCellPosition.y == position.y)
                {
                    closestEnemy = entity;
                }
                else if (currentCellPosition.x == position.x && currentCellPosition.y == position.y - 1)
                {
                    closestEnemy = entity;
                }
                else if (currentCellPosition.x == position.x  && currentCellPosition.y == position.y + 1)
                {
                    closestEnemy = entity;
                }

            }


        });
        return closestEnemy;
    }
    //Return true if the unit is close to an enenmy unit
    private bool ThereIsAnEnemy(int teamNumber, Vector3Int position)
    {
        bool isFighting = false;
        Entities.WithAll<TeamComponent>().ForEach((Entity entity, ref Translation translation, ref TeamComponent team) =>
        {
            Vector3Int currentCellPosition = GameHandler.instance.tilemap.WorldToCell(translation.Value);
            if (team.number != teamNumber)
            {
                if ((currentCellPosition.x == position.x - 1 && currentCellPosition.y == position.y)
                || (currentCellPosition.x == position.x + 1 && currentCellPosition.y == position.y)
                || (currentCellPosition.x == position.x && currentCellPosition.y == position.y - 1)
                || (currentCellPosition.x == position.x && currentCellPosition.y == position.y + 1))
                {
                    isFighting = true;
                }
            }

        });
        return isFighting;
    }
    //Check if there is a range enemy

    private bool ThereIsARangeEnemy(int teamNumber, Vector3Int position)
    {
        bool isFighting = false;
        Entities.WithAll<TeamComponent>().ForEach((Entity entity, ref Translation translation, ref TeamComponent team) =>
        {
            Vector3Int currentCellPosition = GameHandler.instance.tilemap.WorldToCell(translation.Value);
            if (team.number != teamNumber)
            {
                if (Norme(currentCellPosition, position) <= 3)
                {
                    isFighting = true;
                }
            }

        });
        return isFighting;
    }

    //Return the closest enemy entity at range
    private Entity RangeEnemyTarget(int teamNumber, Vector3Int position)
    {
        Entity closestEnemy = new Entity();
        float norm =3f;

        Entities.WithAll<TeamComponent>().ForEach((Entity entity, ref Translation translation, ref TeamComponent team) =>
        {
            Vector3Int currentCellPosition = GameHandler.instance.tilemap.WorldToCell(translation.Value);
            if (team.number != teamNumber)
            {
                if(Norme(currentCellPosition,position)<=norm)
                {
                    closestEnemy = entity;
                    norm = (Norme(currentCellPosition, position));
                }
            }

        });

            return closestEnemy;
    }

    // REturn the translation value of the closest enemy entity

    private Vector3Int GetPositionRangeEnemyTarget(int teamNumber, Vector3Int position)
    {
        Vector3Int enemyPosition = new Vector3Int();
        float norm = 3f;

        Entities.WithAll<TeamComponent>().ForEach((Entity entity, ref Translation translation, ref TeamComponent team) =>
        {
            Vector3Int currentCellPosition = GameHandler.instance.tilemap.WorldToCell(translation.Value);
            if (team.number != teamNumber)
            {
                if (Norme(currentCellPosition, position) <= norm)
                {
                    enemyPosition = GameHandler.instance.tilemap.WorldToCell(translation.Value);
                    norm = (Norme(currentCellPosition, position));
                }
            }

        });

        return enemyPosition;
    }


    private void Fight(Entity enemyTarget)
    {
        Entities.WithAll<FightComponent>().ForEach((Entity entity, ref HealthComponent health, ref FightComponent fightComponent) =>
        {
            if(entity == enemyTarget)
            {
                health.health -= 10;
            }
        });
    }

    private float Norme(Vector3Int vector_1, Vector3Int vector_2)
    {
        return (math.sqrt((vector_2.x - vector_1.x) * (vector_2.x - vector_1.x) + (vector_2.y - vector_1.y) * (vector_2.y - vector_1.y)));
    }

    // Return the closest free adjacent cell bewteen a unit and an enemy
    private Vector3Int FindClosestAdjacentPosition(Vector3Int currentPosition, Vector3Int enemyPosition)
    {
        bool ok = false;
        Vector3Int potentialCell = new Vector3Int();

        while (ok != true)
        {
            List<Vector3Int> potentialCells = GetAdjacentCells(enemyPosition);
            potentialCell = potentialCells[ClosestPosition(currentPosition, potentialCells)];
            if(ThisCellIsFree(potentialCell))
            {
                ok = true;
            }
            else
            {
                potentialCells.Remove(potentialCell);
            }
        }

        return potentialCell;
    }

    // Return the position of a given unit
    private Vector3Int Position(Entity unit)
    {
        Vector3Int position = new Vector3Int();
        Entities.WithAll<UnitComponent>().ForEach((Entity entity, ref Translation translation) =>
        {
            Vector3Int currentCellPosition = GameHandler.instance.tilemap.WorldToCell(translation.Value);
            if (entity == unit)
            {
                position = currentCellPosition;
            }

        });
        return position;
    }

    //return the four adjacent cells 
    private List<Vector3Int> GetAdjacentCells(Vector3Int cell)
    {
        List<Vector3Int> list = new List<Vector3Int>();
        list.Add(cell + Vector3Int.down);
        list.Add(cell + Vector3Int.right);
        list.Add(cell + Vector3Int.up);
        list.Add(cell + Vector3Int.left);
        return list;
    }
    
    // return the list of all the adjacent positions
    private List<Vector3Int> GetAdjacentListOfCells(List<Vector3Int> enemyList)
    {
        List<Vector3Int> list = new List<Vector3Int>();
        List<Vector3Int> temporaryList = new List<Vector3Int>();

        for(int k=0; k < enemyList.Count; k++)
        {
            temporaryList = GetAdjacentCells(enemyList[k]);
            for (int i=0; i<temporaryList.Count; i++)
            {
                if (list.Contains(temporaryList[i]) == false)
                {
                    list.Add(temporaryList[i]);
                }
            }
        }

        return list;

    }

    //Check if one cell is free or available
    private bool ThisCellIsFree(Vector3Int cell)
    {
        bool isFree = true;
        Entities.WithAny<BlockableEntityComponent, UnitComponent>().ForEach((Entity entity, ref Translation translation) =>
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

    // Check if one cell is occupied by an ally unit
    private bool ThisCellIsFreeBis(Vector3Int cell)
    {
        bool isFree = true;
        Entities.WithAny<UnitComponent>().ForEach((Entity entity, ref Translation translation, ref TeamComponent team) =>
        {
            Vector3Int structurCell = GameHandler.instance.tilemap.WorldToCell(translation.Value);
            if (structurCell.x == cell.x && structurCell.y == cell.y && team.number == 0)
            {
                isFree = false;
            }
        });
        return isFree;
    }

    //Return the indice of the closest potiion from a list
    private int ClosestPosition(Vector3Int currentCellPosition, List<Vector3Int> potentialPositions)
    {
        int closestPositionIndice = new int();
        float min = 100f;
        for (int k = 0; k < potentialPositions.Count; k++)
        {
            if (Norme(currentCellPosition, potentialPositions[k]) < min)
            {
                closestPositionIndice = k;
                min = Norme(currentCellPosition, potentialPositions[k]);
            }
        }
        return closestPositionIndice;
    }
}