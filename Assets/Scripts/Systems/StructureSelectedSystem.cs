using UnityEngine;
using Unity.Entities;


public class StructureSelectedSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        handleStructureUnselect();
        handleStructureSelect();
    }

    private void handleStructureSelect()
    {
        // get all structures that have received the EntitySelectedComponent tag but are not marked for Unselect
        Entities.WithAll<EntitySelectedComponent>().WithNone<EntityUnselectedComponent>()
            .ForEach((Entity entity, ref StructureComponent structure) =>
        {
            Debug.Log("selecting " + entity.Index);
            // add the StructureSelectedComponent to the first found structure
            PostUpdateCommands.AddComponent(entity, new StructureSelectedComponent());
            
            // show the structures overlay in UI
            UI.instance.showStructureOverlay(structure.type);
            
            // just take the first one to be assured that only one structure is selected
            return; 
        });
    }

    private void handleStructureUnselect()
    {
        // get all structures that received an EntityUnselectedComponent tag
        Entities.WithAll<StructureComponent, EntityUnselectedComponent>().ForEach((Entity entity) => 
        {
            Debug.Log("unselecting " + entity.Index);
            // handle the unselection of the previous selected structure
            Entities.WithAll<StructureSelectedComponent>().ForEach((Entity buildingEntity) => 
            {
                PostUpdateCommands.RemoveComponent<StructureSelectedComponent>(buildingEntity);
                UI.instance.hideStructureOverlay();
            });

            // remove EntityUnselectedComponent and EntitySelectedComponent tags
            PostUpdateCommands.RemoveComponent<EntitySelectedComponent>(entity);
            PostUpdateCommands.RemoveComponent<EntityUnselectedComponent>(entity);
        });
    }
}