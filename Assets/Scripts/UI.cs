using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    public static UI instance;

    private GameObject structureOverlay;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        structureOverlay = transform.Find("StructureOverlay").gameObject;
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    public void showStructureOverlay(int structureNumber) 
    {
        // set icon to building sprite
        GameObject icon = structureOverlay.transform.Find("SelectionIcon").gameObject;
        SpriteRenderer iconRenderer = icon.GetComponent<SpriteRenderer>();
        Sprite structureSprite = Resources.Load<Sprite>("Sprites/TileSprites/tileset16x16_1_" + structureNumber);
        iconRenderer.sprite = structureSprite;

        // show buildable units
        string[] buildableUnits = BuildingData.getBuildableUnits(structureNumber);
        for (int i = 0; i < 3; i++) 
        {
            // reset
            GameObject unitSlot = structureOverlay.transform.Find("UnitSlot" + (i + 1)).gameObject;
            SpriteRenderer unitSlotRenderer = unitSlot.GetComponent<SpriteRenderer>();
            unitSlotRenderer.sprite = null;
            // set unit icon
            if (i < buildableUnits.Length)
            {
                Sprite unitSlotSprite = Resources.Load<Sprite>("Sprites/Animation/" + buildableUnits[i]);
                unitSlotRenderer.sprite = unitSlotSprite;
            }
        }
        structureOverlay.SetActive(true);   
    }

    public void hideStructureOverlay() 
    {
        structureOverlay.SetActive(false);   
    }
}
