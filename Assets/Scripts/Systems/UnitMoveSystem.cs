using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Jobs;
using Unity.Burst;


//Unit go to Move Position
public class UnitMoveSystem : JobComponentSystem
{

    private struct Job : IJobForEachWithEntity<MoveToComponent, Translation>
    {
        public float deltaTime;

        public void Execute(Entity entity, int index, ref MoveToComponent moveTo, ref Translation translation)
        {
            if (moveTo.move)
            {
                float reachedPositionDistance = 1f;
                if (math.distance(translation.Value, moveTo.position) > reachedPositionDistance)
                {
                    // Far from the entity position
                    float z = translation.Value.z;
                    float3 moveDir = math.normalize(moveTo.position - translation.Value);
                    Debug.Log(moveDir);
                    moveTo.lastMoveDir = moveDir;
                    translation.Value += moveDir * moveTo.moveSpeed * deltaTime;
                    translation.Value.z = z;



                }
                else
                {
                    //Already in the right position
                    moveTo.move = false;
                }
            }
        }
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        Job job = new Job
        {
            deltaTime = Time.deltaTime
        };
        return job.Schedule(this, inputDeps);
    }
}
    

