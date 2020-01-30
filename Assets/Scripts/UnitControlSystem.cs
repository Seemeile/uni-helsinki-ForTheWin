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

    protected override void OnUpdate()
    {

        if (Input.GetMouseButtonDown(0))
        { //Mouse Pressed
            
            GameHandler.instance.selectionAreaTransform.gameObject.SetActive(true);
            startPosition = Camera.main.WorldToScreenPoint(Input.mousePosition);
            GameHandler.instance.selectionAreaTransform.position = startPosition;
            Debug.Log(startPosition);
        }

        if(Input.GetMouseButton(0))
        {
            //Mouse Held Down
            selectionAreaSize.x = Input.mousePosition.x - startPosition.x;
            selectionAreaSize.y = Input.mousePosition.y - startPosition.y;
            GameHandler.instance.selectionAreaTransform.localScale = selectionAreaSize;
        }
        if (Input.GetMouseButtonUp(0))
        { //Mouse Released
            endPosition = Camera.main.WorldToScreenPoint(Input.mousePosition);

            float3 lowerLeftPosition = new float3(math.min(startPosition.x, endPosition.x), math.min(startPosition.y, endPosition.y), 0);
            float3 upperRightPosition = new float3(math.max(startPosition.x, endPosition.x), math.max(startPosition.y, endPosition.y), 0);

            Entities.ForEach((Entity entity, ref Translation translation) => {
                float3 entityPosition = translation.Value;
                if (entityPosition.x >= lowerLeftPosition.x && 
                    entityPosition.y >= lowerLeftPosition.y &&
                    entityPosition.x <= upperRightPosition.x &&
                    entityPosition.y <= upperRightPosition.y)
                {
                    Debug.Log("ok");
                }
            });
        }
    }
}

