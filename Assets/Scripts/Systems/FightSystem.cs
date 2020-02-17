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
    protected override void OnUpdate()
    {
        Entities.WithAll<TeamComponent>().ForEach((Entity entity, ref Translation translation, ref TeamComponent team) =>
        {
            if (ThereIsAnEnemy(team.number, GameHandler.instance.tilemap.WorldToCell(translation.Value)))
            {
                PostUpdateCommands.AddComponent(entity, new FightComponent());
            }
            else
            {
                PostUpdateCommands.RemoveComponent<FightComponent>(entity);
            }
        });

        Entities.WithAll<FightComponent>().ForEach((Entity entity, ref Translation translation, ref HealthComponent health, ref TeamComponent team) =>
        {
            Fight(GameHandler.instance.tilemap.WorldToCell(translation.Value),EnemyTarget(team.number, GameHandler.instance.tilemap.WorldToCell(translation.Value)));
        });
    }

    //return the position of the closest enemy
    private Vector3Int EnemyTarget(int teamNumber , Vector3Int position)
    {
        Vector3Int positionEnemy = new Vector3Int();
        Entities.WithAll<TeamComponent>().ForEach((Entity entity, ref Translation translation, ref TeamComponent team) =>
        {
            Vector3Int currentCellPosition = GameHandler.instance.tilemap.WorldToCell(translation.Value);
            if (team.number != teamNumber)
            {
                if (currentCellPosition.x == position.x - 1 && currentCellPosition.y == position.y)
                {
                    positionEnemy = currentCellPosition;
                }
                else if (currentCellPosition.x == position.x + 1 && currentCellPosition.y == position.y)
                {
                    positionEnemy = currentCellPosition;
                }
                else if (currentCellPosition.x == position.x && currentCellPosition.y == position.y - 1)
                {
                    positionEnemy = currentCellPosition;
                }
                else if (currentCellPosition.x == position.x  && currentCellPosition.y == position.y + 1)
                {
                    positionEnemy = currentCellPosition;
                }

            }


        });
        return positionEnemy;
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

    private void Fight(Vector3Int unitPosition , Vector3Int enemyPosition)
    {
        Entities.WithAll<FightComponent>().ForEach((Entity entity, ref Translation translation, ref HealthComponent health, ref TeamComponent team) =>
        {
            Vector3Int currentCellPosition = GameHandler.instance.tilemap.WorldToCell(translation.Value);
            if (enemyPosition.x == currentCellPosition.x && enemyPosition.y == currentCellPosition.y)
            {
                health.health -= 10;
            }
            if (unitPosition.x == currentCellPosition.x && unitPosition.y == currentCellPosition.y)
            {
                health.health -= 10;
            }
        });
        GameHandler.instance.Dps();
        
    }

   
}