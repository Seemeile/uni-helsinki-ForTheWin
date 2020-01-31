using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Jobs;

public struct MoveToComponent : IComponentData
{
    public bool move;
    public float3 position;
    public float3 lastMoveDir;
    public float moveSpeed;
}

