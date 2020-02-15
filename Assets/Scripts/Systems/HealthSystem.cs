using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class HealthSystem : ComponentSystem
{
    private float3 startPosition;
    private float3 endPosition;


    protected override void OnUpdate()
    {
        Entities.WithAll<HealthComponent>().ForEach((Entity entity, ref Translation translation, ref HealthComponent healthBar) =>
        {
            if (!healthBar.isPresent)
            {
                GameHandler.instance.healthBarTransform.gameObject.SetActive(true);
                GameHandler.Instantiate(GameHandler.instance.healthBarTransform, translation.Value, Quaternion.identity);
                healthBar.isPresent = true;
            }

        });

        Entities.WithAll<HealthComponent>().ForEach((Entity entity, ref Translation translation, ref HealthComponent healthBar) =>
        {
            if (healthBar.isPresent)
            {

            }

        });
    }
    
}
