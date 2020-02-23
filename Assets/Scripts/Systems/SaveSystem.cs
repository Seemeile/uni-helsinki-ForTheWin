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
    public static List<Entity> listOfUnit = new List<Entity>();
    public static List<Entity> listOfHarvestable = new List<Entity>();

    protected override void OnUpdate()
    {/*
        if (SceneManager.GetActiveScene().name == "test")
        {
            if (SettingScript.isSaved)
            {
                Entities.WithAll<UnitComponent>().ForEach((Entity entity) =>
                {
                    if (!listOfUnit.Contains(entity))
                    {
                        listOfUnit.Add(entity);
                    }
                });
            }
        }
    }

    public void DestroyHarvestable(Entity harvestable)
    {
        Entities.WithAll<HarvestableComponent>().ForEach((Entity entity) =>
        {
            if (entity == harvestable)
            {
                PostUpdateCommands.DestroyEntity(entity);
            }
        });*/
    }
}

