using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;

public class TilemapObjectsToEntities : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private Mesh mesh;

    // Start is called before the first frame update
    void Start()
    {
        EntityManager entityManager = World.Active.EntityManager;   
        EntityArchetype simpleEntityArchetype = entityManager.CreateArchetype(
            typeof(Translation),
            typeof(RenderMesh),
            typeof(LocalToWorld)
        );

        Tilemap tilemap = GetComponent<Tilemap>();
        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {   
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
            Vector3 place = tilemap.CellToWorld(localPlace);
            if (tilemap.HasTile(localPlace))
            {
                TileBase tile = tilemap.GetTile(localPlace);
                Entity entity = entityManager.CreateEntity(simpleEntityArchetype);
                entityManager.SetSharedComponentData(entity, new RenderMesh {
                    material = material,
                    mesh = mesh
                });
                entityManager.SetComponentData(entity, new Translation {
                    Value = new float3(place.x + 0.5f, place.y + 0.5f, -1)
                });
            }
        }
        tilemap.ClearAllTiles();
    }
}
