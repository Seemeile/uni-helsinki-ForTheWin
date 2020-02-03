using UnityEngine;
using Unity.Entities;

public struct StructureComponent : IComponentData 
{
    public BuildingType type;
}