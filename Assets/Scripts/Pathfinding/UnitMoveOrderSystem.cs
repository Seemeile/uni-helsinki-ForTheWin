using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

public class UnitMoveOrderSystem : ComponentSystem
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
            handleLeftMousePressed();
        }
        if (Input.GetMouseButton(0))
        {
            handleLeftMouseHeldDown();
        }
        if (Input.GetMouseButtonUp(0))
        {
            handleLeftMouseReleased();
        }
        if (Input.GetMouseButtonDown(1))
        {
            handleRightMousePressed();
        }
    }

    private void handleLeftMousePressed() 
    {
        GameHandler.instance.selectionAreaTransform.gameObject.SetActive(true);
        currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        startPosition.x = currentMousePosition.x;
        startPosition.y = currentMousePosition.y;
        GameHandler.instance.selectionAreaTransform.position = startPosition;
    }

    private void handleLeftMouseHeldDown()
    {
        currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        selectionAreaSize.x = currentMousePosition.x - startPosition.x;
        selectionAreaSize.y = currentMousePosition.y - startPosition.y;
        GameHandler.instance.selectionAreaTransform.localScale = selectionAreaSize;
    }

    private void handleLeftMouseReleased()
    {
        // Deselect all previous selected entities
        Entities.WithAll<EntitySelectedComponent>().ForEach((Entity entity) =>
        {
            PostUpdateCommands.RemoveComponent<EntitySelectedComponent>(entity);   
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
                    PostUpdateCommands.AddComponent(entity, new EntitySelectedComponent());
                    selectEntityCount++;
                }
            }
        });
    }

    private void handleRightMousePressed()
    {
        //Right mouse button down
        float3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int targetCellPosition = GameHandler.instance.pathfindingTest.WorldToCell(targetPosition);

        Entities.WithAll<EntitySelectedComponent, UnitComponent>().ForEach((Entity entity, ref Translation translation) => 
        {
            Vector3Int currentCellPosition = GameHandler.instance.pathfindingTest.WorldToCell(translation.Value);
            
            Debug.Log("start path from " + currentCellPosition.x + "," + currentCellPosition.y + " to " 
                + targetCellPosition.x + "," + targetCellPosition.y);

            EntityManager.AddComponentData(entity, new PathfindingParamsComponent {
                startPosition = new int2(currentCellPosition.x, currentCellPosition.y),
                endPosition = new int2(targetCellPosition.x, targetCellPosition.y)
            });
            EntityManager.AddBuffer<PathPosition>(entity);
        });

        //Checking wether the target is a ressource or not
        /*
        if (IsHarvestable(targetPosition, 0.3f))
        {
            //if the target is a ressource, then only one entity will go to harvest it
            int entityCount = 0;
            Entities.WithAll<EntitySelectedComponent>().ForEach((Entity entity, ref MoveToComponent moveTo) =>
            {
                if (entityCount == 0)
                {
                    float3 ressourcePosition = RessourcePosition(targetPosition,0.3f);
                    ressourcePosition.x = ressourcePosition.x - 0.5f;
                    ressourcePosition.y = ressourcePosition.y + 0.5f;
                    moveTo.position = ressourcePosition;
                    moveTo.move = true;
                    //The units start harvesting and becomes unselected
                    PostUpdateCommands.RemoveComponent<EntitySelectedComponent>(entity);
                    entityCount++;
                }

            });
        } else {
            //If the target is not a ressource, then all the units must go there

            List<float3> movePositionList = GetPositionListAround(targetPosition, new float[] { 1f, 1.9f, 3f }, new int[] { 5, 10, 20 });
            int positionIndex = 0;
            Entities.WithAll<EntitySelectedComponent>().ForEach((Entity entity, ref MoveToComponent moveTo) =>
            {

                moveTo.position = movePositionList[positionIndex];
                positionIndex = (positionIndex + 1) % movePositionList.Count;
                moveTo.move = true;
            });
        }
        */
    }
}