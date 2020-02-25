using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using System;
using UnityEngine.Tilemaps;
using Unity.Collections;

public class GameHandler : MonoBehaviour
{
    public static GameHandler instance;

    public Mesh demonMesh;
    public Material demonMaterial;
    public Transform selectionAreaTransform;
    public Material unitSelectedCircleMaterial;
    public Mesh unitSelectedCircleMesh;
    public Material enemyUnitCircleMaterial;
    public Mesh enemyUnitCircleMesh;
    public Tilemap tilemap;
    public Mesh axeMesh;
    public Material axeMaterial;
    public Mesh pickAxeMesh;
    public Material pickAxeMaterial;
    public Mesh swordMesh;
    public Material swordMaterial;

    [HideInInspector]
    public int tilemapSizeX;
    [HideInInspector]
    public int tilemapSizeY;
    [HideInInspector]
    public int[] tilemapCellBoundsX;
    [HideInInspector]
    public int[] tilemapCellBoundsY;

    public static bool hasToDelete;
    public static bool hasToLoad;

    private void Awake()
    {
        instance = this;
        tilemapSizeX = tilemap.size.x;
        tilemapSizeY = tilemap.size.y;
        tilemapCellBoundsX = new int[]{ tilemap.cellBounds.xMin, tilemap.cellBounds.xMax };
        tilemapCellBoundsY = new int[]{ tilemap.cellBounds.yMin, tilemap.cellBounds.yMax };
    }

    private void Start()
    {
        /*
        if(hasToLoad)
        {
            SaveSystem.loading = true;
            hasToLoad = false;
        }
        if (SettingScript.newGame)
        {
                */
            UnitData.spawnUnit(UnitType.WIZARD, 0, 0);
            UnitData.spawnEnemyUnit(UnitType.KNIGHT, -8, 2);
            UnitData.spawnEnemyUnit(UnitType.KNIGHT, -7, 5);
        //}
    }
}


