using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class SlotScript : MonoBehaviour
{    
    public int slotNumber;
    public string spriteName;
    private string currentToolTipText = "";
    private GUIStyle guiStyleFore;

    void Start()
    {
        guiStyleFore = new GUIStyle();
        guiStyleFore.normal.textColor = Color.black;  
        guiStyleFore.normal.background = makeHoverBackground();
        guiStyleFore.fontSize = 20;
        guiStyleFore.alignment = TextAnchor.UpperCenter;
        guiStyleFore.wordWrap = true;
    }
    
    void OnMouseEnter ()
    {
        if (!spriteName.Equals(""))
        {
            int[] costs = UnitData.getUnitCostsByName(spriteName);
            if (costs.Length > 0)
            {
                makeHoverText(costs);
                return;
            }
            costs = BuildingData.getBuildingCostsByName(spriteName);
            if (costs.Length > 0)
            {
                makeHoverText(costs);
            }
        }
    }
    
    void OnMouseExit ()
    {
        currentToolTipText = "";
    }
    
    void OnGUI()
    {
        if (currentToolTipText != "")
        {
            var x = Event.current.mousePosition.x;
            var y = Event.current.mousePosition.y;
            GUI.Label(new Rect(x - 150, y - 40, 150, 45), currentToolTipText, guiStyleFore);
        }
    }

    void OnMouseDown()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr.sprite)
        {
            EntityManager entityManager = World.Active.EntityManager;
            Entity entity = entityManager.CreateEntity(typeof(SlotClickedEvent));
            entityManager.SetComponentData(entity, new SlotClickedEvent {
                slotNumber = slotNumber
            });
            OnMouseExit();
        }
    }

    private string makeHoverText(int[] costs)
    {
        currentToolTipText = "Gold: ";
        currentToolTipText += "" + costs[0];
        for (int i = 0; i < (4 - costs[0].ToString().Length); i++) {
            currentToolTipText += " ";
        }
        currentToolTipText += " Wood: " + costs[1];
        return currentToolTipText;
    }

    private Texture2D makeHoverBackground() 
    {
        Color[] pix = new Color[150 * 45];
        for (int i = 0; i < pix.Length; i++) 
        {
            pix[i] = new Color(0.949f, 0.890f, 0.761f, 1);
        }
        var result = new Texture2D(150, 45);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }

}
