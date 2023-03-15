
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

    public void Set(bool isChar, bool zoomed, Sprite cardArt)
    {
        meshFilter.mesh = isChar
            ? zoomed ? zoomedCharMesh : unzoomedCharMesh
            : zoomed ? zoomedSpellMesh : unzoomedSpellMesh;
        //If cardArt is null, don't regen texture

        var material = meshRenderer.material;
        var texture = isChar
            ? zoomed ? zoomedCharTex : unzoomedCharTex
            : zoomed ? zoomedSpellTex : unzoomedSpellTex;
        var metalness = isChar
            ? zoomed ? zoomedCharMetalness : unzoomedCharMetalness
            : zoomed ? zoomedSpellMetalness : unzoomedSpellMetalness;

        if (cardArt != default)
            VoxelCard.BuildTextureCardArt(256, !zoomed, 0.0225f, VoxelCard.GetCharacterArtUpperBound(!zoomed), VoxelCard.GetCharacterArtSamplingIncrementRatio(!zoomed), cardArt, 0f, 0f, texture, metalness);
        
        material.SetTexture(MainTextureName, texture);
        material.SetTexture(MainMetalnessName, metalness);
    }
}