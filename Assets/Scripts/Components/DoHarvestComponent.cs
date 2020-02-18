using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct DoHarvestComponent : IComponentData
{
    public enum STATE {
        INIT, GO, HARVEST, RETURN
    }

    public STATE state;
    public Entity target;
    public Vector3Int targetCell;
    public Entity hq;
    public Vector3Int hqCell;
    public float time;
    public int carryAmount;
    public HarvestableType carryType;
}