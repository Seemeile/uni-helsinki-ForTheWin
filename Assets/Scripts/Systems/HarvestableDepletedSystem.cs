using System.Collections;
using System.Collections.Generic;
using Unity.Entities;

public class HarvestableDepletedSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, ref HarvestableComponent harvestableComponent) => 
        {
            if (harvestableComponent.ressourceAmount <= 0)
            {
                EntityManager.DestroyEntity(entity);
            }
        });
    }
}