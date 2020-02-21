using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


public class HealthSystem : ComponentSystem
{
    protected override void OnUpdate()
    {

        Entities.WithAll<HealthComponent>().ForEach((ref Translation translation, ref HealthComponent health) =>
        {
            if (!health.bar)
            {
                //GameObject.Instantiate(health.healthBar, translation.Value, Quaternion.identity);
                health.bar = true;
            }
        });
    }
}
