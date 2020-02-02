using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class UnitSlotScript : MonoBehaviour
{
    public int slotNumber;

    void OnMouseDown()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr.sprite)
        {
            EntityManager entityManager = World.Active.EntityManager;
            Entity entity = entityManager.CreateEntity(typeof(UnitSlotClickedEvent));
            entityManager.SetComponentData(entity, new UnitSlotClickedEvent {
                slotNumber = slotNumber
            });
        }
    }
}
