using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

public class UnitAnimationSystem : ComponentSystem
{
    protected override void OnUpdate() 
    {
        EntityManager entityManager = World.Active.EntityManager;

        Entities.ForEach((Entity entity, ref UnitComponent unitComponent, ref AnimationComponent animationComponent) => 
        {
            animationComponent.frameTimer += Time.deltaTime;
            while (animationComponent.frameTimer >= animationComponent.frameTimerMax)
            {
                animationComponent.frameTimer -= animationComponent.frameTimerMax;
                animationComponent.currentFrame = (animationComponent.currentFrame + 1) % animationComponent.frameCount;
            }
            string curAnimationName = UnitData.getUnitAnimation(unitComponent.unitType, animationComponent.currentFrame);
            RenderMesh renderer = entityManager.GetSharedComponentData<RenderMesh>(entity);
            Sprite curAnimationSprite = Resources.Load<Sprite>("Sprites/Animation/" + curAnimationName);
            renderer.material.mainTexture = curAnimationSprite.texture;
        });
    }
}