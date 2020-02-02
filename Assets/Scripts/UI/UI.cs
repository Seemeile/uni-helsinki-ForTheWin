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

    public void showStructureOverlay(BuildingType structureType) 
    {
        // set icon to building sprite
        GameObject icon = structureOverlay.transform.Find("SelectionIcon").gameObject;
        SpriteRenderer iconRenderer = icon.GetComponent<SpriteRenderer>();
        int tileNo = BuildingData.getTileNumber(structureType);
        Sprite structureSprite = Resources.Load<Sprite>("Sprites/TileSprites/tileset16x16_1_" + tileNo);
        iconRenderer.sprite = structureSprite;

        // show buildable units
        UnitType[] buildableUnits = BuildingData.getBuildableUnits(structureType);
        for (int i = 0; i < 3; i++) 
        {
            // reset
            GameObject unitSlot = structureOverlay.transform.Find("UnitSlot" + (i + 1)).gameObject;
            SpriteRenderer unitSlotRenderer = unitSlot.GetComponent<SpriteRenderer>();
            unitSlotRenderer.sprite = null;
            // set unit icon
            if (i < buildableUnits.Length)
            {
                string spriteName = UnitData.getUnitSprite(buildableUnits[i]);
                Sprite unitSlotSprite = Resources.Load<Sprite>("Sprites/Animation/" + spriteName);
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
