using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct HealthComponent : IComponentData
{
    public float health;
    public bool bar;
    public Entity healthBar;
}
