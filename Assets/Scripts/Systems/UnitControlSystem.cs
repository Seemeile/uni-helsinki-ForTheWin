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

        if (Input.GetMouseButtonDown(1))
        {
            //Right mouse button down
            float3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //Checking wether the target is a ressource or not
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
            }

            else
            {
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

        }
    }

    //Functions which generate the circle positions of the units

    private List<float3> GetPositionListAround(float3 startPosition, float[] ringDistance, int[] ringPositionCount)
    {
        List<float3> positionList = new List<float3>();
        positionList.Add(startPosition);
        for(int ring = 0; ring < ringPositionCount.Length; ring++)
        {
            List<float3> ringPositionList = GetPositionListAround(startPosition, ringDistance[ring], ringPositionCount[ring]);
            positionList.AddRange(ringPositionList);
        }
        return positionList;
    }
    private List<float3> GetPositionListAround(float3 startPosition, float distance, int positionCount)
    {
        List<float3> positionList = new List<float3>();
        for (int i=0; i < positionCount; i++)
        {
            int angle = i * (360 / positionCount);
            float3 dir = ApplyRotationToVector(new float3(0, 1, 0), angle);
            float3 position = startPosition + dir * distance;
            positionList.Add(position);;
        }
        return positionList;
    }

    private float3 ApplyRotationToVector (float3 vec, float angle)
    {
        return Quaternion.Euler(0 , 0 , angle) * vec;
    }

    //Fucntion which dectect if the mouse is pointing at a ressource

    public bool IsHarvestable(float3 currentMousePosition, float distanceMini)
    {
        bool harvestable = false;
        Entities.WithAll<HarvestableComponent>().ForEach((Entity entity, ref Translation translation) =>
        {
            if (math.distance(currentMousePosition.x, translation.Value.x) < distanceMini && (math.distance(currentMousePosition.y, translation.Value.y) < distanceMini))
            {
                harvestable = true;
            }
        });
        return harvestable;
    }
    //Fucntion wich returns the position of the ressource

    public float3 RessourcePosition(float3 currentMousePosition, float distanceMini)
    {
        float3 ressourcePosition = Vector3.zero;
        Entities.WithAll<HarvestableComponent>().ForEach((Entity entity, ref Translation translation) =>
        {
            if (math.distance(currentMousePosition.x, translation.Value.x) < distanceMini && (math.distance(currentMousePosition.y, translation.Value.y) < distanceMini))
            {
                ressourcePosition = translation.Value;
            }
        });
        return ressourcePosition;
    }
}
