using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;

public class SpriteSheetAnimation_Animate : JobComponentSystem
{
    [BurstCompile]
    public struct Job : IJobForEach<SpriteSheetAnimation_Data>
    {
       public float deltaTime;
       public void Execute(ref SpriteSheetAnimation_Data spriteSheetAnimationData)
       {
            spriteSheetAnimationData.frameTimer += deltaTime;
            while (spriteSheetAnimationData.frameTimer >= spriteSheetAnimationData.frameTimerMax)
            {
                spriteSheetAnimationData.frameTimer -= spriteSheetAnimationData.frameTimerMax;
                spriteSheetAnimationData.currentFrame = (spriteSheetAnimationData.currentFrame + 1) % spriteSheetAnimationData.frameCount;
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
