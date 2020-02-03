using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Jobs;
using Unity.Burst;


public class GameHandler : MonoBehaviour
{
    public static GameHandler instance;

    public Mesh demonMesh;
    public Material demonMaterial;
    public Transform selectionAreaTransform;
    public Material unitSelectedCircleMaterial;
    public Mesh unitSelectedCircleMesh;
    private EntityManager entityManager;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        entityManager = World.Active.EntityManager;

        for (int i=0; i < 36; i++)
        {
            SpawnDemon();
        }
    }

    private void SpawnDemon()
    {
        SpawnDemon(new float3(UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(-5f, 5f), -10f));
    }
    private void SpawnDemon(float3 spawnPosition)
    {
        EntityArchetype entityArchetype = entityManager.CreateArchetype(
            typeof(Demon),
            typeof(Translation),
            typeof(MoveToComponent),
            typeof(RenderMesh),
            typeof(LocalToWorld),
            typeof(SpriteSheetAnimation_Data)
        );

        Entity entity = entityManager.CreateEntity(entityArchetype);

        entityManager.SetComponentData(entity, new Translation { Value = spawnPosition });
        entityManager.SetComponentData(entity, new MoveToComponent { move = true, position = spawnPosition, moveSpeed = 20f });
        entityManager.SetComponentData(entity, new SpriteSheetAnimation_Data { currentFrame = 0, frameCount = 4, frameTimer = 0f, frameTimerMax = 0.1f });
        entityManager.SetSharedComponentData(entity, new RenderMesh
        {
            mesh = demonMesh,
            material = demonMaterial,
        });
    }

}


