using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Jobs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//Unit go to Move Position
public class UnitMoveSystem : MonoBehaviour
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
                    float3 moveDir = math.normalize(moveTo.position - translation.Value);
                    moveTo.lastMoveDir = moveDir;
                    translation.Value += moveDir * moveTo.moveSpeed * deltaTime;

                }
                else
                {
                    //Already in the right position
                    moveTo.move = false;
                }
            }
        }
    }
}

