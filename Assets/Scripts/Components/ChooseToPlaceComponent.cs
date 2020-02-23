using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Jobs;

public struct ChooseToPlaceComponent : IComponentData
{
    public BuildingType buildingType;
}

