using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;
using UnityEngine.Tilemaps;

public class GameHandler : MonoBehaviour
{
    public static GameHandler instance;

    public Mesh demonMesh;
    public Material demonMaterial;
    public Transform selectionAreaTransform;
    public Material unitSelectedCircleMaterial;
    public Mesh unitSelectedCircleMesh;
    public Tilemap pathfindingTest;
    public Grid grid;

    private EntityManager entityManager;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        EntityManager entityManager = World.Active.EntityManager;

        EntityArchetype entityArchetype = entityManager.CreateArchetype(
            typeof(Translation),
            typeof(UnitComponent),
            typeof(RenderMesh),
            typeof(LocalToWorld)
            //typeof(SpriteSheetAnimation_Data)
        );

        Material mat = new Material(Shader.Find("Unlit/Transparent"));
        Sprite tileSprite = Resources.Load<Sprite>("Sprites/Animation/knight_f_idle_anim_f0");
        mat.mainTexture = tileSprite.texture;
        
        int width = 1;
        int height = 1;
        Mesh mesh = new Mesh();
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
        mesh.vertices = newVertices;
        mesh.uv = newUVs;
        mesh.triangles = newTriangles;
        mesh.normals = newNormals;
        
        Entity entity = entityManager.CreateEntity(entityArchetype);

        float3 spawnPos = new float3(+0.5f, +0.5f, -1);
        entityManager.SetComponentData(entity, new Translation { Value = spawnPos });
        //entityManager.SetComponentData(entity, new SpriteSheetAnimation_Data { currentFrame = 0, frameCount = 4, frameTimer = 0f, frameTimerMax = 0.1f });
        entityManager.SetSharedComponentData(entity, new RenderMesh
        {
            mesh = mesh,
            material = mat,
        });
    }
}


