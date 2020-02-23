using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class PathFollowSystem : ComponentSystem
{
    protected override void OnUpdate() 
    {
        Entities.ForEach((Entity entity, 
            DynamicBuffer<PathPosition> pathPositionBuffer, 
            ref Translation translation, 
            ref UnitComponent unitComponent, 
            ref AnimationComponent animationComponent) 
        => {
            if (pathPositionBuffer.Length > 0)
            {
                int2 nextGridPosition = pathPositionBuffer[pathPositionBuffer.Length - 1].position;
                float3 targetPosition = new float3(nextGridPosition.x, nextGridPosition.y, -1);

                // check if we're standing on the target
                if (translation.Value.Equals(targetPosition)) {
                    pathPositionBuffer.RemoveAt(pathPositionBuffer.Length - 1);
                    if (pathPositionBuffer.Length == 0) {
                        return;
                    }
                    nextGridPosition = pathPositionBuffer[pathPositionBuffer.Length - 1].position;
                    targetPosition = new float3(nextGridPosition.x, nextGridPosition.y, -1);
                }

                // change animation direction when necessary
                if (translation.Value.x < targetPosition.x) {
                    animationComponent.direction = UnitDirection.RIGHT;
                } else {
                    animationComponent.direction = UnitDirection.LEFT;
                }

                // change to run animation while moving
                if (UnitAnimation.RUN != animationComponent.animationType) {
                    animationComponent.animationType = UnitAnimation.RUN;
                    animationComponent.currentFrame = 0;
                    animationComponent.frameCount = UnitData.getUnitAnimationCount(unitComponent.unitType, UnitAnimation.RUN);
                    animationComponent.frameTimer = 0f;
                    animationComponent.frameTimerMax = 0.1f;
                }

                // calculate move vector and move the unit
                float3 moveDir = math.normalize(targetPosition - translation.Value);
                float moveSpeed = 3f;
                translation.Value += moveDir * moveSpeed * Time.deltaTime;

                // if the unit reaches the destination
                if (math.distance(translation.Value, targetPosition) < .1f) {
                    pathPositionBuffer.RemoveAt(pathPositionBuffer.Length - 1);
                    if(pathPositionBuffer.Length == 0) {
                        // correct the new position of the entity
                        translation.Value = targetPosition;
                        animationComponent.animationType = UnitAnimation.IDLE;
                        animationComponent.currentFrame = 0;
                        animationComponent.frameCount = UnitData.getUnitAnimationCount(unitComponent.unitType, UnitAnimation.IDLE);
                        animationComponent.frameTimer = 0f;
                        animationComponent.frameTimerMax = 0.30f;
                    }
                }                
            }
        });
    }
}