using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;

public class TileHarvestablesToEntities : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EntityManager entityManager = World.Active.EntityManager;   
        EntityArchetype harvestableComponent = entityManager.CreateArchetype(
            typeof(HarvestableComponent),
            typeof(Translation),
            typeof(RenderMesh),
            typeof(LocalToWorld),
            typeof(BlockableEntityComponent)
        );

        Tilemap tilemap = GetComponent<Tilemap>();
        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {   
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
            Vector3 place = tilemap.CellToWorld(localPlace);
            if (tilemap.HasTile(localPlace))
            {
                TileBase tile = tilemap.GetTile(localPlace);
                
                Material tileMat = new Material(Shader.Find("Unlit/Transparent"));
                Sprite tileSprite = Resources.Load<Sprite>("Sprites/TileSprites/" + tile.name);
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
                
                Entity entity = entityManager.CreateEntity(harvestableComponent);
                entityManager.SetSharedComponentData(entity, new RenderMesh {
                    material = tileMat,
                    mesh = tileMesh
                });

                // pivot of the sprite is center so we have to add 0.5 to x and y
                entityManager.SetComponentData(entity, new Translation {
                    Value = new float3(place.x + 0.5f, place.y + 0.5f, -1)
                });

                int tileNo = int.Parse(tile.name.Substring(tile.name.LastIndexOf('_') + 1));
                HarvestableType type = HarvestableData.getHarvestableType(tileNo);
                entityManager.SetComponentData(entity, new HarvestableComponent {
                    type = type,
                    ressourceAmount = type.Equals(HarvestableType.WOOD) ? 50 : 20000
                });

                
            }
        }
        // delete all tiles because we replaced them with entities
        tilemap.ClearAllTiles();
    }
}
