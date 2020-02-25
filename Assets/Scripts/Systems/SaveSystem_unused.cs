using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using System;
using UnityEngine.SceneManagement;

/*
public class SaveSystem : ComponentSystem
{
    public static List<Entity> listOfPreviousUnits = new List<Entity>();
    public static List<Entity> listOfPreviousHealthBar = new List<Entity>();
    public static List<Entity> listOfDestroyedHarvestable = new List<Entity>();
    public static List<Entity> listOfPreviousStructures = new List<Entity>();

    public int woodAmount = new int();
    public int goldAmount = new int();

    public static bool loading;

    protected override void OnUpdate()
    {
        for(int k=0; k<listOfDestroyedHarvestable.Count; k++)
        {
            Debug.Log("tree " + listOfDestroyedHarvestable[k]);
        }

        if (SceneManager.GetActiveScene().name == "test")
        {
            //saving all the entities
            Entities.WithAny<UnitComponent>().ForEach((Entity entity) =>
            {
                if (!listOfPreviousUnits.Contains(entity))
                {
                    listOfPreviousUnits.Add(entity);
                }
            });
            Entities.WithAny<StructureComponent>().ForEach((Entity entity) =>
            {
                if (!listOfPreviousStructures.Contains(entity))
                {
                    listOfPreviousStructures.Add(entity);
                }
            });

            //Saving the datas
            woodAmount = UI.instance.getWoodAmount();
            goldAmount = UI.instance.getGoldAmount();


            //NEW GAME
            if (GameHandler.hasToDelete)
            {
                DestroyUnits(listOfPreviousStructures);
                DestroyUnits(listOfPreviousUnits);
                GameHandler.hasToDelete = false;

            }

            //LOAD THE PREVIOUS GAME
            //destroy all the ressources that have already been harvested 

            if(loading)
            {
                Debug.Log("destroy tree");
                DestroyUnits(listOfDestroyedHarvestable);

                UI.instance.setWoodAmount(woodAmount);
                Debug.Log("setAmount");
                UI.instance.setGoldAmount(goldAmount);

                loading = false;
            }
        }
        
    }

    public void DestroyUnits(List<Entity> list)
    {
        for(int k=0; k<list.Count; k++)
        {
            PostUpdateCommands.DestroyEntity(list[k]);
            
        }
        list.Clear();
    }
}
*/
