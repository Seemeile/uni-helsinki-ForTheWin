using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class StructureSelectedSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<UnitSelectedComponent>()
            .WithNone<StructureSelectedComponent>()
            .ForEach((Entity entity, ref StructureComponent structure) =>
        {
            PostUpdateCommands.AddComponent(entity, new StructureSelectedComponent());
            UI.instance.showStructureOverlay(structure.tileNo);
            return;
        });
    }
}