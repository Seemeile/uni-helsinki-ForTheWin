using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Rendering;
using Unity.Mathematics;
using Unity.Transforms;

public class UnitData
{


    private static Dictionary<UnitType, string> unitSprites = new Dictionary<UnitType, string> {
        {UnitType.WIZARD, "wizzard_m_idle_anim_f0" },
        {UnitType.PEASANT, "skelet_idle_anim_f0" },
        {UnitType.KNIGHT, "knight_f_idle_anim_f0" },
        {UnitType.ELF, "elf_f_idle_anim_f0" },
        {UnitType.HEALTHBAR, "healthBarSprite"},
    };



    private static Dictionary<UnitType, string[]> idleAnimations = new Dictionary<UnitType, string[]> {
        {UnitType.WIZARD, new string[]{ "wizzard_m_idle_anim_f0", "wizzard_m_idle_anim_f1", "wizzard_m_idle_anim_f2", "wizzard_m_idle_anim_f3"} },
        {UnitType.PEASANT, new string[]{ "skelet_idle_anim_f0", "skelet_idle_anim_f1", "skelet_idle_anim_f2", "skelet_idle_anim_f3"} },
        {UnitType.KNIGHT, new string[]{ "knight_f_idle_anim_f0", "knight_f_idle_anim_f1", "knight_f_idle_anim_f2", "knight_f_idle_anim_f3"} },
        {UnitType.ELF, new string[]{ "elf_f_idle_anim_f0", "elf_f_idle_anim_f1", "elf_f_idle_anim_f2", "elf_f_idle_anim_f3"} },
    };

    private static Dictionary<UnitType, string[]> runAnimations = new Dictionary<UnitType, string[]> {
        {UnitType.WIZARD, new string[]{ "wizzart_m_run_anim_f0", "wizzart_m_run_anim_f1", "wizzart_m_run_anim_f2", "wizzart_m_run_anim_f3"} },
        {UnitType.PEASANT, new string[]{ "skelet_run_anim_f0", "skelet_run_anim_f1", "skelet_run_anim_f2", "skelet_run_anim_f3"} },
        {UnitType.KNIGHT, new string[]{ "knight_f_run_anim_f0", "knight_f_run_anim_f1", "knight_f_run_anim_f2", "knight_f_run_anim_f3"} },
        {UnitType.ELF, new string[]{ "elf_f_run_anim_f0", "elf_f_run_anim_f1", "elf_f_run_anim_f2", "elf_f_run_anim_f3"} },
    };

    private static Dictionary<UnitType, string[]> fightAnimations = new Dictionary<UnitType, string[]> {
        {UnitType.WIZARD, new string[]{ "wizzart_m_run_anim_f0", "wizzart_m_run_anim_f1", "wizzart_m_hit_anim_f0", "wizzart_m_run_anim_f1", "wizzart_m_run_anim_f0"} },
        {UnitType.PEASANT, new string[]{ "skelet_idle_anim_f3", "skelet_run_anim_f0", "skelet_run_anim_f1", "skelet_run_anim_f0", "skelet_idle_anim_f3"} },
        {UnitType.KNIGHT, new string[]{ "knight_f_idle_anim_f3", "knight_f_run_anim_f0", "knight_f_run_anim_f1", "knight_f_hit_anim_f0", "knight_f_run_anim_f1", "knight_f_run_anim_f0", "knight_f_idle_anim_f3"} },
        {UnitType.ELF, new string[]{ "elf_f_idle_anim_f1", "elf_f_idle_anim_f0", "elf_f_hit_anim_f0", "elf_f_idle_anim_f0", "elf_f_idle_anim_f1"} },
    };

    private static Dictionary<UnitType, BuildingType[]> unitActions = new Dictionary<UnitType, BuildingType[]> {
        {UnitType.WIZARD, new BuildingType[] { BuildingType.HQ }},
        {UnitType.KNIGHT, new BuildingType[] { }},
        {UnitType.PEASANT, new BuildingType[] { BuildingType.FARM, BuildingType.BARRACK }},
        {UnitType.ELF, new BuildingType[] { }},
    };

    private static Dictionary<UnitType, int[]> unitCosts = new Dictionary<UnitType, int[]> {
        {UnitType.PEASANT, new int[]{ 50, 0 }},
        {UnitType.KNIGHT, new int[]{ 100, 0 }},
        {UnitType.ELF, new int[]{ 100, 50 }}
    };

    public static int[] getUnitCosts(UnitType unitType)
    {
        return unitCosts[unitType];
    }

    public static BuildingType[] getUnitActions(UnitType unitType)
    {
        return unitActions[unitType];
    }

    public static string getUnitAnimation(UnitType unitType, UnitAnimation unitAnimation, int animationIndex)
    {
        if (UnitAnimation.RUN == unitAnimation) {
            return runAnimations[unitType][animationIndex];
        } else if (UnitAnimation.FIGHT == unitAnimation) {
            return fightAnimations[unitType][animationIndex];
        } else if (UnitAnimation.HARVEST == unitAnimation) {
            return fightAnimations[unitType][animationIndex];
        } else {
            return idleAnimations[unitType][animationIndex];
        }
    }

    public static int getUnitAnimationCount(UnitType unitType, UnitAnimation unitAnimation) 
    {
        if (UnitAnimation.RUN == unitAnimation) {
            return runAnimations[unitType].Length;
        } else if (UnitAnimation.FIGHT == unitAnimation) {
            return fightAnimations[unitType].Length;
        } else if (UnitAnimation.HARVEST == unitAnimation) {
            return fightAnimations[unitType].Length;
        } else {
            return idleAnimations[unitType].Length;
        }
    }

    public static string getUnitSprite(UnitType unitType)
    {
        return unitSprites[unitType];
    }
    
    public static void spawnUnit(UnitType unitType, int gridPosX, int gridPosY)
    {
        EntityManager entityManager = World.Active.EntityManager;

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
        Sprite tileSprite = Resources.Load<Sprite>("Sprites/Animation/" + getUnitSprite(unitType));
        mat.mainTexture = tileSprite.texture;
        
        float ratio = (float) tileSprite.texture.height / (float) tileSprite.texture.width;
        int width = 1;
        float height = 1 * ratio;
        Mesh mesh = new Mesh();
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
        mesh.vertices = newVertices;
        mesh.uv = newUVs;
        mesh.triangles = newTriangles;
        mesh.normals = newNormals;
        
        Entity entity = entityManager.CreateEntity(entityArchetype);

        // Set unit translation
        float3 spawnPos = new float3(gridPosX, gridPosY, -2);
        entityManager.SetComponentData(entity, new Translation { Value = spawnPos });
        entityManager.SetSharedComponentData(entity, new RenderMesh {
            mesh = mesh,
            material = mat,
        });

        // Set unit component
        entityManager.SetComponentData(entity, new UnitComponent {
            unitType = unitType
        });

        // Set animation
        entityManager.SetComponentData(entity, new AnimationComponent {
            direction = UnitDirection.RIGHT,
            animationType = UnitAnimation.IDLE,
            currentFrame = 0,
            frameCount = idleAnimations[unitType].Length,
            frameTimer = 0f,
            frameTimerMax = 0.3f
        });

        // Set unit team
        entityManager.SetComponentData(entity, new TeamComponent {
            number = 0
        });

        // Set unit health
        entityManager.SetComponentData(entity, new HealthComponent
        {
            health = 100,
            bar = false
        });

        //Set FightComponent
        entityManager.SetComponentData(entity, new FightComponent
        {
            isFighting = false,
            hasToMove = false
        });
    }


    public static void spawnEnemyUnit(UnitType unitType, int gridPosX, int gridPosY)
    {
        EntityManager entityManager = World.Active.EntityManager;

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
        Sprite tileSprite = Resources.Load<Sprite>("Sprites/Animation/" + getUnitSprite(unitType));
        mat.mainTexture = tileSprite.texture;

        float ratio = (float) tileSprite.texture.height / (float) tileSprite.texture.width;
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
        float3 spawnPos = new float3(gridPosX, gridPosY, -2);
        entityManager.SetComponentData(entity, new Translation { Value = spawnPos });
        entityManager.SetSharedComponentData(entity, new RenderMesh
        {
            mesh = mesh,
            material = mat,
        });

        // Set unit component
        entityManager.SetComponentData(entity, new UnitComponent
        {
            unitType = unitType
        });

        // Set animation
        entityManager.SetComponentData(entity, new AnimationComponent
        {
            direction = UnitDirection.RIGHT,
            animationType = UnitAnimation.IDLE,
            currentFrame = 0,
            frameCount = idleAnimations[unitType].Length,
            frameTimer = 0f,
            frameTimerMax = 0.3f
        });

        // Set unit team
        entityManager.SetComponentData(entity, new TeamComponent
        {
            number = 1
        });

        // Set unit health
        entityManager.SetComponentData(entity, new HealthComponent
        {
            health = 100,
            bar = false

        });
        //Set FightComponent
        entityManager.SetComponentData(entity, new FightComponent
        {
            isFighting = false,
            hasToMove =false
        });

    }


    public static Entity spawnHealthBar(UnitType unitType, float gridPosX, float gridPosY, Entity unit)
    {
        EntityManager entityManager = World.Active.EntityManager;

        EntityArchetype entityArchetype = entityManager.CreateArchetype(
            typeof(Translation),
            typeof(UnitComponent),
            typeof(RenderMesh),
            typeof(RenderBounds),
            typeof(LocalToWorld),
            typeof(HealthBarComponent)
        );

        Material mat = new Material(Shader.Find("Unlit/Transparent"));
        Sprite tileSprite = Resources.Load<Sprite>("Sprites/Animation/" + getUnitSprite(unitType));
        mat.mainTexture = tileSprite.texture;

        int width = 1;
        float height = 1;
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
        float3 spawnPos = new float3(gridPosX, gridPosY, -3);
        entityManager.SetComponentData(entity, new Translation { Value = spawnPos });
        entityManager.SetSharedComponentData(entity, new RenderMesh
        {
            mesh = mesh,
            material = mat,
        });

        // Set unit component
        entityManager.SetComponentData(entity, new UnitComponent
        {
            unitType = unitType
        });

        //Set healthbar its unit
        entityManager.SetComponentData(entity, new HealthBarComponent
        {
            soldier = unit
        });

        return entity;
    }
}

