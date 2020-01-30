using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class UnitSelectedRenderer : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<UnitSelectedComponent>().ForEach((ref Translation translation) =>
        {
            Graphics.DrawMesh(
                GameHandler.instance.unitSelectedCircleMesh,
                translation.Value, Quaternion.identity,
                GameHandler.instance.unitSelectedCircleMaterial,
                0
                );
        });
    }
}