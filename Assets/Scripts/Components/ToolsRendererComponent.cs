using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;
using System.Collections;
using System.Collections.Generic;

public class ToolsRendererComponent : ComponentSystem
{
    private float3 currentMousePosition;
    private Vector3Int currentMouseCell;
    private Vector3Int entityCell;
    private bool isSelected =false;
    private Vector3Int entitySelectedcell;


    protected override void OnUpdate()
    {//Displaying an axe if the mouse is over a tree
        currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentMouseCell = GameHandler.instance.tilemap.WorldToCell(currentMousePosition);


        Cursor.visible = true;
        isSelected = false;

        Entities.WithAll<UnitComponent, EntitySelectedComponent>().ForEach((Entity entity, ref Translation translation) =>
        {
            isSelected = true;
            entitySelectedcell = GameHandler.instance.tilemap.WorldToCell(translation.Value);
        });

        //If a unit is selected, then we can order it to do something
        if (isSelected)
        {

            Entities.WithAll<Translation, HarvestableComponent>().ForEach((Entity entity, ref Translation translation, ref HarvestableComponent harvestableComponent) =>
            {
                entityCell = GameHandler.instance.tilemap.WorldToCell(translation.Value);

                if (harvestableComponent.type.Equals(HarvestableType.WOOD))
                {//If the mouse is above a tree

                if (currentMouseCell.x == entityCell.x && currentMouseCell.y == entityCell.y)
                    {
                        Cursor.visible = false;
                        currentMousePosition.z = -10f;
                        Graphics.DrawMesh(
                       GameHandler.instance.axeMesh,
                       currentMousePosition, Quaternion.identity,
                       GameHandler.instance.axeMaterial,
                       0
                       );

                    };

                }

                if (harvestableComponent.type.Equals(HarvestableType.GOLDMINE))
                {//If the mouse is avove a gold mine
                if (currentMouseCell.x == entityCell.x && currentMouseCell.y == entityCell.y)
                    {
                        Cursor.visible = false;
                        currentMousePosition.z = -10f;
                        Graphics.DrawMesh(
                       GameHandler.instance.pickAxeMesh,
                       currentMousePosition, Quaternion.identity,
                       GameHandler.instance.pickAxeMaterial,
                       0
                       );

                    };
                }
            });

            Entities.WithAll<Translation, UnitComponent>().ForEach((Entity entity, ref Translation translation) =>
            {
                entityCell = GameHandler.instance.tilemap.WorldToCell(translation.Value);
            //if the mouse is over a unit which is no the current selected unit
            if (currentMouseCell.x == entityCell.x && currentMouseCell.y == entityCell.y && currentMouseCell.x != entitySelectedcell.x && currentMouseCell.y != entitySelectedcell.y)
                {
                    Cursor.visible = false;
                    currentMousePosition.z = -10f;
                    Graphics.DrawMesh(
                   GameHandler.instance.swordMesh,
                   currentMousePosition, Quaternion.identity,
                   GameHandler.instance.swordMaterial,
                   0
                   );

                };

            });


        }
    }

}
