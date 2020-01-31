using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;

public class UnitSelectedRenderer : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<UnitSelectedComponent>().ForEach((ref Translation translation) =>
        {
            float3 position = translation.Value + new float3(0, -3f, +1);
            Graphics.DrawMesh(
                GameHandler.instance.unitSelectedCircleMesh,
                translation.Value, Quaternion.identity,
                GameHandler.instance.unitSelectedCircleMaterial,
                0
                );
        });
    }
}