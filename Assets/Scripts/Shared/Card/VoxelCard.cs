using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

public struct MeshData
{
    public Vector3[] Verts;
    public Vector2[] UVs;
    public int[] Tris;
}

public abstract class VoxelCardBase : MonoBehaviour
{
    protected abstract Sprite CardImage { set; }

    protected abstract bool ShowCardStats { set; }

    protected abstract void ApplyUpdates();

    public void Init(bool isChar, Sprite cardImage)
    {
        CardImage = cardImage;
        ShowCardStats = isChar;

        ApplyUpdates();
    }
}

public class VoxelCard : VoxelCardBase
{
    private const float CardBaseThicknessDivisor = 12.0f;

    public const float CharacterArtLowerBoundBase = 0f;//3.0f / 12.0f;
    public const float CharacterArtUpperBoundFullArt = 11.0f / 12.0f;
    public const float CharacterArtUpperBoundHasText = 1f;// 19.0f / 24.0f;

    public const float CharacterArtSamplingIncrementRatioFullArt = 24.0f / 19.0f;
    public const float CharacterArtSamplingIncrementRatioHasText = 1f;//24.0f / 21.0f;

    public const float CharacterArtSamplingStartIndexRatioBase = 1.0f / 17.0f;

    private const string MainTextureName = "_MainTex";
    private const string MainMetalnessName = "_MetallicGlossMap";

    public bool TEST_YES;
    public bool stashMeshes;
    /// <summary>
    /// A temporary measure until I can properly figure out how Antikronus did the bezel stuff, so I can replicate it for the zoomed-out card.
    /// That said, the difference is probably basically imperceptible at the distances you see the zoomed-out model at,
    /// So I'm not going to worry about it much for now.
    /// </summary>
    private const float ExtraZoomOutBezel = 0.05f;

    public bool fullArt;
    public static float CharacterArtLowerBound => CharacterArtLowerBoundBase;
    public static float GetCharacterArtUpperBound(bool fullArt) => fullArt ? CharacterArtUpperBoundFullArt : CharacterArtUpperBoundHasText;
    private float CharacterArtUpperBound => GetCharacterArtUpperBound(fullArt);

    private float CharacterArtSamplingIncrementRatio => GetCharacterArtSamplingIncrementRatio(fullArt);
    public static float GetCharacterArtSamplingIncrementRatio(bool fullArt) => fullArt
        ? CharacterArtSamplingIncrementRatioFullArt
        : CharacterArtSamplingIncrementRatioHasText;
    public static float CharacterArtSamplingStartIndexRatio => CharacterArtSamplingStartIndexRatioBase;

    /// <summary>
    /// Aka 45 degree angle
    /// </summary>
    public static float PI4 = Mathf.PI / 4.0f;
    public Material BaseMaterial;

    public bool RebuildMeshOnChange;
    public bool RebuildTextureOnChange;
    [Range(0.001f, 0.1f)]
    public float frameThickness;
    [Range(0.001f, 0.1f)]
    public float frameThicknessFullArt;
    private float FrameThickness => fullArt ? frameThicknessFullArt : frameThickness;
    [Range(0.1f, 0.7f)]
    public float TypePlacardWidth;

    protected override bool ShowCardStats { set => HasN = HasE = HasW = value; }

    public bool HasN, HasE, HasSAC, HasW, HasR, HasD;
    public int TextureResolution;

    public Sprite FrameTexture;
    [Range(0.0f, 1.0f)]
    public float FrameMetallic;
    [Range(0.0f, 1.0f)]
    public float FrameGloss;

    public Sprite NamePlacardTexture;
    [Range(0.0f, 1.0f)]
    public float NamePlacardMetallic;
    [Range(0.0f, 1.0f)]
    public float NamePlacardGloss;

    public Sprite TypePlacardTexture;
    [Range(0.0f, 1.0f)]
    public float TypePlacardMetallic;
    [Range(0.0f, 1.0f)]
    public float TypePlacardGloss;
    [Range(0.0f, 1.0f)]
    public float TypePlacardHeight = 0.3f;

    public bool ApplyStatColors;
    [Range(0.0f, 1.0f)]
    public float StatsMetallic;
    [Range(0.0f, 1.0f)]
    public float StatsGloss;

    public Sprite NTexture;
    public Color NColor;

    public Sprite ETexture;
    public Color EColor;

    public Sprite SACTexture;
    public Color SACColor;

    public Sprite WTexture;
    public Color WColor;

    public Sprite RTexture;

    public Sprite DTexture;

    protected override Sprite CardImage { set { CharacterArt = value; } }

    public Sprite CharacterArt;
    [Range(0.0f, 1.0f)]
    public float CharacterArtMetallic;
    [Range(0.0f, 1.0f)]
    public float CharacterArtGloss;

    public Sprite EffectTextTexture;
    [Range(0.0f, 1.0f)]
    public float EffectTextMetallic;
    [Range(0.0f, 1.0f)]
    public float EffectTextGloss;

    public Sprite CardBackTexture;
    [Range(0.0f, 1.0f)]
    public float CardBackMetallic;
    [Range(0.0f, 1.0f)]
    public float CardBackGloss;

    private Mesh CardMesh;
    
    private float CardBaseThickness => FrameThickness / CardBaseThicknessDivisor;

    public void OnInspectorChange()
    {
        //var start = System.DateTime.Now;
        if (RebuildMeshOnChange)
        {
            var meshName = $"Assets/{(fullArt ? "ZoomedOut" : "ZoomedIn")}{(HasN ? "Char" : "Spell")}Mesh.asset";
            var meshFilter = GenerateMesh();
            if (stashMeshes) AssetDatabase.CreateAsset(meshFilter.mesh, meshName);
        }
        if (RebuildTextureOnChange)
        {
            var textureName = $"Assets/{(fullArt ? "ZoomedOut" : "ZoomedIn")}{(HasN ? "Char" : "Spell")}Texture.asset";
            var metalnessName = $"Assets/{(fullArt ? "ZoomedOut" : "ZoomedIn")}{(HasN ? "Char" : "Spell")}Metalness.asset";
            var (tex, met) = GenerateTexture();
            if (stashMeshes) AssetDatabase.CreateAsset(tex, textureName);
            if (stashMeshes) AssetDatabase.CreateAsset(met, metalnessName);
        }
        //Debug.Log($"Took {System.DateTime.Now - start}");
    }

    protected override void ApplyUpdates() => Generate();

    public void Generate()
    {
        //var start = System.DateTime.Now;
        GenerateMesh();
        GenerateTexture();
        //List<TextMeshPro> textBoxes = BuildTextBoxes();   //Not Implemented
        //ApplyText(textBoxes);
        //Debug.Log($"Took {System.DateTime.Now - start}");
    }

    public MeshFilter GenerateMesh()
    {
        MeshData newMesh = BuildMesh();
        return ApplyMesh(newMesh);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>Texture, Metalness</returns>
    public (Texture2D, Texture2D) GenerateTexture()
    {
        var start = System.DateTime.Now;
        Material mat = gameObject.GetComponent<MeshRenderer>().material;
        bool rebuild = true;
        if(mat == null)
        {
            mat = new Material(BaseMaterial);
            rebuild = true;
        }
        var newTextures = BuildTexture(mat.GetTexture(MainTextureName) as Texture2D, mat.GetTexture(MainMetalnessName) as Texture2D, MyParams, rebuild);
        ApplyTexture(newTextures, mat);
        Debug.Log($"Took {System.DateTime.Now - start}");
        return newTextures;
    }

    private MeshData BuildMesh()
    {
        List<Vector3> verts = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();
        void addTri(int v1, int v2, int v3) => tris.AddRange(new List<int> { v1, v2, v3 });
        Vector3 findInnerAngle(Vector3 center, Vector3 side1, Vector3 side2)
        {
            Vector3 avg = ((side1 - center).normalized + (side2 - center).normalized) * 0.5f;
            return avg.normalized;
        }
        void addUV(Vector3 point, int texture)
        {
            if (texture == 0) point *= 1.0f / (1.0f + 2.0f * FrameThickness);
            float uvX = (point.x + 1.0f) * 0.5f;
            float uvY = (point.z + 1.0f) * 0.5f;

            uvs.Add(new Vector2((uvX + (texture % 3)) / 3.0f, (uvY + (texture / 3)) / 2.0f));
        }

        (float, float) trig(float angle) => (Mathf.Cos(angle), Mathf.Sin(angle));
        Vector2 trigVector(float angle)
        {
            var (cos, sin) = trig(angle);
            return new Vector2(cos, sin);
        }

        //Build card base

        //Build card base verts counter-clockwise around origin from (1, 0, 0)
        for (int i = 0; i < 8; i++)
        {
            var (cos, sin) = trig(PI4 * i);
            verts.Add(new Vector3(cos, CardBaseThickness, sin)); //front
            verts.Add(new Vector3(cos, -CardBaseThickness, sin)); //back
        }

        //Build card base edge tris counter-clockwise around origin from right, front and back
        for (int i = 0; i < 8; i += 2)
        {
            int v1 = i;
            int v2 = (i + 7) % 8;
            int v3 = (i + 1) % 8;
            addTri(v1 * 2, v2 * 2, v3 * 2); //front face
            addTri(v1 * 2 + 1, v3 * 2 + 1, v2 * 2 + 1); //back face
        }

        //Build card base center tris, front and back
        addTri(14, 06, 02);    //front UR
        addTri(06, 14, 10);    //front LL
        addTri(15, 03, 07);    //back UR
        addTri(07, 11, 15);    //back LL

        //add front verts to front texture, back verts to back texture
        for (int i = 0; i < 16; i += 2)
        {
            addUV(verts[i], 4);
            addUV(verts[i + 1], 5);
        }

        //Build 3/4 of outer frame. Current number of verts is 16.
        int vI = 16;
        //Build modifier vectors, clockwise around origin from (-FrameThickness, 0, 0)
        List<Vector2> modifiers = new List<Vector2>();
        for (int i = 4; i <= 10; i++)
        {
            modifiers.Add(trigVector(PI4 * i) * FrameThickness);
        }

        //Build 3/4 outer frame vertices counter-clockwise around origin from (1, 0, 0)
        for (int i = 0; i < 8; i++)
        {
            foreach (Vector3 mod in modifiers)
            {
                float modX = 1.0f + mod.x;
                var (cos, sin) = trig(PI4 * i);
                verts.Add(new Vector3(cos * modX, mod.y, sin * modX));
            }
        }

        //Build 3/4 outer frame tris. hope you've been following along cause I don't feel like explaining the order.
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < modifiers.Count - 1; j++)
            {
                int v1 = vI + modifiers.Count * i + j;  //sorry the vI and v1 are a little confusing
                int v2 = v1 + 1;
                int v3 = vI + modifiers.Count * ((i + 1) % 8) + j;
                int v4 = v3 + 1;

                addTri(v1, v2, v3);
                addTri(v2, v4, v3);
            }
        }

        //add frame verts to frame texture
        for (int i = vI; i < verts.Count; i++)
        {
            addUV(verts[i], 0);
        }


        vI = verts.Count;
        if (!fullArt)
        {
            vI = CreateNamePlacard(verts, uvs, tris, vI);
            vI = CreateSubtypePlacard(verts, uvs, tris, vI);
        }
        //let's finish rounding out our modifiers, adding a copy of [0] so we don't have to play mod games
        for (int i = 11; i <= 12; i++)
        {
            modifiers.Add(trigVector(PI4 * i) * FrameThickness);
        }

        //Calulate edge points for thick part of frames
        float fullArtProportion = 0.3f;
        float innerEdgeProportion = fullArt ? fullArtProportion : 0.25f;
        Vector3 edgeInnerThick = new Vector3(Mathf.Lerp(verts[2].x, verts[4].x, 1f - innerEdgeProportion), 
            0,
            Mathf.Lerp(verts[2].z, verts[4].z, innerEdgeProportion));
        float outerEdgeProportion = fullArt ? fullArtProportion : 0.25f;
        Vector3 edgeOuterThick = new Vector3(Mathf.Lerp(verts[2].x, verts[4].x, 1f - outerEdgeProportion),
            0,
            Mathf.Lerp(verts[2].z, verts[4].z, 1f - outerEdgeProportion));
        Vector3 edgeThin = new Vector3(Mathf.Lerp(verts[2].x, verts[4].x, outerEdgeProportion), 0, edgeInnerThick.z);

        //Starting with the UR corner at y = 0 and working our way up, going counter-clockwise
        for (int i = 8; i >= 6; i--)
        {
            int vIlocal = verts.Count;
            float modX = 1.0f + modifiers[i].x;
            verts.Add(new Vector3(edgeInnerThick.x, modifiers[i].y, edgeInnerThick.z + modifiers[i].x));
            verts.Add(new Vector3(edgeThin.x * modX, modifiers[i].y, edgeThin.z * modX));
            verts.Add(new Vector3(verts[2].x * modX, modifiers[i].y, verts[2].z * modX));
            verts.Add(new Vector3(verts[vIlocal + 1].z, verts[vIlocal + 1].y,   verts[vIlocal + 1].x));
            verts.Add(new Vector3(verts[vIlocal].z,     verts[vIlocal].y,       verts[vIlocal].x));
        }
        //add the last two vertices
        verts.Add(new Vector3(edgeOuterThick.x, modifiers[6].y, edgeOuterThick.z));
        verts.Add(new Vector3(edgeOuterThick.z, modifiers[6].y, edgeOuterThick.x));

        //and 18 triangles
        for (int i = 0; i < 4; i++)
        {
            int v1 = vI + i;
            int v2 = v1 + 1;
            int v3 = v1 + 5;
            int v4 = v3 + 1;
            int v5 = v3 + 5;
            int v6 = v5 + 1;

            addTri(v1, v3, v2);
            addTri(v3, v4, v2);
            addTri(v3, v5, v4);
            addTri(v5, v6, v4);
        }
        addTri(vI + 10, vI + 15, vI + 11);
        addTri(vI + 14, vI + 13, vI + 16);

        //UL corner inner frame
        //Some more cheating, but reflected across the x axis. Have to be careful with order so we can reuse our tris
        for (int i = 0; i < 3; i++)
        {
            for (int j = 4; j >= 0; j--)
            {
                int vIlocal = vI + 5 * i + j;
                verts.Add(new Vector3(-verts[vIlocal].x, verts[vIlocal].y, verts[vIlocal].z));
            }
        }
        //Good luck figuring out what's going on, but I promise it works
        verts.Add(new Vector3(-verts[vI + 16].x, verts[vI + 16].y, verts[vI + 16].z));
        verts.Add(new Vector3(-verts[vI + 15].x, verts[vI + 15].y, verts[vI + 15].z));

        //Now to copy our tris
        for (int i = 0; i < 18; i++)
        {
            int triI = tris.Count - 54;
            addTri(tris[triI] + 17, tris[triI + 1] + 17, tris[triI + 2] + 17);
        }

        //add everything to UVs
        for (int i = vI; i < verts.Count; i++)
        {
            addUV(verts[i], 0);
        }

        //For the lower corners, we're just going to generate the thick parts of the frame for now.
        //The thin parts will depend on whether the modular stat tabs are present.
        int copyVI = vI;
        vI = verts.Count;

        //frame verts
        for (int i = 0; i < 2; i++)
        {
            int copyVIlocal = copyVI + 17 * i;

            for (int j = 0; j < 3; j++)
            {
                verts.Add(new Vector3(-verts[copyVIlocal + 0].x, verts[copyVIlocal + 0].y, -verts[copyVIlocal + 0].z));
                verts.Add(new Vector3(-verts[copyVIlocal + 1].x, verts[copyVIlocal + 1].y, -verts[copyVIlocal + 1].z));
                verts.Add(new Vector3(-verts[copyVIlocal + 3].x, verts[copyVIlocal + 3].y, -verts[copyVIlocal + 3].z));
                verts.Add(new Vector3(-verts[copyVIlocal + 4].x, verts[copyVIlocal + 4].y, -verts[copyVIlocal + 4].z));

                copyVIlocal += 5;
            }
            verts.Add(new Vector3(-verts[copyVIlocal + 0].x, verts[copyVIlocal + 0].y, -verts[copyVIlocal + 0].z));
            verts.Add(new Vector3(-verts[copyVIlocal + 1].x, verts[copyVIlocal + 1].y, -verts[copyVIlocal + 1].z));
        }

        //triangles
        for (int i = 0; i < 4; i++)
        {
            int vIlocal = vI + 2 * i + (i > 1 ? 10 : 0);
            addTri(vIlocal + 0, vIlocal + 4, vIlocal + 1);
            addTri(vIlocal + 4, vIlocal + 5, vIlocal + 1);
            addTri(vIlocal + 4, vIlocal + 8, vIlocal + 5);
            addTri(vIlocal + 8, vIlocal + 9, vIlocal + 5);

            addTri(vIlocal + 8, i % 2 == 0 ? vIlocal + 12 : vIlocal + 11, vIlocal + 9);
        }

        //uv
        for (int i = vI; i < verts.Count; i++)
        {
            addUV(verts[i], 0);
        }

        //Home stretch - modular stat tabs

        //NESW first
        bool[] makeTab = { HasN, HasE, HasSAC, HasW };

        for (int i = 0; i < 4; i++)
        {
            vI = verts.Count;

            if (makeTab[i])
            {
                MakeStatCutout(verts, tris, vI, modifiers, edgeInnerThick, edgeOuterThick);
            }
            else
            {
                //make a thick frame
                verts.Add(new Vector3(0.0f, FrameThickness, 1.0f));
                verts.Add(new Vector3(edgeOuterThick.x, FrameThickness, edgeOuterThick.z));
                verts.Add(new Vector3(-edgeOuterThick.x, FrameThickness, edgeOuterThick.z));
                verts.Add(new Vector3(edgeInnerThick.x, FrameThickness, edgeInnerThick.z));
                verts.Add(new Vector3(-edgeInnerThick.x, FrameThickness, edgeInnerThick.z));
                verts.Add(new Vector3(verts[vI + 3].x, modifiers[7].y, verts[vI + 3].z + modifiers[7].x));
                verts.Add(new Vector3(verts[vI + 4].x, modifiers[7].y, verts[vI + 4].z + modifiers[7].x));
                verts.Add(new Vector3(verts[vI + 3].x, modifiers[0].y, verts[vI + 3].z + modifiers[0].x));
                verts.Add(new Vector3(verts[vI + 4].x, modifiers[0].y, verts[vI + 4].z + modifiers[0].x));

                addTri(vI + 0, vI + 1, vI + 2);
                addTri(vI + 1, vI + 3, vI + 2);
                addTri(vI + 2, vI + 3, vI + 4);
                addTri(vI + 3, vI + 5, vI + 4);
                addTri(vI + 4, vI + 5, vI + 6);
                addTri(vI + 5, vI + 7, vI + 6);
                addTri(vI + 6, vI + 7, vI + 8);
            }

            //rotate vertices
            for (int j = vI; j < verts.Count; j++)
            {
                float newX = verts[j].x * Mathf.Cos(-2 * PI4 * i) - verts[j].z * Mathf.Sin(-2 * PI4 * i);
                float newZ = verts[j].x * Mathf.Sin(-2 * PI4 * i) + verts[j].z * Mathf.Cos(-2 * PI4 * i);
                verts[j] = new Vector3(newX, verts[j].y, newZ);

                //because some parts of this need different textures, we need to be a little sneaky with the UVs
                if (makeTab[i] && j - vI < 6)
                {
                    addUV(verts[j], 3);
                }
                else
                {
                    addUV(verts[j], 0);
                }
            }
        }

        //RD
        bool[] makeTabDR = { HasR, HasD };

        for (int i = 0; i < 2; i++)
        {
            vI = verts.Count;
            Vector3 thinFrameBorder = new Vector3(Mathf.Lerp(verts[4].x, verts[2].x, 0.25f), 0.0f, Mathf.Lerp(verts[4].z, verts[2].z, 0.25f));

            if (makeTabDR[i])
            {
                float tabLowerX = Mathf.Lerp(verts[4].x, verts[2].x, 0.1875f);
                //make a tab
                //start with the placard
                verts.Add(new Vector3(0.0f, FrameThickness / 2.0f, 1.0f));
                verts.Add(new Vector3(edgeOuterThick.x, FrameThickness / 2.0f, edgeOuterThick.z));
                verts.Add(new Vector3(-edgeOuterThick.x, FrameThickness / 2.0f, edgeOuterThick.z));
                verts.Add(new Vector3(tabLowerX, FrameThickness / 2.0f, edgeInnerThick.z));
                verts.Add(new Vector3(-tabLowerX, FrameThickness / 2.0f, edgeInnerThick.z));
                verts.Add(new Vector3(0.0f, FrameThickness / 2.0f, verts[2].z));

                addTri(vI + 0, vI + 1, vI + 2);
                addTri(vI + 1, vI + 5, vI + 2);
                addTri(vI + 1, vI + 3, vI + 5);
                addTri(vI + 2, vI + 5, vI + 4);

                //build upper edge, clockwise
                verts.Add(verts[vI + 0] + Vector3.up * (FrameThickness / 2.0f));
                verts.Add(verts[vI + 1] + Vector3.up * (FrameThickness / 2.0f));
                verts.Add(verts[vI + 3] + Vector3.up * (FrameThickness / 2.0f));
                verts.Add(verts[vI + 5] + Vector3.up * (FrameThickness / 2.0f));
                verts.Add(verts[vI + 4] + Vector3.up * (FrameThickness / 2.0f));
                verts.Add(verts[vI + 2] + Vector3.up * (FrameThickness / 2.0f));

                //find inner angles and build verts, triangles
                List<Vector3> innerAngles = new List<Vector3>();
                Vector3 innerAngle;
                for (int j = 0; j <= 6; j++)
                {
                    int vIlocal = vI + (j % 6) + 6;
                    int ccwNeighbor = vI + ((j + 5) % 6) + 6;

                    if (j != 6)
                    {
                        int cwNeighbor = vI + ((j + 1) % 6) + 6;
                        innerAngle = findInnerAngle(verts[vIlocal], verts[ccwNeighbor], verts[cwNeighbor]);
                        verts.Add(new Vector3(verts[vIlocal].x + innerAngle.x * modifiers[5].x, modifiers[5].y, verts[vIlocal].z + innerAngle.z * modifiers[5].x));
                        verts.Add(new Vector3(verts[vIlocal].x + innerAngle.x * modifiers[4].x, modifiers[4].y, verts[vIlocal].z + innerAngle.z * modifiers[4].x));
                        innerAngles.Add(innerAngle);
                    }

                    if (j != 0)
                    {
                        int v1 = vIlocal;
                        int v2 = ccwNeighbor;
                        int v3 = v1 + 6 + (j % 6);
                        int v4 = v2 + 6 + ((j + 5) % 6);
                        int v5 = v3 + 1;
                        int v6 = v4 + 1;
                        addTri(v1, v3, v2);
                        addTri(v2, v3, v4);
                        addTri(v3, v5, v4);
                        addTri(v4, v5, v6);
                    }
                }

                //outer edge
                verts.Add(new Vector3(verts[vI + 7].x + Mathf.Sqrt(3) * 0.5f * modifiers[5].x, modifiers[5].y, verts[vI + 7].z - 0.5f * modifiers[5].x));
                verts.Add(new Vector3(verts[vI + 7].x + Mathf.Sqrt(3) * 0.5f * modifiers[4].x, modifiers[4].y, verts[vI + 7].z - 0.5f * modifiers[4].x));

                verts.Add(new Vector3(verts[vI + 8].x - innerAngles[2].x * modifiers[5].x, modifiers[5].y, verts[vI + 8].z - innerAngles[2].z * modifiers[5].x));
                verts.Add(new Vector3(verts[vI + 8].x - innerAngles[2].x * modifiers[4].x, modifiers[4].y, verts[vI + 8].z - innerAngles[2].z * modifiers[4].x));

                verts.Add(new Vector3(0.0f, modifiers[7].y, verts[vI + 09].z + modifiers[7].x));
                verts.Add(new Vector3(0.0f, modifiers[0].y, verts[vI + 09].z + modifiers[0].x));

                verts.Add(new Vector3(verts[vI + 10].x + innerAngles[2].x * modifiers[5].x, modifiers[5].y, verts[vI + 10].z - innerAngles[2].z * modifiers[5].x));
                verts.Add(new Vector3(verts[vI + 10].x + innerAngles[2].x * modifiers[4].x, modifiers[4].y, verts[vI + 10].z - innerAngles[2].z * modifiers[4].x));

                verts.Add(new Vector3(verts[vI + 11].x - Mathf.Sqrt(3) * 0.5f * modifiers[5].x, modifiers[5].y, verts[vI + 11].z - 0.5f * modifiers[5].x));
                verts.Add(new Vector3(verts[vI + 11].x - Mathf.Sqrt(3) * 0.5f * modifiers[4].x, modifiers[4].y, verts[vI + 11].z - 0.5f * modifiers[4].x));

                addTri(vI + 07, vI + 24, vI + 08);
                addTri(vI + 08, vI + 24, vI + 26);
                addTri(vI + 24, vI + 25, vI + 26);
                addTri(vI + 26, vI + 25, vI + 27);

                addTri(vI + 08, vI + 26, vI + 09);
                addTri(vI + 09, vI + 26, vI + 28);
                addTri(vI + 26, vI + 27, vI + 28);
                addTri(vI + 28, vI + 27, vI + 29);

                addTri(vI + 09, vI + 28, vI + 10);
                addTri(vI + 10, vI + 28, vI + 30);
                addTri(vI + 28, vI + 29, vI + 30);
                addTri(vI + 30, vI + 29, vI + 31);

                addTri(vI + 10, vI + 30, vI + 11);
                addTri(vI + 11, vI + 30, vI + 32);
                addTri(vI + 30, vI + 31, vI + 32);
                addTri(vI + 32, vI + 31, vI + 33);
            }
            else
            {
                //make a thin frame
                float midModX = 1.0f + modifiers[7].x;
                float lowModX = 1.0f + modifiers[0].x;
                verts.Add(new Vector3(-thinFrameBorder.x, FrameThickness, thinFrameBorder.z));
                verts.Add(new Vector3(verts[vI + 0].x * midModX, modifiers[7].y, verts[vI + 0].z * midModX));
                verts.Add(new Vector3(verts[vI + 0].x * lowModX, modifiers[0].y, verts[vI + 0].z * lowModX));
                verts.Add(new Vector3(0.0f, FrameThickness, 1.0f));
                verts.Add(new Vector3(verts[vI + 3].x * midModX, modifiers[7].y, verts[vI + 3].z * midModX));
                verts.Add(new Vector3(verts[vI + 3].x * lowModX, modifiers[0].y, verts[vI + 3].z * lowModX));
                verts.Add(new Vector3(thinFrameBorder.x, FrameThickness, thinFrameBorder.z));
                verts.Add(new Vector3(verts[vI + 6].x * midModX, modifiers[7].y, verts[vI + 6].z * midModX));
                verts.Add(new Vector3(verts[vI + 6].x * lowModX, modifiers[0].y, verts[vI + 6].z * lowModX));

                addTri(vI + 0, vI + 3, vI + 4);
                addTri(vI + 0, vI + 4, vI + 1);
                addTri(vI + 1, vI + 4, vI + 5);
                addTri(vI + 1, vI + 5, vI + 2);

                addTri(vI + 3, vI + 6, vI + 7);
                addTri(vI + 3, vI + 7, vI + 4);
                addTri(vI + 4, vI + 7, vI + 8);
                addTri(vI + 4, vI + 8, vI + 5);
            }

            //rotate vertices
            for (int j = vI; j < verts.Count; j++)
            {
                float newX = verts[j].x * Mathf.Cos(-PI4 * (3 + 2 * i)) - verts[j].z * Mathf.Sin(-PI4 * (3 + 2 * i));
                float newZ = verts[j].x * Mathf.Sin(-PI4 * (3 + 2 * i)) + verts[j].z * Mathf.Cos(-PI4 * (3 + 2 * i));
                verts[j] = new Vector3(newX, verts[j].y, newZ);

                //because some parts of this need different textures, we need to be a little sneaky with the UVs
                if (makeTabDR[i] && j - vI < 6)
                {
                    addUV(verts[j], 3);
                }
                else
                {
                    addUV(verts[j], 0);
                }
            }
        }

        //Phew, we made it. Send the data back.
        MeshData newMesh = new MeshData
        {
            Verts = verts.ToArray(),
            UVs = uvs.ToArray(),
            Tris = tris.ToArray()
        };
        return newMesh;

        int CreateNamePlacard(List<Vector3> verts, List<Vector2> uvs, List<int> tris, int vI)
        {
            //Build name placard. Current number of verts is <math>, so cheat
            float height = FrameThickness / 3.0f;
            //We want six verts
            //More cheating
            float upperX = Mathf.Lerp(verts[2].x, verts[4].x, 0.25f);
            float upperZ = Mathf.Lerp(verts[2].z, verts[4].z, 0.25f);
            float lowerX = Mathf.Lerp(verts[0].x, verts[2].x, 0.75f);
            float lowerZ = Mathf.Lerp(verts[0].z, verts[2].z, 0.75f);

            //Right side, bottom to top

            verts.Add(new Vector3(lowerX, height, lowerZ));
            verts.Add(new Vector3(verts[2].x, height, verts[2].z));
            verts.Add(new Vector3(upperX, height, upperZ));

            //Left side, bottom to top
            verts.Add(new Vector3(-lowerX, height, lowerZ));
            verts.Add(new Vector3(-verts[2].x, height, verts[2].z));
            verts.Add(new Vector3(-upperX, height, upperZ));

            //Friendly reminder, Unity winds triangles clockwise
            addTri(vI + 0, vI + 3, vI + 1);
            addTri(vI + 1, vI + 3, vI + 4);
            addTri(vI + 1, vI + 4, vI + 2);
            addTri(vI + 2, vI + 4, vI + 5);

            //add name placard verts to name placard texture
            for (int i = vI; i < verts.Count; i++)
            {
                addUV(verts[i], 1);
            }

            //Consistency is for nerds. Type placard
            return vI + 6;
        }

        int CreateSubtypePlacard(List<Vector3> verts, List<Vector2> uvs, List<int> tris, int vI)
        {
            float height = FrameThickness / 6.0f;

            //Just four verts this time
            float upperX = TypePlacardWidth;
            float lowerX = TypePlacardWidth - 0.1f;
            float lowerZ = verts[2].z - TypePlacardHeight; //Mathf.Lerp(verts[0].z, verts[2].z, 0.5f);

            //Right side, bottom to top
            verts.Add(new Vector3(lowerX, height, lowerZ));
            verts.Add(new Vector3(upperX, height, verts[2].z)); //don't change

            //Left side, same deal
            verts.Add(new Vector3(-lowerX, height, lowerZ));
            verts.Add(new Vector3(-upperX, height, verts[2].z));

            //Time to trihard
            addTri(vI + 0, vI + 2, vI + 1);
            addTri(vI + 1, vI + 2, vI + 3);

            //and uvs again
            for (int i = vI; i < verts.Count; i++)
            {
                addUV(verts[i], 2);
            }

            //NOW FOR THE FUN PART
            //The non-modular part of the inner frame
            vI += 4;
            return vI;
        }

        void MakeStatCutout(List<Vector3> verts, List<int> tris, int vI, List<Vector2> modifiers, Vector3 edgeInnerThick, Vector3 edgeOuterThick)
        {
            float tabLowerXMult = fullArt ? 0.35f : 0.1875f;
            //float tabLowerXMult = 0.1875f;
            float tabLowerX = Mathf.Lerp(verts[4].x, verts[2].x, tabLowerXMult);
            //Debug.Log($"Interpolating between [4]:{verts[4]} and [2]:{verts[2]} by {tabLowerXMult}");
            //Debug.Log($"edgeOuterThick {edgeOuterThick}; edgeInnerThick {edgeInnerThick}");

            float fullArtPointMult = fullArt ? 0.5f : 0f;
            float fullArtDragDown = (verts[4].z - verts[2].z) * fullArtPointMult;

            float height = FrameThickness / 2.0f;
            //make a tab
            //start with the placard TODO maniuplate these if i want stat placards to be bigger
            //specifically "edgeOuterThick" or osmething. mess with it
            //clockwise, for N orientation: midnight
            verts.Add(new Vector3(0.0f, height, 1.0f));

            verts.Add(new Vector3(edgeOuterThick.x, height, edgeOuterThick.z)); //2
            verts.Add(new Vector3(-edgeOuterThick.x, height, edgeOuterThick.z)); //10

            verts.Add(new Vector3(tabLowerX, height, edgeInnerThick.z - fullArtDragDown)); //5
            verts.Add(new Vector3(-tabLowerX, height, edgeInnerThick.z - fullArtDragDown)); //7

            verts.Add(new Vector3(0.0f, height, verts[2].z - fullArtDragDown)); //6. Matches z dimension of first clockwise point (the 1-2 PM point)

            addTri(vI + 0, vI + 1, vI + 2);
            addTri(vI + 1, vI + 5, vI + 2);
            addTri(vI + 1, vI + 3, vI + 5);
            addTri(vI + 2, vI + 5, vI + 4);

            //build upper ridge edge, clockwise
            verts.Add(verts[vI + 0] + Vector3.up * (height));
            verts.Add(verts[vI + 1] + Vector3.up * (height));
            verts.Add(verts[vI + 3] + Vector3.up * (height));
            verts.Add(verts[vI + 5] + Vector3.up * (height));
            verts.Add(verts[vI + 4] + Vector3.up * (height));
            verts.Add(verts[vI + 2] + Vector3.up * (height));

            //find inner angles and build verts, triangles
            Vector3 innerAngle;
            for (int j = 0; j <= 6; j++)
            {
                int vIlocal = vI + (j % 6) + 6;
                int ccwNeighbor = vI + ((j + 5) % 6) + 6;

                if (j != 6)
                {
                    int cwNeighbor = vI + ((j + 1) % 6) + 6;
                    innerAngle = findInnerAngle(verts[vIlocal], verts[ccwNeighbor], verts[cwNeighbor]);
                    verts.Add(new Vector3(verts[vIlocal].x + innerAngle.x * modifiers[5].x,
                        modifiers[5].y,
                        verts[vIlocal].z + innerAngle.z * modifiers[5].x));
                    verts.Add(new Vector3(verts[vIlocal].x + innerAngle.x * modifiers[4].x,
                        modifiers[4].y,
                        verts[vIlocal].z + innerAngle.z * modifiers[4].x));
                }

                if (j != 0)
                {
                    int v1 = vIlocal;
                    int v2 = ccwNeighbor;
                    int v3 = v1 + 6 + (j % 6);
                    int v4 = v2 + 6 + ((j + 5) % 6);
                    int v5 = v3 + 1;
                    int v6 = v4 + 1;
                    addTri(v1, v3, v2);
                    addTri(v2, v3, v4);
                    addTri(v3, v5, v4);
                    addTri(v4, v5, v6);
                }
            }

            //Outer frame and fill holes
            float lowerBezelVertexXAdjustment = 0f;//ExtraZoomOutBezel;
            float lowerBezelVertexYAdjustment = fullArt ? (fullArtDragDown - (ExtraZoomOutBezel * 2)) : 0f;
            verts.Add(new Vector3(-edgeInnerThick.x, FrameThickness, edgeInnerThick.z));
            verts.Add(new Vector3(verts[vI + 24].x, modifiers[7].y, verts[vI + 24].z + modifiers[7].x));
            verts.Add(new Vector3(verts[vI + 24].x, modifiers[0].y, verts[vI + 24].z + modifiers[0].x));

            innerAngle = findInnerAngle(verts[vI + 10], verts[vI + 24], verts[vI + 09]);
            verts.Add(new Vector3(verts[vI + 10].x + innerAngle.x * modifiers[5].x, modifiers[5].y, verts[vI + 10].z + innerAngle.z * modifiers[5].x - lowerBezelVertexYAdjustment));
            verts.Add(new Vector3(verts[vI + 10].x + innerAngle.x * modifiers[4].x - lowerBezelVertexXAdjustment, modifiers[4].y, verts[vI + 10].z + innerAngle.z * modifiers[4].x - lowerBezelVertexYAdjustment));

            verts.Add(new Vector3(0.0f, modifiers[7].y, verts[vI + 09].z + modifiers[7].x));
            verts.Add(new Vector3(0.0f, modifiers[0].y, verts[vI + 09].z + modifiers[0].x));

            innerAngle = findInnerAngle(verts[vI + 08], new Vector3(edgeInnerThick.x, FrameThickness, edgeInnerThick.z), verts[vI + 09]);
            verts.Add(new Vector3(verts[vI + 08].x + innerAngle.x * modifiers[5].x + lowerBezelVertexXAdjustment, modifiers[5].y, verts[vI + 08].z + innerAngle.z * modifiers[5].x - lowerBezelVertexYAdjustment));
            verts.Add(new Vector3(verts[vI + 08].x + innerAngle.x * modifiers[4].x, modifiers[4].y, verts[vI + 08].z + innerAngle.z * modifiers[4].x - lowerBezelVertexYAdjustment));

            verts.Add(new Vector3(edgeInnerThick.x, FrameThickness, edgeInnerThick.z));
            verts.Add(new Vector3(verts[vI + 33].x, modifiers[7].y, verts[vI + 33].z + modifiers[7].x));
            verts.Add(new Vector3(verts[vI + 33].x, modifiers[0].y, verts[vI + 33].z + modifiers[0].x));

            addTri(vI + 11, vI + 10, vI + 24);
            addTri(vI + 24, vI + 10, vI + 27);
            addTri(vI + 24, vI + 27, vI + 25);
            addTri(vI + 25, vI + 27, vI + 28);
            addTri(vI + 25, vI + 28, vI + 26);

            addTri(vI + 10, vI + 09, vI + 29);
            addTri(vI + 10, vI + 29, vI + 27);
            addTri(vI + 27, vI + 29, vI + 30);
            addTri(vI + 27, vI + 30, vI + 28);

            addTri(vI + 09, vI + 08, vI + 31);
            addTri(vI + 09, vI + 31, vI + 29);
            addTri(vI + 29, vI + 31, vI + 32);
            addTri(vI + 29, vI + 32, vI + 30);

            addTri(vI + 08, vI + 07, vI + 33);
            addTri(vI + 08, vI + 33, vI + 34);
            addTri(vI + 08, vI + 34, vI + 31);
            addTri(vI + 31, vI + 34, vI + 35);
            addTri(vI + 31, vI + 35, vI + 32);
        }
    }

    private MeshFilter ApplyMesh(MeshData newMesh)
    {
        MeshFilter filter = gameObject.GetComponent<MeshFilter>();
        MeshCollider collider = gameObject.GetComponent<MeshCollider>();

        if(CardMesh == null)
        {
            CardMesh = new Mesh();
        }

        CardMesh.Clear();
        CardMesh.vertices = newMesh.Verts;
        CardMesh.uv = newMesh.UVs;
        CardMesh.triangles = newMesh.Tris;
        CardMesh.RecalculateNormals();

        filter.sharedMesh = CardMesh;
        collider.sharedMesh = CardMesh;
        return filter;
    }

    private TextureParams MyParams => new TextureParams()
    {
        TextureResolution = TextureResolution,
        CharacterArt = CharacterArt,
        CharacterArtSamplingIncrementRatio = CharacterArtSamplingIncrementRatio,
        fullArt = fullArt,

        ApplyStatColors = ApplyStatColors,
        FrameThickness = FrameThickness,

        HasN = HasN,
        HasE = HasE,
        HasSAC = HasSAC,
        HasW = HasW,
        HasR = HasR,
        HasD = HasD,

        NColor = NColor,
        EColor = EColor,
        SACColor = SACColor,
        WColor = WColor,

        FrameMetallic = FrameMetallic,
        FrameGloss = FrameGloss,

        NamePlacardMetallic = NamePlacardMetallic,
        NamePlacardGloss = NamePlacardGloss,

        TypePlacardMetallic = TypePlacardMetallic,
        TypePlacardGloss = TypePlacardGloss,

        StatsMetallic = StatsMetallic,
        StatsGloss = StatsGloss,

        EffectTextMetallic = EffectTextMetallic,
        EffectTextGloss = EffectTextGloss,

        CharacterArtMetallic = CharacterArtMetallic,
        CharacterArtGloss = CharacterArtGloss,

        CardBackMetallic = CardBackMetallic,
        CardBackGloss = CardBackGloss,

        CharacterArtUpperBound = CharacterArtUpperBound,

        FrameTexture = FrameTexture,
        NamePlacardTexture = NamePlacardTexture,
        TypePlacardTexture = TypePlacardTexture,
        EffectTextTexture = EffectTextTexture,
        CardBackTexture = CardBackTexture,
        NTexture = NTexture,
        ETexture = ETexture,
        SACTexture = SACTexture,
        WTexture = WTexture,
        RTexture = RTexture,
        DTexture = DTexture,
    };

    public class TextureParams
    {
        public static TextureParams Default => new TextureParams()
        {
            TextureResolution = 256,

            //FrameThickness = 0.0225f,

            HasSAC = true, //Duh

            NColor = new Color32(48, 128, 48, 0),
            EColor = new Color32(192, 48, 48, 0),
            SACColor = new Color32(128, 48, 128, 0),
            WColor = new Color32(0, 128, 128, 0),

            FrameMetallic = 0.75f,
            FrameGloss = 0.5f,

            NamePlacardMetallic = 0.75f,
            NamePlacardGloss = 0.5f,

            TypePlacardMetallic = 0.5f,
            TypePlacardGloss = 0f,

            StatsMetallic = 0f,
            StatsGloss = 0f,

            EffectTextMetallic = 0f,
            EffectTextGloss = 0f,

            CharacterArtMetallic = 0f,
            CharacterArtGloss = 0f,

            CardBackMetallic = 0.75f,
            CardBackGloss = 0.5f,
        };

        public static TextureParams DefaultZoomed
        {
            get
            {
                var zoomed = Default;
                zoomed.FrameThickness = 0.05f;
                zoomed.CharacterArtSamplingIncrementRatio = CharacterArtSamplingIncrementRatioHasText;
                zoomed.fullArt = false;
                zoomed.CharacterArtUpperBound = CharacterArtUpperBoundHasText;
                return zoomed;
            }
        }

        public static TextureParams DefaultUnzoomed
        {
            get
            {
                var unzoomed = Default;
                unzoomed.FrameThickness = 0.0225f;
                unzoomed.CharacterArtSamplingIncrementRatio = CharacterArtSamplingIncrementRatioFullArt;
                unzoomed.fullArt = true;
                unzoomed.CharacterArtUpperBound = CharacterArtUpperBoundFullArt;
                return unzoomed;
            }
        }

        public static TextureParams Params(bool isZoomed, bool isChar, 
            Sprite FrameTexture, Sprite NamePlacardTexture, Sprite TypePlacardTexture, Sprite EffectTextTexture, Sprite CardBackTexture,
            Sprite NTexture, Sprite ETexture, Sprite SACTexture, Sprite WTexture, Sprite RTexture, Sprite DTexture)
        {
            var ret = isZoomed ? DefaultZoomed : DefaultUnzoomed;

            ret.FrameTexture = FrameTexture;
            ret.NamePlacardTexture = NamePlacardTexture;
            ret.TypePlacardTexture = TypePlacardTexture;
            ret.EffectTextTexture = EffectTextTexture;
            ret.CardBackTexture = CardBackTexture;
            ret.NTexture = NTexture;
            ret.ETexture = ETexture;
            ret.SACTexture = SACTexture;
            ret.WTexture = WTexture;
            ret.RTexture = RTexture;
            ret.DTexture = DTexture;

            return ret;
        }

        internal int TextureResolution;
        internal Sprite FrameTexture;
        internal Sprite NamePlacardTexture;
        internal Sprite TypePlacardTexture;
        internal Sprite CharacterArt;
        internal float CharacterArtSamplingIncrementRatio;
        internal bool fullArt;

        internal Sprite EffectTextTexture;
        internal Sprite CardBackTexture;

        internal bool ApplyStatColors;
        internal float FrameThickness;

        internal bool HasN;
        internal bool HasE;
        internal bool HasSAC;
        internal bool HasW;
        internal bool HasR;
        internal bool HasD;

        internal Color NColor;
        internal Color EColor;
        internal Color SACColor;
        internal Color WColor;

        internal float FrameMetallic;
        internal float FrameGloss;
        internal float NamePlacardMetallic;
        internal float NamePlacardGloss;
        internal float TypePlacardMetallic;
        internal float TypePlacardGloss;
        internal Sprite NTexture;
        internal Sprite ETexture;
        internal Sprite SACTexture;
        internal Sprite WTexture;
        internal Sprite RTexture;
        internal Sprite DTexture;
        internal float StatsMetallic;
        internal float StatsGloss;
        internal float EffectTextMetallic;
        internal float EffectTextGloss;
        internal float CharacterArtMetallic;
        internal float CharacterArtGloss;
        internal float CardBackMetallic;
        internal float CardBackGloss;
        internal float CharacterArtUpperBound;
    }

    public static (Texture2D, Texture2D) BuildTexture(Texture2D oldTexture, Texture2D oldMetalness, TextureParams textureParams, bool rebuildAll = true)
    {
        Texture2D newTexture = rebuildAll || oldTexture == default
            ? new Texture2D(textureParams.TextureResolution * 3, textureParams.TextureResolution * 2, TextureFormat.ARGB32, false)
            : oldTexture;
        Texture2D metalness = rebuildAll || oldMetalness == default
            ? new Texture2D(textureParams.TextureResolution * 3, textureParams.TextureResolution * 2, TextureFormat.ARGB32, false)
            : oldMetalness;

        (Vector2Int FrameSamplingStartIndex, float FrameSamplingIncrement) = SamplingInformation(textureParams.FrameTexture, textureParams);
        (Vector2Int NamePlacardSamplingStartIndex, float NamePlacardSamplingIncrement) = SamplingInformation(textureParams.NamePlacardTexture, textureParams);
        (Vector2Int TypePlacardSamplingStartIndex, float TypePlacardSamplingIncrement) = SamplingInformation(textureParams.TypePlacardTexture, textureParams);

        var charArtTex = textureParams.CharacterArt.texture;
        int squaringFactor = Mathf.Abs(charArtTex.width - charArtTex.height) / 2;
        float shorterDimension = Mathf.Min(charArtTex.width, charArtTex.height);

        float CharacterArtSamplingIncrement = (shorterDimension / (float) textureParams.TextureResolution) * textureParams.CharacterArtSamplingIncrementRatio;
        Vector2Int CharacterArtSamplingStartIndex = textureParams.fullArt
            ? (Vector2Int.right * (int)(squaringFactor + (shorterDimension * CharacterArtSamplingStartIndexRatio)))
            + (Vector2Int.down * (int)(shorterDimension * 2.0f * CharacterArtSamplingStartIndexRatio))
            : (Vector2Int.right * 0)//(int)(squaringFactor + (shorterDimension * CharacterArtSamplingStartIndexRatio)))
            + (Vector2Int.down * (int)(shorterDimension * -2.0f * CharacterArtSamplingStartIndexRatio));
        //Debug.Log($"Character art {CharacterArtSamplingIncrement}, {CharacterArtSamplingStartIndex}");

        if (textureParams.fullArt) CharacterArtSamplingStartIndex = new Vector2Int(-2 * CharacterArtSamplingStartIndex.x, CharacterArtSamplingStartIndex.y);

        (Vector2Int EffectTextSamplingStartIndex, float EffectTextSamplingIncrement) = SamplingInformation(textureParams.EffectTextTexture, textureParams);
        (Vector2Int CardBackSamplingStartIndex, float CardBackSamplingIncrement) = SamplingInformation(textureParams.CardBackTexture, textureParams);

        for (int x = 0; x < textureParams.TextureResolution; x++)
        {
            for (int y = 0; y < textureParams.TextureResolution; y++)
            {
                Vector2Int position = new Vector2Int(x, y);
                Vector2Int samplePosition;

                //Frame texture
                samplePosition = new Vector2Int(FrameSamplingStartIndex.x + (int)(FrameSamplingIncrement * x), FrameSamplingStartIndex.y + (int)(FrameSamplingIncrement * y));
                Color frameColor = textureParams.FrameTexture.texture.GetPixel(samplePosition.x, samplePosition.y);
                if (textureParams.ApplyStatColors)
                {
                    Vector2 normalizedXY = new Vector2(((float)x / textureParams.TextureResolution - textureParams.FrameThickness) / (1.0f - 2.0f * textureParams.FrameThickness), ((float)y / textureParams.TextureResolution - textureParams.FrameThickness) / (1.0f - 2.0f * textureParams.FrameThickness));
                    if (normalizedXY.y > 7.1f / 12.0f || normalizedXY.y < 4.9f / 12.0f)
                    {
                        if (textureParams.HasW && normalizedXY.x < 1.0f / 9.3f && normalizedXY.x > (4.97f / 12.0f) * (12.0f - 24.0f * normalizedXY.y) * (1.0f / 24.0f) && normalizedXY.x > -(4.97f / 12.0f) * (12.0f - 24.0f * normalizedXY.y) * (1.0f / 24.0f))
                        {
                            Recolor(ref frameColor, textureParams.WColor);
                        }
                        else if (textureParams.HasE && normalizedXY.x > 1.0f - 1.0f / 9.3f && normalizedXY.x - 1.0f < (4.97f / 12.0f) * (12.0f - 24.0f * normalizedXY.y) * (1.0f / 24.0f) && normalizedXY.x - 1.0f < -(4.97f / 12.0f) * (12.0f - 24.0f * normalizedXY.y) * (1.0f / 24.0f))
                        {
                            Recolor(ref frameColor, textureParams.EColor);
                        }
                    }
                    if (normalizedXY.x > 7.1f / 12.0f || normalizedXY.x < 4.9f / 12.0f)
                    {
                        if (textureParams.HasSAC && normalizedXY.y < 1.0f / 9.3f && normalizedXY.y > (4.97f / 12.0f) * (12.0f - 24.0f * normalizedXY.x) * (1.0f / 24.0f) && normalizedXY.y > -(4.97f / 12.0f) * (12.0f - 24.0f * normalizedXY.x) * (1.0f / 24.0f))
                        {
                            Recolor(ref frameColor, textureParams.SACColor);
                        }
                        else if (textureParams.HasN && normalizedXY.y > 1.0f - 1.0f / 9.3f && normalizedXY.y - 1.0f < (4.97f / 12.0f) * (12.0f - 24.0f * normalizedXY.x) * (1.0f / 24.0f) && normalizedXY.y - 1.0f < -(4.97f / 12.0f) * (12.0f - 24.0f * normalizedXY.x) * (1.0f / 24.0f))
                        {
                            Recolor(ref frameColor, textureParams.NColor);
                        }
                    }
                }
                newTexture.SetPixel(position.x, position.y, frameColor);
                metalness.SetPixel(position.x, position.y, new Color(textureParams.FrameMetallic, 0.0f, 0.0f, textureParams.FrameGloss));

                void Paint(Vector2Int startIndex, float samplingIncrement, Sprite sprite, float metallic, float gloss)
                {
                    samplePosition = new Vector2Int(startIndex.x + (int)(samplingIncrement * x), startIndex.y + (int)(samplingIncrement * y));
                    newTexture.SetPixel(position.x, position.y, sprite.texture.GetPixel(samplePosition.x, samplePosition.y));
                    metalness.SetPixel(position.x, position.y, new Color(metallic, 0.0f, 0.0f, gloss));
                }

                //Name placard texture
                position += textureParams.TextureResolution * Vector2Int.right;
                Paint(NamePlacardSamplingStartIndex, NamePlacardSamplingIncrement, textureParams.NamePlacardTexture, textureParams.NamePlacardMetallic, textureParams.NamePlacardGloss);

                //Type placard texture
                position += textureParams.TextureResolution * Vector2Int.right;
                Paint(TypePlacardSamplingStartIndex, TypePlacardSamplingIncrement, textureParams.TypePlacardTexture, textureParams.TypePlacardMetallic, textureParams.TypePlacardGloss);

                //Stats placards texture
                position = new Vector2Int(x, textureParams.TextureResolution + y);
                (Vector2Int statsStartIndex, float statsIncrement, Sprite statsTexture) = DetermineRelevantStatValues(x, y, textureParams);
                Paint(statsStartIndex, statsIncrement, statsTexture, textureParams.StatsMetallic, textureParams.StatsGloss);

                //Art and effect text texture
                position = new Vector2Int(textureParams.TextureResolution + x, textureParams.TextureResolution + y);
                Vector2Int frontStartIndex = EffectTextSamplingStartIndex;
                float frontIncrement = EffectTextSamplingIncrement;
                Sprite frontTexture = textureParams.EffectTextTexture;
                Color frontMetallic = new Color(textureParams.EffectTextMetallic, 0.0f, 0.0f, textureParams.EffectTextGloss);
                float effTextZoneRatio = 0.37f;
                float artRightBound = textureParams.fullArt ? 1.0f : effTextZoneRatio * (1f + textureParams.FrameThickness);

                if(textureParams.fullArt)
                {
                        frontStartIndex = CharacterArtSamplingStartIndex;
                        frontIncrement = CharacterArtSamplingIncrement;
                        frontTexture = textureParams.CharacterArt;
                        frontMetallic = new Color(textureParams.CharacterArtMetallic, 0.0f, 0.0f, textureParams.CharacterArtGloss);
                }
                else
                {
                    if (y > textureParams.TextureResolution * artRightBound)
                    {
                        if (x > textureParams.TextureResolution * CharacterArtLowerBound && x < textureParams.TextureResolution * textureParams.CharacterArtUpperBound)
                        {
                            frontStartIndex = CharacterArtSamplingStartIndex;
                            frontIncrement = CharacterArtSamplingIncrement;
                            frontTexture = textureParams.CharacterArt;
                            frontMetallic = new Color(textureParams.CharacterArtMetallic, 0.0f, 0.0f, textureParams.CharacterArtGloss);
                        }
                    }
                    else if (y > textureParams.TextureResolution * effTextZoneRatio * (1 - textureParams.FrameThickness))
                    {
                        frontStartIndex = FrameSamplingStartIndex;
                        frontIncrement = FrameSamplingIncrement;
                        frontTexture = textureParams.FrameTexture;
                        frontMetallic = new Color(textureParams.FrameMetallic, 0.0f, 0.0f, textureParams.FrameGloss);
                    }
                }

                samplePosition = new Vector2Int(frontStartIndex.x + (int)(frontIncrement * x), frontStartIndex.y + (int)(frontIncrement * y));
                newTexture.SetPixel(position.x, position.y, frontTexture.texture.GetPixel(samplePosition.x, samplePosition.y));
                metalness.SetPixel(position.x, position.y, frontMetallic);

                //Card back texture
                position += textureParams.TextureResolution * Vector2Int.right;
                samplePosition = new Vector2Int(CardBackSamplingStartIndex.x + (int)(CardBackSamplingIncrement * x), CardBackSamplingStartIndex.y + (int)(CardBackSamplingIncrement * y));
                newTexture.SetPixel(position.x, position.y, textureParams.CardBackTexture.texture.GetPixel(samplePosition.x, samplePosition.y));
                metalness.SetPixel(position.x, position.y, new Color(textureParams.CardBackMetallic, 0.0f, 0.0f, textureParams.CardBackGloss));
            }
        }

        newTexture.Apply();
        metalness.Apply();
        return (newTexture, metalness);
    }

    private static (Vector2Int, float, Sprite) DetermineRelevantStatValues(int x, int y, TextureParams textureParams)
    {

        (Vector2Int NSamplingStartIndex, float NSamplingIncrement) = SamplingInformation(textureParams.NTexture, textureParams);
        (Vector2Int ESamplingStartIndex, float ESamplingIncrement) = SamplingInformation(textureParams.ETexture, textureParams);
        (Vector2Int SACSamplingStartIndex, float SACSamplingIncrement) = SamplingInformation(textureParams.SACTexture, textureParams);
        (Vector2Int WSamplingStartIndex, float WSamplingIncrement) = SamplingInformation(textureParams.WTexture, textureParams);
        (Vector2Int RSamplingStartIndex, float RSamplingIncrement) = SamplingInformation(textureParams.RTexture, textureParams);
        (Vector2Int DSamplingStartIndex, float DSamplingIncrement) = SamplingInformation(textureParams.DTexture, textureParams);

        if (x < textureParams.TextureResolution / 3)
        {
            if (y > textureParams.TextureResolution / 3)
            {
                return (WSamplingStartIndex, WSamplingIncrement, textureParams.WTexture);
            }
            else
            {
                return (DSamplingStartIndex, DSamplingIncrement, textureParams.DTexture);
            }
        }
        else if (x < 2 * textureParams.TextureResolution / 3)
        {
            if (y > textureParams.TextureResolution / 2)
            {
                return (NSamplingStartIndex, NSamplingIncrement,textureParams. NTexture);
            }
            else
            {
                return (SACSamplingStartIndex, SACSamplingIncrement, textureParams.SACTexture);
            }
        }
        else
        {
            if (y > textureParams.TextureResolution / 3)
            {
                return (ESamplingStartIndex, ESamplingIncrement, textureParams.ETexture);
            }
            else
            {
                return (RSamplingStartIndex, RSamplingIncrement, textureParams.RTexture);
            }
        }
    }

    private static (Vector2Int, float) SamplingInformation(Sprite sprite, TextureParams textureParams)
    {
        int maxDim = Mathf.Max(sprite.texture.height, sprite.texture.width);
        int minDim = Mathf.Min(sprite.texture.height, sprite.texture.width);

        return (new Vector2Int((maxDim - minDim) / 2, 0), (float)minDim / textureParams.TextureResolution);
    }

    public static void BuildTextureCardArt(int TextureResolution, bool fullArt, float FrameThickness, float CharacterArtUpperBound,
        float CharacterArtSamplingIncrementRatio, Sprite CharacterArt, float CharacterArtMetallic, float CharacterArtGloss, Texture2D newTexture, Texture2D metalness)
    {
        int squaringFactor = Mathf.Abs(CharacterArt.texture.width - CharacterArt.texture.height) / 2;
        float shorterDimension = Mathf.Min(CharacterArt.texture.width, CharacterArt.texture.height);

        float CharacterArtSamplingIncrement = (shorterDimension / (float) TextureResolution) * CharacterArtSamplingIncrementRatio;
        Vector2Int CharacterArtSamplingStartIndex =
            (Vector2Int.right * (int)(squaringFactor + (shorterDimension * CharacterArtSamplingStartIndexRatio)))
            + (Vector2Int.down * (int)(shorterDimension * 2.0f * CharacterArtSamplingStartIndexRatio));
        //Debug.Log($"Character art {CharacterArtSamplingIncrement}, {CharacterArtSamplingStartIndex}");

        if (fullArt) CharacterArtSamplingStartIndex = new Vector2Int(-2 * CharacterArtSamplingStartIndex.x, CharacterArtSamplingStartIndex.y);

        float effTextZoneRatio = 0.37f;
        float artRightBound = fullArt ? 1.0f : effTextZoneRatio * (1f + FrameThickness);

        Vector2Int frontStartIndex = CharacterArtSamplingStartIndex;
        float frontIncrement = CharacterArtSamplingIncrement;
        Sprite frontTexture = CharacterArt;
        Color frontMetallic = new Color(CharacterArtMetallic, 0.0f, 0.0f, CharacterArtGloss);
        Debug.Log($"Bounds {artRightBound}, {CharacterArtLowerBound}, {CharacterArtUpperBound}\n"
            + $"Aka {TextureResolution * artRightBound}, {TextureResolution * CharacterArtLowerBound}x{TextureResolution * CharacterArtUpperBound}");

        for (int x = 0; x < TextureResolution; x++)
        {
            for (int y = 0; y < TextureResolution; y++)
            {
                //Art and effect text texture
                Vector2Int position = new Vector2Int(TextureResolution + x, TextureResolution + y);
                if (fullArt)
                {
                    Vector2Int samplePosition = new Vector2Int(frontStartIndex.x + (int)(frontIncrement * x), frontStartIndex.y + (int)(frontIncrement * y));
                    newTexture.SetPixel(position.x, position.y, frontTexture.texture.GetPixel(samplePosition.x, samplePosition.y));
                    metalness.SetPixel(position.x, position.y, frontMetallic);
                }
                if (y > TextureResolution * artRightBound)
                {
                    if (x > TextureResolution * CharacterArtLowerBound && x < TextureResolution * CharacterArtUpperBound)
                    {
                        Vector2Int samplePosition = new Vector2Int(frontStartIndex.x + (int)(frontIncrement * x), frontStartIndex.y + (int)(frontIncrement * y));
                        newTexture.SetPixel(position.x, position.y, frontTexture.texture.GetPixel(samplePosition.x, samplePosition.y));
                        metalness.SetPixel(position.x, position.y, frontMetallic);
                    }
                }
            }
        }
        newTexture.Apply();
        metalness.Apply();
    }

    private void ApplyTexture((Texture2D tex, Texture2D met) textures, Material mat)
    {
        mat.SetTexture(MainTextureName, textures.tex);
        mat.SetTexture(MainMetalnessName, textures.met);
        gameObject.GetComponent<MeshRenderer>().sharedMaterial = mat;
    }

    //private List<TextMeshPro> BuildTextBoxes()
    //{

    //}

    //private void ApplyText(List<TextMeshPro> textBoxes)
    //{

    //}

    public static void Recolor(ref Color target, Color newColor)
    {
        Color.RGBToHSV(target, out float tH, out float tS, out float tV);
        Color.RGBToHSV(newColor, out float nH, out float nS, out float nV);
        //target = Color.HSVToRGB(nH, 0.8f * nS + 0.2f * tS, tV * nV);
        target = newColor;
    }
}
