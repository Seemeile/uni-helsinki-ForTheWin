using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Rendering;
using Unity.Mathematics;
using Unity.Transforms;

public class BuildingData
{
    private static Dictionary<int, BuildingType> typeToTileNo = new Dictionary<int, BuildingType> {
        {0, BuildingType.FARM},
        {26, BuildingType.BARRACK},
        {28, BuildingType.HQ}
    };

    private static Dictionary<BuildingType, UnitType[]> buildableUnits = new Dictionary<BuildingType, UnitType[]> {
        {BuildingType.HQ, new UnitType[] { UnitType.PEASANT }},
        {BuildingType.BARRACK, new UnitType[] { UnitType.KNIGHT, UnitType.ELF }},
        {BuildingType.FARM, new UnitType[] {}}
    };

    private static Dictionary<BuildingType, int[]> buildingCosts = new Dictionary<BuildingType, int[]> {
        {BuildingType.HQ, new int[]{ 1000, 200 }},
        {BuildingType.BARRACK, new int[]{ 400, 150 }},
        {BuildingType.FARM, new int[]{ 100, 50 }}
    };

    public static int[] getBuildingCosts(BuildingType buildingType)
    {
        return buildingCosts[buildingType];
    }

    public static int[] getBuildingCostsByName(string spriteName)
    {
        int structureNumber = int.Parse(spriteName.Substring(spriteName.LastIndexOf('_') + 1));
        BuildingType buildingType = getBuildingType(structureNumber);
        return getBuildingCosts(buildingType);
    }

    public static UnitType[] getBuildableUnits(BuildingType buildingType)
    {
        return buildableUnits[buildingType];
    }

    public static BuildingType getBuildingType(int tileNo)
    {
        return typeToTileNo[tileNo];
    }

    public static int getTileNumber(BuildingType buildingType)
    {
        foreach(KeyValuePair<int, BuildingType> entry in typeToTileNo)
        {
            if (buildingType.Equals(entry.Value)) 
            {
                return entry.Key;
            }
        }
        return 0;
    }

    public static void spawnBuilding(BuildingType type, int gridPosX, int gridPosY)
    {
        EntityManager entityManager = World.Active.EntityManager;   
        EntityArchetype structureArchetype = entityManager.CreateArchetype(
            typeof(StructureComponent),
            typeof(Translation),
            typeof(RenderMesh),
            typeof(LocalToWorld),
            typeof(BlockableEntityComponent)
        );

        int tileNo = BuildingData.getTileNumber(type);
        Material tileMat = new Material(Shader.Find("Unlit/Transparent"));
        Sprite tileSprite = Resources.Load<Sprite>("Sprites/TileSprites/tileset16x16_1_" + tileNo);
        tileMat.mainTexture = tileSprite.texture;
        
        int width = 1;
        int height = 1;
        Mesh tileMesh = new Mesh();
        // Setup vertices
        Vector3[] newVertices = new Vector3[4];
        newVertices [0] = new Vector3 (0, 0, 0);
        newVertices [1] = new Vector3 (0, height, 0);
        newVertices [2] = new Vector3 (width, 0, 0);
        newVertices [3] = new Vector3 (width, height, 0);
        
        // Setup UVs
        Vector2[] newUVs = new Vector2[newVertices.Length];
        newUVs [0] = new Vector2 (0, 0);
        newUVs [1] = new Vector2 (0, 1);
        newUVs [2] = new Vector2 (1, 0);
        newUVs [3] = new Vector2 (1, 1);
        
        // Setup triangles
        int[] newTriangles = new int[] { 0, 1, 2, 3, 2, 1 };
        
        // Setup normals
        Vector3[] newNormals = new Vector3[newVertices.Length];
        for (int i = 0; i < newNormals.Length; i++) {
            newNormals [i] = Vector3.forward;
        }
        
        // Create quad
        tileMesh.vertices = newVertices;
        tileMesh.uv = newUVs;
        tileMesh.triangles = newTriangles;
        tileMesh.normals = newNormals;
        
        Entity entity = entityManager.CreateEntity(structureArchetype);
        entityManager.SetSharedComponentData(entity, new RenderMesh {
            material = tileMat,
            mesh = tileMesh
        });

        entityManager.SetComponentData(entity, new Translation {
            Value = new float3(gridPosX, gridPosY, -1)
        });

        entityManager.SetComponentData(entity, new StructureComponent {
            type = type
        });
    }
}
