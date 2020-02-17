using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
/*
public class HealthSystem : ComponentSystem
{

    protected override void OnUpdate()
    {
        Entities.WithAll<HealthComponent>().ForEach((Entity entity, ref Translation translation, ref HealthComponent healthBar) =>
        {
            GameObject clone = GameHandler.Instantiate(GameHandler.instance.healthBarGameObject, translation.Value, Quaternion.identity);
            GameObject.Destroy(clone, Time.deltaTime);
           

        });

       
    }
    
}
*/
