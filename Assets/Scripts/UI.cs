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
        structureOverlay.SetActive(true);
        GameObject icon = structureOverlay.transform.Find("SelectionIcon").gameObject;
        SpriteRenderer iconRenderer = icon.GetComponent<SpriteRenderer>();
        Sprite structureSprite = Resources.Load<Sprite>("Sprites/TileSprites/tileset16x16_1_" + structureNumber);
        iconRenderer.sprite = structureSprite;
    }

    public void hideStructureOverlay() 
    {
        structureOverlay.SetActive(false);   
    }
}
