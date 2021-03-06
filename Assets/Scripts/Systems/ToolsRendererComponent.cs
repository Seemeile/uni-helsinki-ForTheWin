﻿using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ToolsRendererComponent : ComponentSystem
{
    protected override void OnUpdate()
    {
        Cursor.visible = true;
        bool peasantIsSelected = false;
        Entities.WithAll<EntitySelectedComponent>().ForEach((ref UnitComponent unitComponent) =>
        {
            if (UnitType.PEASANT.Equals(unitComponent.unitType))
            {
                peasantIsSelected = true;
            }
        });

        if (peasantIsSelected)
        {
            Entities.ForEach((ref Translation translation, ref HarvestableComponent harvestableComponent) =>
            {
                Vector3 currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int currentMouseCell = GameHandler.instance.tilemap.WorldToCell(currentMousePosition);
                Vector3Int entityCell = GameHandler.instance.tilemap.WorldToCell(translation.Value);

                if (harvestableComponent.type.Equals(HarvestableType.WOOD))
                {
                    //If the mouse is above a tree
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
                {
                    //If the mouse is avove a gold mine
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
        }

        Entities.WithNone<EntitySelectedComponent>().ForEach((ref Translation translation, ref TeamComponent team) =>
        {
            Vector3 currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int currentMouseCell = GameHandler.instance.tilemap.WorldToCell(currentMousePosition);
            Vector3Int entityCell = GameHandler.instance.tilemap.WorldToCell(translation.Value);

            //if the mouse is over a unit which is no the current selected unit
            if (currentMouseCell.x == entityCell.x && currentMouseCell.y == entityCell.y && team.number == 1)
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
