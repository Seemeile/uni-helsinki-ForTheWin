using UnityEngine;
using Unity.Entities;

public struct HarvestableComponent : IComponentData 
{
    public HarvestableType type;
    public int ressourceAmount;
}