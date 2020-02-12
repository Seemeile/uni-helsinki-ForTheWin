﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class BuildUnitSystem : ComponentSystem
{
    protected override void OnUpdate() 
    {
        EntityManager entityManager = World.Active.EntityManager;

        Entities.ForEach((Entity clickedEventEntity, ref UnitSlotClickedEvent UnitSlotClickedEvent) => 
        {
            UnitSlotClickedEvent clickEvent = UnitSlotClickedEvent;
            Entities.WithAll<StructureSelectedComponent>()
                .ForEach((ref StructureComponent structureComponent, ref Translation translation) => 
            {
                UnitType[] buildableUnits = BuildingData.getBuildableUnits(structureComponent.type);
                
                int2 spawnPosition = findFreeNeighborTile(translation.Value);

                if (!spawnPosition.Equals(new int2(0, 0))) 
                {
                    UnitData.spawnUnit(buildableUnits[clickEvent.slotNumber - 1], spawnPosition.x, spawnPosition.y);
                    //spawnUnit(buildableUnits[clickEvent.slotNumber - 1], spawnPosition);
                }
            });
            entityManager.DestroyEntity(clickedEventEntity);
        });
    }

    private int2 findFreeNeighborTile(float3 buildingTranslation) 
    {
        for (int radius = 1; radius <= 5; radius++) 
        {
            for (float x = buildingTranslation.x - radius; x <= buildingTranslation.x + radius; x++)
            {
                for (float y = buildingTranslation.y - radius; y <= buildingTranslation.y + radius; y++)
                {
                    bool foundEntity = false;
                    Entities.ForEach((ref Translation translation) => 
                    {
                        if (translation.Value.x > x - 0.1f 
                            && translation.Value.x < x + 0.1f
                            && translation.Value.y < y + 0.1f
                            && translation.Value.y > y - 0.1f) 
                        {
                            foundEntity = true;
                        }
                    });
                    if (!foundEntity) {
                        return new int2((int) x, (int) y);
                    }
                }
            }
        }
        return 0;
    }

    /*
    private void spawnUnit(UnitType unit, float3 spawnPosition)
    {
        EntityManager entityManager = World.Active.EntityManager;
        EntityArchetype unitArchetype = entityManager.CreateArchetype(
            typeof(Translation),
            typeof(UnitComponent),
            typeof(RenderMesh),
            typeof(LocalToWorld)
            //typeof(SpriteSheetAnimation_Data)
        );
        Entity entity = entityManager.CreateEntity(unitArchetype);
        entityManager.SetComponentData(entity, new Translation { Value = spawnPosition });
        //entityManager.SetComponentData(entity, new SpriteSheetAnimation_Data { currentFrame = 0, frameCount = 4, frameTimer = 0f, frameTimerMax = 0.1f });
        
        Material tileMat = new Material(Shader.Find("Unlit/Transparent"));
        Sprite tileSprite = Resources.Load<Sprite>("Sprites/Animation/" + UnitData.getUnitSprite(unit));
        tileMat.mainTexture = tileSprite.texture;
        
        int width = 1;
        int height = 1;
        Mesh tileMesh = new Mesh();
        // Setup vertices
        Vector3[] newVertices = new Vector3[4];
        float halfHeight = height * 0.5f;
        float halfWidth = width * 0.5f;
        newVertices [0] = new Vector3 (-halfWidth, -halfHeight, 0);
        newVertices [1] = new Vector3 (-halfWidth, halfHeight, 0);
        newVertices [2] = new Vector3 (halfWidth, -halfHeight, 0);
        newVertices [3] = new Vector3 (halfWidth, halfHeight, 0);
        
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

        entityManager.SetSharedComponentData(entity, new RenderMesh
        {
            mesh = tileMesh,
            material = tileMat,
        });

        entityManager.SetComponentData(entity, new UnitComponent {
            unitType = unit
        });
    }
    */
}