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

 
    protected override void OnUpdate()
    {//Displaying an axe if the mouse is over a tree
        currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentMouseCell = GameHandler.instance.environmentTilemap.WorldToCell(currentMousePosition);

        Entities.WithAll<Translation, HarvestableComponent>().ForEach((Entity entity, ref Translation translation, ref HarvestableComponent harvestableComponent) =>
        {
            entityCell = GameHandler.instance.environmentTilemap.WorldToCell(translation.Value);

            if (harvestableComponent.type.Equals(HarvestableType.WOOD))
            {//If the mouse is above a tree

                if (currentMouseCell.x == entityCell.x && currentMouseCell.y == entityCell.y)
                {
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
            {

                if (currentMouseCell.x == entityCell.x && currentMouseCell.y == entityCell.y)
                {
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
        
     

    }
}
