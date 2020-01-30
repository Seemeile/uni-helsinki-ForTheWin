using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class StructureSelectedSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<UnitSelectedComponent>().ForEach((ref StructureComponent structure) =>
        {
            GameHandler.instance.UI.gameObject.SetActive(true);
        });
    }
}