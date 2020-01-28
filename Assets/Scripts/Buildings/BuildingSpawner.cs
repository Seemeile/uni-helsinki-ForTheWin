using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;

public class BuildingSpawner : MonoBehaviour
{
    [SerializeField] private Mesh mesh;
    [SerializeField] private Material material;

    // Start is called before the first frame update
    void Start()
    {
        EntityManager entityManager = World.Active.EntityManager;   
        
        EntityArchetype entityArchetype = entityManager.CreateArchetype(
            typeof(HealthComponent),
            typeof(Translation),
            typeof(MeshRenderer),
            typeof(LocalToWorld)            
        );

        Entity entity = entityManager.CreateEntity(entityArchetype);
        entityManager.SetComponentData(entity, new HealthComponent { health = 100 });
        entityManager.SetSharedComponentData(entity, new RenderMesh {
            mesh = mesh,
            material = material
        });
    }
}
