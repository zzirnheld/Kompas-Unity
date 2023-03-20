
using System.IO;
using System.Linq;
using UnityEditor;
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

    public CardRepository cardRepository;

    private static readonly string cardTexturesPath = Path.Join("Assets", "Card Textures");

    public void Generate()
    {
        // Uncomment to regen textures...
        Debug.Log("Generating...");
        CardRepository.Init();
        foreach (var card in CardRepository.SerializableCards)
        {
            var fileName = CardRepository.FileNameFor(card.cardName);
            var cardName = card.cardName;
            var sprite = CardRepository.LoadSprite(fileName);
            if (sprite == null) continue;
            Set(card.cardType == 'C', true, sprite);

            var dir = fileName.Split(Path.DirectorySeparatorChar);
            var dirName = string.Join(Path.AltDirectorySeparatorChar, dir.Take(dir.Length - 1));
            Directory.CreateDirectory($"Assets/Resources/Card Textures/Zoomed/Texture/{dirName}");
            Directory.CreateDirectory($"Assets/Resources/Card Textures/Zoomed/Metalness/{dirName}");
            Directory.CreateDirectory($"Assets/Resources/Card Textures/Unzoomed/Texture/{dirName}");
            Directory.CreateDirectory($"Assets/Resources/Card Textures/Unzoomed/Metalness/{dirName}");

            var zoomedTextureName = $"Assets/Resources/Card Textures/Zoomed/Texture/{fileName}.asset";
            var zoomedMetalnessName = $"Assets/Resources/Card Textures/Zoomed/Metalness/{fileName}.asset";
            var unzoomedTextureName = $"Assets/Resources/Card Textures/Unzoomed/Texture/{fileName}.asset";
            var unzoomedMetalnessName = $"Assets/Resources/Card Textures/Unzoomed/Metalness/{fileName}.asset";

            var width = ZoomedTex.width / 3;
            var height = ZoomedTex.height / 2;
            var zoomedTexPortion = new Texture2D(width, height);
            zoomedTexPortion.SetPixels(0, 0, width, height, ZoomedTex.GetPixels(width, height - 1, width, height));
            var zoomedMetPortion = new Texture2D(width, height);
            zoomedMetPortion.SetPixels(0, 0, width, height, ZoomedMet.GetPixels(width, height - 1, width, height));
            var unzoomedTexPortion = new Texture2D(width, height);
            unzoomedTexPortion.SetPixels(0, 0, width, height, UnzoomedTex.GetPixels(width, height - 1, width, height));
            var unzoomedMetPortion = new Texture2D(width, height);
            unzoomedMetPortion.SetPixels(0, 0, width, height, UnzoomedMet.GetPixels(width, height - 1, width, height));

            AssetDatabase.CreateAsset(zoomedTexPortion, zoomedTextureName);
            AssetDatabase.CreateAsset(zoomedMetPortion, zoomedMetalnessName);
            AssetDatabase.CreateAsset(unzoomedTexPortion, unzoomedTextureName);
            AssetDatabase.CreateAsset(unzoomedMetPortion, unzoomedMetalnessName);
            //break;
        }
    }
    public void Set(bool isChar, bool zoomed, Sprite sprite)
    {
        var start = System.DateTime.Now;
        //If cardArt is null, don't regen texture
            //Debug.Log($"Loading fresh {Path.Combine(ZoomedTextureFolder, $"{cardFileName}")}");
        (ZoomedTex, ZoomedMet) = Copy(isChar ? zoomedCharTex : zoomedSpellTex, isChar ? zoomedCharMetalness : zoomedSpellMetalness, true, sprite);
        (UnzoomedTex, UnzoomedMet) = Copy(isChar ? unzoomedCharTex : unzoomedSpellTex, isChar ? unzoomedCharMetalness : unzoomedSpellMetalness, false, sprite);

        //Debug.Log($"Regen took {System.DateTime.Now - start} with cardArt {cardfi != default}?");
        Set(isChar, zoomed, zoomed ? ZoomedTex : UnzoomedTex, zoomed ? ZoomedMet : UnzoomedMet);
    }

    private const string ZoomedTextureFolder = "Assets/Resources/Card Textures/Zoomed/Texture";
    private const string ZoomedMetalnessFolder = "Assets/Resources/Card Textures/Zoomed/Metalness";
    private const string UnzoomedTextureFolder = "Assets/Resources/Card Textures/Unzoomed/Texture";
    private const string UnzoomedMetalnessFolder = "Assets/Resources/Card Textures/Unzoomed/Metalness";

    public void Set(bool isChar, bool zoomed, string cardFileName)
    {
        var start = System.DateTime.Now;
        //If cardArt is null, don't regen texture
        if (ZoomedTex == default)
        {
            var zoomedTex = AssetDatabase.LoadAssetAtPath(Path.Combine(ZoomedTextureFolder, $"{cardFileName}.asset"), typeof(Texture2D)) as Texture2D;
            Debug.Log($"Loading fresh {Path.Combine(ZoomedTextureFolder, $"{cardFileName}.asset")}. Is it null? {zoomedTex == null}");
            var zoomedMet = AssetDatabase.LoadAssetAtPath(Path.Combine(ZoomedMetalnessFolder, $"{cardFileName}.asset"), typeof(Texture2D)) as Texture2D;
            var unzoomedTex = AssetDatabase.LoadAssetAtPath(Path.Combine(UnzoomedTextureFolder, $"{cardFileName}.asset"), typeof(Texture2D)) as Texture2D;
            var unzoomedMet = AssetDatabase.LoadAssetAtPath(Path.Combine(UnzoomedMetalnessFolder, $"{cardFileName}.asset"), typeof(Texture2D)) as Texture2D;

            ZoomedTex = Copy(isChar ? zoomedCharTex : zoomedSpellTex, zoomedTex);
            ZoomedMet = Copy(isChar ? zoomedCharMetalness : zoomedSpellMetalness, zoomedMet);
            UnzoomedTex = Copy(isChar ? unzoomedCharTex : unzoomedSpellTex, unzoomedTex);
            UnzoomedMet = Copy(isChar ? unzoomedCharMetalness : unzoomedSpellMetalness, unzoomedMet);
        }

        //Debug.Log($"Regen took {System.DateTime.Now - start} with cardArt {cardfi != default}?");
        Set(isChar, zoomed, zoomed ? ZoomedTex : UnzoomedTex, zoomed ? ZoomedMet : UnzoomedMet);
    }

    private Texture2D Copy(Texture2D baseTexture, Texture2D cardSpecificTexture)
    {
        var texture = new Texture2D(baseTexture.width, baseTexture.height);
        texture.SetPixels(0, 0, texture.width, texture.height, baseTexture.GetPixels(0, 0, baseTexture.width, baseTexture.height));
        var width = cardSpecificTexture.width;
        var height = cardSpecificTexture.height;
        texture.SetPixels(width, height - 1, width, height, cardSpecificTexture.GetPixels(0, 0, width, height));
        texture.Apply();
        return texture;
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

    //TODO: refactor this method to copy the section of the texture that has the card face, and remove all the other redundant stuff from the saved textures
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