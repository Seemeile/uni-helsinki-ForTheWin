using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class UnitControlSystem : ComponentSystem
{
    private float3 startPosition;
    private float3 endPosition;
    private float3 selectionAreaSize;
    private float3 currentMousePosition;
    private float3 lowerLeftPosition;
    private float3 upperRightPosition;


    protected override void OnUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Mouse Pressed
            GameHandler.instance.selectionAreaTransform.gameObject.SetActive(true);
            currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            startPosition.x = currentMousePosition.x;
            startPosition.y = currentMousePosition.y;
            GameHandler.instance.selectionAreaTransform.position = startPosition;
        }

        if(Input.GetMouseButton(0))
        {
            //Mouse Held Down
            currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            selectionAreaSize.x = currentMousePosition.x - startPosition.x;
            selectionAreaSize.y = currentMousePosition.y - startPosition.y;
            GameHandler.instance.selectionAreaTransform.localScale = selectionAreaSize;
        }
        if (Input.GetMouseButtonUp(0))
        {
            //Mouse Released
            // Deselect all previous selected entities
            Entities.WithAll<UnitSelectedComponent>().ForEach((Entity entity) =>
            {
                PostUpdateCommands.RemoveComponent<UnitSelectedComponent>(entity);   
            });
            Entities.WithAll<StructureSelectedComponent>().ForEach((Entity entity) => {
                PostUpdateCommands.RemoveComponent<StructureSelectedComponent>(entity);
                UI.instance.hideStructureOverlay();
            });
            GameHandler.instance.selectionAreaTransform.gameObject.SetActive(false);
            currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            endPosition.x = currentMousePosition.x;
            endPosition.y = currentMousePosition.y;

            // Entities detection
            float3 lowerLeftPosition = new float3(math.min(startPosition.x, endPosition.x), math.min(startPosition.y, endPosition.y), 0);
            float3 upperRightPosition = new float3(math.max(startPosition.x, endPosition.x), math.max(startPosition.y, endPosition.y), 0);
            bool selectOnlyOneEntity = false;
            float selectionAreaMinSize = 1f;
            float smallSelectionAreaSize = math.distance(lowerLeftPosition, upperRightPosition);

            if (smallSelectionAreaSize < selectionAreaMinSize)
            {
                //Selection too small, meaning if there is only one click
                lowerLeftPosition += new float3(-1f, -1f, 0f) * (selectionAreaMinSize - selectionAreaSize) * .5f;
                upperRightPosition += new float3(+1f, +1f, 0f) * (selectionAreaMinSize - selectionAreaSize) * .5f;
                selectOnlyOneEntity = true;
            }
            //Selection
            int selectEntityCount = 0;
            Entities.ForEach((Entity entity, ref Translation translation) => {
                if (selectOnlyOneEntity == false || selectEntityCount < 1)
                {
                    float3 entityPosition = translation.Value;

                    if (entityPosition.x >= lowerLeftPosition.x &&
                        entityPosition.y >= lowerLeftPosition.y &&
                        entityPosition.x <= upperRightPosition.x &&
                        entityPosition.y <= upperRightPosition.y)
                    {
                        //Entity inside the selection area
                        PostUpdateCommands.AddComponent(entity, new UnitSelectedComponent());
                        selectEntityCount++;
                    }
                }
            });
        }

        if (Input.GetMouseButtonDown(1))
        {
            //Right mouse button down
            Entities.WithAll<UnitSelectedComponent>().ForEach((Entity entity, ref MoveToComponent moveTo) =>
            {
                moveTo.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                moveTo.move = true;
            });

        }
    }
}
