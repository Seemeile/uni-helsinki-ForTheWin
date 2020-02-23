using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Rendering;
using Unity.Mathematics;
using Unity.Transforms;


public class LoadHandler : MonoBehaviour
{/*
    public static List<Entity> listOfUnit;
    public static List<Entity> listOfHarvestable;

    private void Awake()
    {
        Debug.Log(SettingScript.isSaved);
        if(SettingScript.isSaved)
        {
            Debug.Log("issafe est vrai");
            SettingScript.isSaved = false;
            listOfUnit = SaveSystem.listOfUnit;
            listOfHarvestable = SaveSystem.listOfHarvestable;
            //spawning the unit entities where they were
            for(int k=0; k < listOfUnit.Count; k++)
            {
                SpawnUnit(listOfUnit[k]);
                
            }

        }

    }



    private void SpawnUnit(Entity unit)
    {
        EntityManager entityManager = World.Active.EntityManager;

        Translation translation = entityManager.GetComponentData<Translation>(unit);
        UnitComponent unitComponent = entityManager.GetComponentData<UnitComponent>(unit);
        TeamComponent teamComponent = entityManager.GetComponentData<TeamComponent>(unit);
        HealthComponent healthComponent = entityManager.GetComponentData<HealthComponent>(unit);
        FightComponent fightComponent = entityManager.GetComponentData<FightComponent>(unit);
        

        EntityArchetype entityArchetype = entityManager.CreateArchetype(
            typeof(Translation),
            typeof(UnitComponent),
            typeof(RenderMesh),
            typeof(LocalToWorld),
            typeof(AnimationComponent),
            typeof(TeamComponent),
            typeof(HealthComponent),
            typeof(FightComponent)
        //typeof(IsWalkableComponent)
        );

        Material mat = new Material(Shader.Find("Unlit/Transparent"));
        Sprite tileSprite = Resources.Load<Sprite>("Sprites/Animation/" + UnitData.getUnitSprite(unitComponent.unitType));
        mat.mainTexture = tileSprite.texture;

        float ratio = (float)tileSprite.texture.height / (float)tileSprite.texture.width;
        int width = 1;
        float height = 1 * ratio;
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

        Entity entity = entityManager.CreateEntity(entityArchetype);

        // Set unit translation
        float3 spawnPos = new float3(translation.Value.x, translation.Value.y, -2);
        entityManager.SetComponentData(entity, new Translation { Value = spawnPos });
        entityManager.SetSharedComponentData(entity, new RenderMesh
        {
            mesh = mesh,
            material = mat,
        });

        // Set unit component
        entityManager.SetComponentData(entity, new UnitComponent
        {
            unitType = unitComponent.unitType
        });

        // Set animation
        entityManager.SetComponentData(entity, new AnimationComponent
        {
            direction = UnitDirection.RIGHT,
            animationType = UnitAnimation.IDLE,
            currentFrame = 0,
            frameCount = UnitData.idleAnimations[unitComponent.unitType].Length,
            frameTimer = 0f,
            frameTimerMax = 0.3f
        });

        // Set unit team
        entityManager.SetComponentData(entity, new TeamComponent
        {
            number = teamComponent.number
        });

        // Set unit health
        entityManager.SetComponentData(entity, new HealthComponent
        {
            health = healthComponent.health,
            bar = healthComponent.bar

        });
        //Set FightComponent
        entityManager.SetComponentData(entity, new FightComponent
        {
            isFighting = fightComponent.isFighting,
            hasToMove = fightComponent.hasToMove
        });

    }*/
}
