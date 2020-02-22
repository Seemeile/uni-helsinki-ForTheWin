using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


public class HealthSystem : ComponentSystem
{
    int iD;
    GameObject gameObject;
    protected override void OnUpdate()
    {

        Entities.WithAll<HealthComponent>().ForEach((Entity entity, ref Translation translation, ref HealthComponent health) =>
        {
            if (!health.bar)
            {
                Vector3Int currentPosition = GameHandler.instance.tilemap.WorldToCell(translation.Value);
                UnitData.spawnHealthBar(UnitType.HEALTHBAR, currentPosition.x, currentPosition.y + 0.3f, entity);
                health.bar = true;
            }
        });

        Entities.WithAll<HealthBarComponent>().ForEach((Entity entity, ref Translation translation,  ref HealthBarComponent healthBar) =>
        {
            float3 currentPosition = Position(healthBar.soldier);
            translation.Value = currentPosition;
        });
    }



    // to get to position of an entity
    private float3 Position(Entity unit)
    {
        float3 potentialPosition = new float3();
        Entities.WithAll<UnitComponent>().ForEach((Entity entity, ref Translation translation) =>
        {
            float3 currentPosition = translation.Value;
            if (entity == unit)
            {
                potentialPosition = translation.Value;
                potentialPosition.x = currentPosition.x;
                potentialPosition.y = currentPosition.y + 0.3f;
                potentialPosition.z = currentPosition.z;
            }
        });
        return potentialPosition;
    }


}
