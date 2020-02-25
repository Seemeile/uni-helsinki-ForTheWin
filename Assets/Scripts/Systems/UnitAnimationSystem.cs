using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
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

                // change animation sprite
                string curAnimationName = UnitData.getUnitAnimation(unitComponent.unitType, animationComponent.animationType, animationComponent.currentFrame);
                RenderMesh renderer = entityManager.GetSharedComponentData<RenderMesh>(entity);
                Sprite curAnimationSprite = Resources.Load<Sprite>("Sprites/Animation/" + curAnimationName);
                renderer.material.mainTexture = curAnimationSprite.texture;    

                // flip direction of sprite if the unit direction changed
                if (getUnitDirection(renderer.mesh) != animationComponent.direction) {
                    flipQuadUV(renderer.mesh);
                }
            }
        });
    }

    private UnitDirection getUnitDirection(Mesh mesh) {
        if (mesh.uv[0].x == 1.0f) {
            return UnitDirection.LEFT;
        }
        return UnitDirection.RIGHT;
    }

    private void flipQuadUV(Mesh mesh) {
        var uvs = mesh.uv;
        if (uvs.Length != 4) {
            Debug.LogError("Error: not a four vertices mesh");
            return;
        }
        for (var i = 0; i < uvs.Length; i++) {
            if (Mathf.Approximately(uvs[i].x, 1.0f))
                uvs[i].x = 0.0f;
            else
                uvs[i].x = 1.0f;
        }
        mesh.uv = uvs;
    }
}