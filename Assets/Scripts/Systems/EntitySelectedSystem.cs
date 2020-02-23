using UnityEngine;
using Unity.Entities;


public class EntitySelectedSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<EntitySelectedComponent>().ForEach((Entity entity) =>
        {
            if (EntityManager.HasComponent<StructureComponent>(entity)) 
            {
                StructureComponent structure = EntityManager.GetComponentData<StructureComponent>(entity);
                UI.instance.showStructureActions(structure.type);
            } 
            else if (EntityManager.HasComponent<UnitComponent>(entity)) 
            {
                UnitComponent unit = EntityManager.GetComponentData<UnitComponent>(entity);
                UI.instance.showUnitActions(unit.unitType);
            }
            // just take the first one to be assured that only one is selected
            return; 
        });
    }
}