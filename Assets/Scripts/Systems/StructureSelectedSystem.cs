using UnityEngine;
using Unity.Entities;


public class StructureSelectedSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        // get all structures that have received the EntitySelectedComponent tag
        Entities.WithAll<EntitySelectedComponent>().ForEach((Entity entity, ref StructureComponent structure) =>
        {
            // add the StructureSelectedComponent to the first found structure
            PostUpdateCommands.AddComponent(entity, new StructureSelectedComponent());
            
            // show the structures overlay in UI
            UI.instance.showStructureOverlay(structure.type);
            
            // just take the first one to be assured that only one structure is selected
            return; 
        });
    }
}