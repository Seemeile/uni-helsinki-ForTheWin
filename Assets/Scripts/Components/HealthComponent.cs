using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct HealthComponent : IComponentData
{
    public float health;
    public Entity unit;
    public bool bar;
    //public GameObject healthBar;
}
