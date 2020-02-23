using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using System;
using UnityEngine.SceneManagement;


public class SaveSystem : ComponentSystem
{
    public static List<Entity> listOfHarvestable = new List<Entity>();
    public int woodAmount = new int();
    public int goldAmount = new int();

    protected override void OnUpdate()
    {/*

        woodAmount = UI.instance.getWoodAmount();
        goldAmount = UI.instance.getGoldAmount();

        for (int k=0; k < listOfHarvestable.Count ;  k++)
        {

        }*/
    }

    public void DestroyHarvestable(Entity harvestable)
    {
        Entities.WithAll<HarvestableComponent>().ForEach((Entity entity) =>
        {
            if (entity == harvestable)
            {
                PostUpdateCommands.DestroyEntity(entity);
            }
        });
    }
}

