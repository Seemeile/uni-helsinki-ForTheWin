using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static UI instance;

    private GameObject actionsOverlay;
    private GameObject canvas;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        actionsOverlay = transform.Find("ActionsOverlay").gameObject;
        canvas = transform.Find("GameOverlay").Find("Canvas").gameObject;
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    public void showUnitActions(UnitType unitType)
    {
        // set icon to building sprite

        GameObject icon = actionsOverlay.transform.Find("SelectionIcon").gameObject;
        SpriteRenderer iconRenderer = icon.GetComponent<SpriteRenderer>();
        string spriteName = UnitData.getUnitSprite(unitType);
        Sprite unitSprite = Resources.Load<Sprite>("Sprites/Animation/" + spriteName);
        iconRenderer.sprite = unitSprite;
        //iconRenderer.gameObject.transform.localScale = new Vector3(1, 1, 0);
        
        // show buildable units
        BuildingType[] unitActions = UnitData.getUnitActions(unitType);
        for (int i = 0; i < 3; i++) 
        {
            // reset
            GameObject unitSlot = actionsOverlay.transform.Find("Slot" + (i + 1)).gameObject;
            SpriteRenderer unitSlotRenderer = unitSlot.GetComponent<SpriteRenderer>();
            //unitSlotRenderer.gameObject.transform.localScale = new Vector3(6, 6, 0);
            unitSlotRenderer.sprite = null;
            // set unit icon
            if (i < unitActions.Length)
            {
                int tileNo = BuildingData.getTileNumber(unitActions[i]);
                Sprite unitSlotSprite = Resources.Load<Sprite>("Sprites/TileSprites/tileset16x16_1_" + tileNo);
                unitSlotRenderer.sprite = unitSlotSprite;
            }
        }
        actionsOverlay.SetActive(true);
    }

    public void showStructureActions(BuildingType structureType) 
    {
        // set icon to building sprite
        GameObject icon = actionsOverlay.transform.Find("SelectionIcon").gameObject;
        SpriteRenderer iconRenderer = icon.GetComponent<SpriteRenderer>();
        int tileNo = BuildingData.getTileNumber(structureType);
        Sprite structureSprite = Resources.Load<Sprite>("Sprites/TileSprites/tileset16x16_1_" + tileNo);
        iconRenderer.sprite = structureSprite;
        //iconRenderer.gameObject.transform.localScale = new Vector3(6, 6, 0);

        // show buildable units
        UnitType[] buildableUnits = BuildingData.getBuildableUnits(structureType);
        for (int i = 0; i < 3; i++) 
        {
            // reset
            GameObject unitSlot = actionsOverlay.transform.Find("Slot" + (i + 1)).gameObject;
            SpriteRenderer unitSlotRenderer = unitSlot.GetComponent<SpriteRenderer>();
            //unitSlotRenderer.gameObject.transform.localScale = new Vector3(1, 1, 0);
            unitSlotRenderer.sprite = null;
            // set unit icon
            if (i < buildableUnits.Length)
            {
                string spriteName = UnitData.getUnitSprite(buildableUnits[i]);
                Sprite unitSlotSprite = Resources.Load<Sprite>("Sprites/Animation/" + spriteName);
                unitSlotRenderer.sprite = unitSlotSprite;
            }
        }
        actionsOverlay.SetActive(true);   
    }

    public void hideActionsOverlay() 
    {
        actionsOverlay.SetActive(false);   
    }

    public void setWoodAmount(int amount)
    {
        GameObject woodObject = canvas.transform.Find("WoodAmount").gameObject;
        Text wood = woodObject.GetComponent<Text>();
        wood.text = "" + amount;
    }

    public void setGoldAmount(int amount)
    {
        GameObject goldObject = canvas.transform.Find("GoldAmount").gameObject;
        Text gold = goldObject.GetComponent<Text>();
        gold.text = "" + amount;
    }

    public void addWood(int amount)
    {
        int newWood = getWoodAmount() + amount;
        setWoodAmount(newWood);
    }

    public void subWood(int amount)
    {
        int newWood = getWoodAmount() - amount;
        setWoodAmount(newWood);
    }

    public void addGold(int amount)
    {
        int newGold = getGoldAmount() + amount;
        setGoldAmount(newGold);
    }

    public void subGold(int amount)
    {
        int newGold = getGoldAmount() - amount;
        setGoldAmount(newGold);
    }

    public int getWoodAmount()
    {
        GameObject woodObject = canvas.transform.Find("WoodAmount").gameObject;
        Text wood = woodObject.GetComponent<Text>();
        return int.Parse(wood.text);
    }

    public int getGoldAmount()
    {
        GameObject goldObject = canvas.transform.Find("GoldAmount").gameObject;
        Text gold = goldObject.GetComponent<Text>();
        return int.Parse(gold.text);
    }
}
