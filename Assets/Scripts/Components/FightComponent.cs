using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct FightComponent : IComponentData
{
    public Entity target;
    public bool isFighting;
    public bool hasToMove;
    public bool isMoving;
}
