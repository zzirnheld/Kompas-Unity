using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DeckbuilderCard : MonoBehaviour
{
    public string cardName;
    public string effText;
    public string subtypeText;
    public string[] subtypes;

    protected MeshRenderer meshRenderer;
    protected Sprite detailedSprite;
    protected Sprite simpleSprite;

    public void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetInfo(SerializableCard card)
    {
        cardName = card.cardName;
        SetImage(cardName);
        effText = card.effText;
        subtypeText = card.subtypeText;
        subtypes = card.subtypes;
    }

    protected void SetImage(string cardFileName)
    {
        detailedSprite = Resources.Load<Sprite>("Detailed Sprites/" + cardFileName);
        simpleSprite = Resources.Load<Sprite>("Simple Sprites/" + cardFileName);
        //check if either is null. if so, log to debug and return
        if (detailedSprite == null || simpleSprite == null)
        {
            Debug.LogError("Could not find sprite with name " + cardFileName);
            return;
        }
        //set this gameobject's texture to the simple sprite (by default, TODO change on zoom level change)
        Texture2D spriteTexture = simpleSprite.texture;
        //spriteTexture.alphaIsTransparency = true;
        meshRenderer.material.SetTexture("_MainTex", spriteTexture);
        //then make unity know it's a sprite so that it'll make the alpha transparent
        meshRenderer.material.shader = Shader.Find("Sprites/Default");
    }
}
