using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine.SceneManagement;


public class HealthSystem : ComponentSystem
{
    EntityManager entityManager = World.Active.EntityManager;
    float time = 1;
    protected override void OnUpdate()
    {
        if (SceneManager.GetActiveScene().name == "test")
        {

            Entities.WithAll<HealthComponent>().ForEach((Entity entity, ref Translation translation, ref HealthComponent health) =>
            {
            if (!health.bar)
                {
                    Vector3Int currentPosition = GameHandler.instance.tilemap.WorldToCell(translation.Value);
                    health.healthBar = UnitData.spawnHealthBar(UnitType.HEALTHBAR, currentPosition.x, currentPosition.y, entity);
                    health.bar = true;
                }
            });

            Entities.WithAll<HealthBarComponent>().ForEach((Entity entity, ref Translation translation, ref HealthBarComponent healthBar, ref UnitComponent unit) =>
            {
                HealthComponent hc = EntityManager.GetComponentData<HealthComponent>(healthBar.soldier);
                float healthValue = hc.health;

            //Move the health bar
            float3 currentPosition = Position(healthBar.soldier);
                translation.Value = currentPosition;

                time += Time.deltaTime;
            //Decrease the health bar
            Mesh mesh = CreateNewQuad(healthValue);
                Material mat = new Material(Shader.Find("Unlit/Transparent"));
                Sprite tileSprite = Resources.Load<Sprite>("Sprites/Animation/" + "healthBarSprite");
                mat.mainTexture = tileSprite.texture;

                entityManager.SetSharedComponentData<RenderMesh>(entity, new RenderMesh
                {
                    mesh = mesh,
                    material = mat
                });
            });
        }
    }


    // to get to position of an entity
    private float3 Position(Entity unit)
    {
        float3 potentialPosition = new float3();
        Entities.WithAll<UnitComponent>().ForEach((Entity entity, ref Translation translation) =>
        {
            float3 currentPosition = translation.Value;
            if (entity == unit)
            {
                RenderMesh unitRenderer = entityManager.GetSharedComponentData<RenderMesh>(entity);
                float unitMeshHeight = unitRenderer.mesh.vertices[1].y;
                potentialPosition = translation.Value;
                potentialPosition.x = currentPosition.x;
                potentialPosition.y = currentPosition.y + unitMeshHeight;
                potentialPosition.z = currentPosition.z;
            }
        });
        return potentialPosition;
    }

    private Mesh CreateNewQuad(float health)
    {
        var mesh = new Mesh();
        float height = 0.1f;
        Vector3[] newVertices = new Vector3[4];
        newVertices[0] = new Vector3(0, 0, 0);
        newVertices[1] = new Vector3(0, height, 0);
        newVertices[2] = new Vector3(health / 100, 0, 0);
        newVertices[3] = new Vector3(health / 100, height, 0);

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
