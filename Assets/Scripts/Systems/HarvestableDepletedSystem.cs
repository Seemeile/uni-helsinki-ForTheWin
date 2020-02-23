using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine.SceneManagement;

public class HarvestableDepletedSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        if (SceneManager.GetActiveScene().name == "test")
        {
            Entities.ForEach((Entity entity, ref HarvestableComponent harvestableComponent) =>
        {
            if (harvestableComponent.ressourceAmount <= 0)
            {
                EntityManager.DestroyEntity(entity);
                SaveSystem.listOfHarvestable.Add(entity);
            }
        });
        }
    }
}