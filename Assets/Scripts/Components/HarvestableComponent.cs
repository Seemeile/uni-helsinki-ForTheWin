using UnityEngine;
using Unity.Entities;

public struct HarvestableComponent : IComponentData 
{
    public HarvestableType type;
    public float harvestTime;
    public int ressourceAmount;
}