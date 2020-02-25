using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;

public class UnitSelectedRenderer : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<EntitySelectedComponent>().ForEach((ref Translation translation) =>
        {
            float3 position = translation.Value + new float3(0.5f, 0, 0);
            position.z = 0f;
            Graphics.DrawMesh(
                GameHandler.instance.unitSelectedCircleMesh,
                position, Quaternion.identity,
                GameHandler.instance.unitSelectedCircleMaterial,
                0
                );
        });

        Entities.WithAll<TeamComponent>().ForEach((ref Translation translation, ref TeamComponent team) =>
        {
            if (team.number == 1)
            {
                float3 position = translation.Value + new float3(0.5f, 0, 0);
                position.z = 0f;
                Graphics.DrawMesh(
                    GameHandler.instance.enemyUnitCircleMesh,
                    position, Quaternion.identity,
                    GameHandler.instance.enemyUnitCircleMaterial,
                      0
                      );
            }            
        });
    }
}