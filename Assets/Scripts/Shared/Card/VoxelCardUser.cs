
using UnityEngine;

public class VoxelCardUser : MonoBehaviour
{
    private const string MainTextureName = "_MainTex";
    private const string MainMetalnessName = "_MetallicGlossMap";
    
    public MeshFilter meshFilter;

    public Mesh zoomedCharMesh;
    public Mesh unzoomedCharMesh;
    public Mesh zoomedSpellMesh;
    public Mesh unzoomedSpellMesh;

    public Texture2D zoomedCharTex;
    public Texture2D unzoomedCharTex;
    public Texture2D zoomedSpellTex;
    public Texture2D unzoomedSpellTex;

    public Texture2D zoomedCharMetalness;
    public Texture2D unzoomedCharMetalness;
    public Texture2D zoomedSpellMetalness;
    public Texture2D unzoomedSpellMetalness;

    public MeshRenderer meshRenderer;

    private Texture2D zoomedTex;
    private Texture2D unzoomedTex;
    private Texture2D zoomedMet;
    private Texture2D unzoomedMet;
    public void Set(bool isChar, bool zoomed, Sprite cardArt)
    {
        var start = System.DateTime.Now;
        meshFilter.mesh = isChar
            ? zoomed ? zoomedCharMesh : unzoomedCharMesh
            : zoomed ? zoomedSpellMesh : unzoomedSpellMesh;
        //If cardArt is null, don't regen texture
        var material = meshRenderer.material;

        if (cardArt != default)
        {
            (zoomedTex, zoomedMet) = Copy(isChar ? zoomedCharTex : zoomedSpellTex, isChar ? zoomedCharMetalness : zoomedSpellMetalness, true, cardArt);
            (unzoomedTex, unzoomedMet) = Copy(isChar ? unzoomedCharTex : unzoomedSpellTex, isChar ? unzoomedCharMetalness : unzoomedSpellMetalness, false, cardArt);
        }

        material.SetTexture(MainTextureName, zoomed ? zoomedTex : unzoomedTex);
        material.SetTexture(MainMetalnessName, zoomed ? zoomedMet : unzoomedMet);
        Debug.Log($"Took {System.DateTime.Now - start} with cardArt {cardArt != default}?");
    }

    private (Texture2D, Texture2D) Copy(Texture2D oldTexture, Texture2D oldMetalness, bool zoomed, Sprite cardArt)
    {
        var texture = new Texture2D(oldTexture.width, oldTexture.height);
        texture.SetPixels(oldTexture.GetPixels());
        texture.Apply();
        var metalness = new Texture2D(oldMetalness.width, oldMetalness.height);
        metalness.SetPixels(oldMetalness.GetPixels());
        metalness.Apply();

        VoxelCard.BuildTextureCardArt(256, !zoomed, 0.0225f, VoxelCard.GetCharacterArtUpperBound(!zoomed), VoxelCard.GetCharacterArtSamplingIncrementRatio(!zoomed), cardArt, 0f, 0f, texture, metalness);
        return (texture, metalness);
    }
}