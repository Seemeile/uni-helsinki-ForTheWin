using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class ChooseToPlaceSystem : ComponentSystem
{
    protected override void OnUpdate() 
    {
        bool leftMouseClicked = false;
        bool rightMouseClicked = false;
        if (Input.GetMouseButtonDown(0))
        {
            leftMouseClicked = true;
        }
        else if (Input.GetMouseButtonDown(1))
        {
            rightMouseClicked = true;
        }

        Entities.ForEach((Entity chooseEntity, ref ChooseToPlaceComponent chooseToPlaceComponent) =>
        {
            EntityManager entityManager = World.Active.EntityManager;
            float3 currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int currentMouseCell = GameHandler.instance.tilemap.WorldToCell(currentMousePosition);

            BuildingType buildingType = chooseToPlaceComponent.buildingType;

            Material mat = new Material(Shader.Find("Unlit/Transparent"));
            int tileNo = BuildingData.getTileNumber(buildingType);
            Sprite buildingSprite = Resources.Load<Sprite>("Sprites/TileSprites/tileset16x16_1_" + tileNo);
            mat.mainTexture = buildingSprite.texture;

            currentMouseCell.z = -10;
            Graphics.DrawMesh(getSimpleQuadMesh(), currentMouseCell, Quaternion.identity, mat, 0);

            if (leftMouseClicked)
            {
                placeBuildingWhenPossible(chooseEntity, buildingType, currentMouseCell);
            }
            else if (rightMouseClicked)
            {
                entityManager.DestroyEntity(chooseEntity);
            }
        });
    }

    private void placeBuildingWhenPossible(Entity chooseEntity, BuildingType buildingType, Vector3Int currentMouseCell)
    {
        bool placeIsBlocked = false;
        Entities.ForEach((Entity entity, ref Translation translation) => 
        {
            if (currentMouseCell.x.Equals((int) translation.Value.x) && currentMouseCell.y.Equals((int) translation.Value.y))
            {
                placeIsBlocked = true;
            }
        });
        if (!placeIsBlocked) 
        {
            int[] costs = BuildingData.getBuildingCosts(buildingType);
            if (UI.instance.getGoldAmount() >= costs[0] && UI.instance.getWoodAmount() >= costs[1])
            {
                UI.instance.subGold(costs[0]);
                UI.instance.subWood(costs[1]);
                BuildingData.spawnBuilding(buildingType, currentMouseCell.x, currentMouseCell.y);
            } 
            else 
            {
                Debug.Log("not enough resources");
            }
            EntityManager.DestroyEntity(chooseEntity);
        }
    }

    private Mesh getSimpleQuadMesh() 
    {
        int width = 1;
        float height = 1;
        Mesh mesh = new Mesh();
        // Setup vertices
        Vector3[] newVertices = new Vector3[4];
        newVertices[0] = new Vector3(0, 0, 0);
        newVertices[1] = new Vector3(0, height, 0);
        newVertices[2] = new Vector3(width, 0, 0);
        newVertices[3] = new Vector3(width, height, 0);
        // Setup UVs
        Vector2[] newUVs = new Vector2[newVertices.Length];
        newUVs[0] = new Vector2(0, 0);
        newUVs[1] = new Vector2(0, 1);
        newUVs[2] = new Vector2(1, 0);
        newUVs[3] = new Vector2(1, 1);
        // Setup triangles
        int[] newTriangles = new int[] { 0, 1, 2, 3, 2, 1 };
        // Setup normals
        Vector3[] newNormals = new Vector3[newVertices.Length];
        for (int i = 0; i < newNormals.Length; i++)
        {
            newNormals[i] = Vector3.forward;
        }
        // Create quad
        mesh.vertices = newVertices;
        mesh.uv = newUVs;
        mesh.triangles = newTriangles;
        mesh.normals = newNormals;
        return mesh;
    }
}