using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;

public class BuildingTest : MonoBehaviour
{
    [SerializeField] private GameObject tilemapLayer;
    [SerializeField] private Mesh mesh;
    [SerializeField] private Material material;

    // Start is called before the first frame update
    void Start()
    {
        
        /*
        EntityManager entityManager = World.Active.EntityManager;   
        EntityArchetype entityArchetype = entityManager.CreateArchetype(
            typeof(Translation),
            typeof(RenderMesh),
            typeof(LocalToWorld)            
        );

        Entity entity = entityManager.CreateEntity(entityArchetype);        
        entityManager.SetSharedComponentData(entity, new RenderMesh {
            mesh = mesh,
            material = material,
        });
        entityManager.SetComponentData(entity, new Translation {
            Value = new float3(0, 0, -1)
        });
        */
    }
}
