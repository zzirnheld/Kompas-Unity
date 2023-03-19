
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

    public Texture2D ZoomedTex { get; private set; }
    public Texture2D UnzoomedTex { get; private set; }
    public Texture2D ZoomedMet { get; private set; }
    public Texture2D UnzoomedMet { get; private set; }
    
    public void Set(bool isChar, bool zoomed, Sprite cardArt)
    {
        var start = System.DateTime.Now;
        //If cardArt is null, don't regen texture
        if (cardArt != default)
        {
            (ZoomedTex, ZoomedMet) = Copy(isChar ? zoomedCharTex : zoomedSpellTex, isChar ? zoomedCharMetalness : zoomedSpellMetalness, true, cardArt);
            (UnzoomedTex, UnzoomedMet) = Copy(isChar ? unzoomedCharTex : unzoomedSpellTex, isChar ? unzoomedCharMetalness : unzoomedSpellMetalness, false, cardArt);
        }

        //Debug.Log($"Regen took {System.DateTime.Now - start} with cardArt {cardArt != default}?");
        Set(isChar, zoomed, zoomed ? ZoomedTex : UnzoomedTex, zoomed ? ZoomedMet : UnzoomedMet);
    }

    public void Set(bool isChar, bool zoomed, Texture2D texture, Texture2D metalness)
    {
        var mesh = isChar
            ? zoomed ? zoomedCharMesh : unzoomedCharMesh
            : zoomed ? zoomedSpellMesh : unzoomedSpellMesh;
        Set(mesh, texture, metalness);
    }

    public void Set(Mesh mesh, Texture2D texture, Texture2D metalness)
    {
        meshFilter.mesh = mesh;

        var material = meshRenderer.material;
        material.SetTexture(MainTextureName, texture);
        material.SetTexture(MainMetalnessName, metalness);
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