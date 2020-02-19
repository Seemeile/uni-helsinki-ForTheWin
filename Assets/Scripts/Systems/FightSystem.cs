using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using System;


public class FightSystem : ComponentSystem
{
    float time = 1;

    protected override void OnUpdate()
    {
        Entities.WithAll<FightComponent, UnitComponent>().ForEach((Entity entity, ref Translation translation, ref TeamComponent team, ref UnitComponent unitComponent, ref FightComponent fightComponent) =>
        {
            if (unitComponent.unitType == UnitType.KNIGHT)
            {

                if (ThereIsAnEnemy(team.number, GameHandler.instance.tilemap.WorldToCell(translation.Value)))
                {
                    fightComponent.isFighting = true;
                    fightComponent.target = EnemyTarget(team.number, GameHandler.instance.tilemap.WorldToCell(translation.Value));

                }
                else
                {
                    fightComponent.isFighting = false;
                }
            }
            else if (unitComponent.unitType == UnitType.ELF)
            {
                if (ThereIsARangeEnemy(team.number, GameHandler.instance.tilemap.WorldToCell(translation.Value)))
                {
                    fightComponent.isFighting = true;
                    fightComponent.target = RangeEnemyTarget(team.number, GameHandler.instance.tilemap.WorldToCell(translation.Value));
                }
                else
                {
                    fightComponent.isFighting = false;
                }
            }
        });

        time += Time.deltaTime;
        if (time > 1)
        {
            Entities.WithAll<FightComponent>().ForEach((Entity entity, ref HealthComponent health, ref TeamComponent team, ref FightComponent fightComponent) =>
            {

                if (health.health <= 0)
                {
                    PostUpdateCommands.DestroyEntity(entity);
                }
                if (fightComponent.isFighting == true)
                {
                    Fight(fightComponent.target);           
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

    private void Fight(Entity enemyTarget)
    {
        Entities.WithAll<FightComponent>().ForEach((Entity entity, ref HealthComponent health) =>
        {
            if(entity == enemyTarget)
            {
                health.health -= 10;
            }

        });
    }

    public float Norme(Vector3Int vector_1, Vector3Int vector_2)
    {
        return (math.sqrt((vector_2.x - vector_1.x) * (vector_2.x - vector_1.x) + (vector_2.y - vector_1.y) * (vector_2.y - vector_1.y)));
    }

   
}