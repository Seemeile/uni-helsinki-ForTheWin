using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class DamagingSystem : ComponentSystem
{
    protected override void OnUpdate() 
    {
        Entities.ForEach((ref HealthComponent healthComponent) => {
            healthComponent.health -= 1f * Time.deltaTime;
        });
    }
}
