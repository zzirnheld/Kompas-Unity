using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public struct MeshData
{
    public Vector3[] Verts;
    public Vector2[] UVs;
    public int[] Tris;
}

public enum CostType
{
    S,
    A,
    C
}

public class VoxelCard : MonoBehaviour
{
    public static float PI4 = Mathf.PI / 4.0f;
    public Material BaseMaterial;

    public bool RebuildMeshOnChange;
    public bool RebuildTextureOnChange;
    [Range(0.001f, 0.1f)]
    public float FrameThickness;
    [Range(0.1f, 0.7f)]
    public float TypePlacardWidth;
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
    
    public void OnInspectorChange()
    {
        if (RebuildMeshOnChange)
        {
            GenerateMesh();
        }
        if (RebuildTextureOnChange)
        {
            GenerateTexture();
        }
    }

    public void Generate()
    {
        GenerateMesh();
        GenerateTexture();
        //List<TextMeshPro> textBoxes = BuildTextBoxes();   //Not Implemented
        //ApplyText(textBoxes);
    }

    public void GenerateMesh()
    {
        MeshData newMesh = BuildMesh();
        ApplyMesh(newMesh);
    }

    public void GenerateTexture()
    {
        List<Texture2D> newTextures = BuildTexture();
        ApplyTexture(newTextures);
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

        //Build card base

        //Build card base verts counter-clockwise around origin from (1, 0, 0)
        for (int i = 0; i < 8; i++)
        {
            verts.Add(new Vector3(Mathf.Cos(PI4 * i), FrameThickness / 12.0f, Mathf.Sin(PI4 * i))); //front
            verts.Add(new Vector3(Mathf.Cos(PI4 * i), -FrameThickness / 12.0f, Mathf.Sin(PI4 * i))); //back
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
            modifiers.Add(new Vector2(Mathf.Cos(PI4 * i), Mathf.Sin(PI4 * i)) * FrameThickness);
        }

        //Build 3/4 outer frame vertices counter-clockwise around origin from (1, 0, 0)
        for (int i = 0; i < 8; i++)
        {
            foreach (Vector3 mod in modifiers)
            {
                float modX = 1.0f + mod.x;
                verts.Add(new Vector3(Mathf.Cos(PI4 * i) * modX, mod.y, Mathf.Sin(PI4 * i) * modX));
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


        //Build name placard. Current number of verts is <math>, so cheat
        vI = verts.Count;
        vI = CreateNamePlacard(verts, uvs, tris, vI); //TODO: this can now be commented out. make it controlled by a flag
        vI = CreateSubtypePlacard(verts, uvs, tris, vI);
        //let's finish rounding out our modifiers, adding a copy of [0] so we don't have to play mod games
        for (int i = 11; i <= 12; i++)
        {
            modifiers.Add(new Vector2(Mathf.Cos(PI4 * i), Mathf.Sin(PI4 * i)) * FrameThickness);
        }

        //Calulate edge points for thick part of frames
        Vector3 edgeInnerThick = new Vector3(Mathf.Lerp(verts[2].x, verts[4].x, 0.75f), 0, Mathf.Lerp(verts[2].z, verts[4].z, 0.25f));
        Vector3 edgeOuterThick = new Vector3(edgeInnerThick.x, 0, Mathf.Lerp(verts[2].z, verts[4].z, 0.75f));
        Vector3 edgeThin = new Vector3(Mathf.Lerp(verts[2].x, verts[4].x, 0.25f), 0, edgeInnerThick.z);

        //Starting with the UR corner at y = 0 and working our way up, going counter-clockwise
        for (int i = 8; i >= 6; i--)
        {
            int vIlocal = verts.Count;
            float modX = 1.0f + modifiers[i].x;
            verts.Add(new Vector3(edgeInnerThick.x, modifiers[i].y, edgeInnerThick.z + modifiers[i].x));
            verts.Add(new Vector3(edgeThin.x * modX, modifiers[i].y, edgeThin.z * modX));
            verts.Add(new Vector3(verts[2].x * modX, modifiers[i].y, verts[2].z * modX));
            verts.Add(new Vector3(verts[vIlocal + 1].z, verts[vIlocal + 1].y, verts[vIlocal + 1].x));
            verts.Add(new Vector3(verts[vIlocal].z, verts[vIlocal].y, verts[vIlocal].x));
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
                verts.Add(new Vector3(-edgeInnerThick.x, FrameThickness, edgeInnerThick.z));
                verts.Add(new Vector3(verts[vI + 24].x, modifiers[7].y, verts[vI + 24].z + modifiers[7].x));
                verts.Add(new Vector3(verts[vI + 24].x, modifiers[0].y, verts[vI + 24].z + modifiers[0].x));

                innerAngle = findInnerAngle(verts[vI + 10], verts[vI + 24], verts[vI + 09]);
                verts.Add(new Vector3(verts[vI + 10].x + innerAngle.x * modifiers[5].x, modifiers[5].y, verts[vI + 10].z + innerAngle.z * modifiers[5].x));
                verts.Add(new Vector3(verts[vI + 10].x + innerAngle.x * modifiers[4].x, modifiers[4].y, verts[vI + 10].z + innerAngle.z * modifiers[4].x));

                verts.Add(new Vector3(0.0f, modifiers[7].y, verts[vI + 09].z + modifiers[7].x));
                verts.Add(new Vector3(0.0f, modifiers[0].y, verts[vI + 09].z + modifiers[0].x));

                innerAngle = findInnerAngle(verts[vI + 08], new Vector3(edgeInnerThick.x, FrameThickness, edgeInnerThick.z), verts[vI + 09]);
                verts.Add(new Vector3(verts[vI + 08].x + innerAngle.x * modifiers[5].x, modifiers[5].y, verts[vI + 08].z + innerAngle.z * modifiers[5].x));
                verts.Add(new Vector3(verts[vI + 08].x + innerAngle.x * modifiers[4].x, modifiers[4].y, verts[vI + 08].z + innerAngle.z * modifiers[4].x));

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
            float lowerZ = Mathf.Lerp(verts[0].z, verts[2].z, 0.5f);

            //Right side, bottom to top
            verts.Add(new Vector3(lowerX, height, lowerZ));
            verts.Add(new Vector3(upperX, height, verts[2].z));

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
    }

    private void ApplyMesh(MeshData newMesh)
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
    }

    private List<Texture2D> BuildTexture()
    {
        Texture2D newTexture = new Texture2D(TextureResolution * 3, TextureResolution * 2, TextureFormat.ARGB32, false);
        Texture2D metalness = new Texture2D(TextureResolution * 3, TextureResolution * 2, TextureFormat.ARGB32, false);

        Vector2Int FrameSamplingStartIndex;
        float FrameSamplingIncrement;

        if(FrameTexture.texture.width > FrameTexture.texture.height)
        {
            FrameSamplingStartIndex = new Vector2Int((FrameTexture.texture.width - FrameTexture.texture.height) / 2, 0);
            FrameSamplingIncrement = (float)FrameTexture.texture.height / TextureResolution;
        }
        else
        {
            FrameSamplingStartIndex = new Vector2Int((FrameTexture.texture.height - FrameTexture.texture.width) / 2, 0);
            FrameSamplingIncrement = (float)FrameTexture.texture.width / TextureResolution;
        }

        Vector2Int NamePlacardSamplingStartIndex;
        float NamePlacardSamplingIncrement;

        if (NamePlacardTexture.texture.width > NamePlacardTexture.texture.height)
        {
            NamePlacardSamplingStartIndex = new Vector2Int((NamePlacardTexture.texture.width - NamePlacardTexture.texture.height) / 2, 0);
            NamePlacardSamplingIncrement = (float)NamePlacardTexture.texture.height / TextureResolution;
        }
        else
        {
            NamePlacardSamplingStartIndex = new Vector2Int((NamePlacardTexture.texture.height - NamePlacardTexture.texture.width) / 2, 0);
            NamePlacardSamplingIncrement = (float)NamePlacardTexture.texture.width / TextureResolution;
        }

        Vector2Int TypePlacardSamplingStartIndex;
        float TypePlacardSamplingIncrement;

        if (TypePlacardTexture.texture.width > TypePlacardTexture.texture.height)
        {
            TypePlacardSamplingStartIndex = new Vector2Int((TypePlacardTexture.texture.width - TypePlacardTexture.texture.height) / 2, 0);
            TypePlacardSamplingIncrement = (float)TypePlacardTexture.texture.height / TextureResolution;
        }
        else
        {
            TypePlacardSamplingStartIndex = new Vector2Int((TypePlacardTexture.texture.height - TypePlacardTexture.texture.width) / 2, 0);
            TypePlacardSamplingIncrement = (float)TypePlacardTexture.texture.width / TextureResolution;
        }

        Vector2Int NSamplingStartIndex;
        float NSamplingIncrement;

        if (NTexture.texture.width > NTexture.texture.height)
        {
            NSamplingStartIndex = new Vector2Int((NTexture.texture.width - NTexture.texture.height) / 2, 0);
            NSamplingIncrement = (float)NTexture.texture.height / TextureResolution;
        }
        else
        {
            NSamplingStartIndex = new Vector2Int((NTexture.texture.height - NTexture.texture.width) / 2, 0);
            NSamplingIncrement = (float)NTexture.texture.width / TextureResolution;
        }

        Vector2Int ESamplingStartIndex;
        float ESamplingIncrement;

        if (ETexture.texture.width > ETexture.texture.height)
        {
            ESamplingStartIndex = new Vector2Int((ETexture.texture.width - ETexture.texture.height) / 2, 0);
            ESamplingIncrement = (float)ETexture.texture.height / TextureResolution;
        }
        else
        {
            ESamplingStartIndex = new Vector2Int((ETexture.texture.height - ETexture.texture.width) / 2, 0);
            ESamplingIncrement = (float)ETexture.texture.width / TextureResolution;
        }

        Vector2Int SACSamplingStartIndex;
        float SACSamplingIncrement;

        if (SACTexture.texture.width > SACTexture.texture.height)
        {
            SACSamplingStartIndex = new Vector2Int((SACTexture.texture.width - SACTexture.texture.height) / 2, 0);
            SACSamplingIncrement = (float)SACTexture.texture.height / TextureResolution;
        }
        else
        {
            SACSamplingStartIndex = new Vector2Int((SACTexture.texture.height - SACTexture.texture.width) / 2, 0);
            SACSamplingIncrement = (float)SACTexture.texture.width / TextureResolution;
        }

        Vector2Int WSamplingStartIndex;
        float WSamplingIncrement;

        if (WTexture.texture.width > WTexture.texture.height)
        {
            WSamplingStartIndex = new Vector2Int((WTexture.texture.width - WTexture.texture.height) / 2, 0);
            WSamplingIncrement = (float)WTexture.texture.height / TextureResolution;
        }
        else
        {
            WSamplingStartIndex = new Vector2Int((WTexture.texture.height - WTexture.texture.width) / 2, 0);
            WSamplingIncrement = (float)WTexture.texture.width / TextureResolution;
        }

        Vector2Int RSamplingStartIndex;
        float RSamplingIncrement;

        if (RTexture.texture.width > RTexture.texture.height)
        {
            RSamplingStartIndex = new Vector2Int((RTexture.texture.width - RTexture.texture.height) / 2, 0);
            RSamplingIncrement = (float)RTexture.texture.height / TextureResolution;
        }
        else
        {
            RSamplingStartIndex = new Vector2Int((RTexture.texture.height - RTexture.texture.width) / 2, 0);
            RSamplingIncrement = (float)RTexture.texture.width / TextureResolution;
        }

        Vector2Int DSamplingStartIndex;
        float DSamplingIncrement;

        if (DTexture.texture.width > DTexture.texture.height)
        {
            DSamplingStartIndex = new Vector2Int((DTexture.texture.width - DTexture.texture.height) / 2, 0);
            DSamplingIncrement = (float)DTexture.texture.height / TextureResolution;
        }
        else
        {
            DSamplingStartIndex = new Vector2Int((DTexture.texture.height - DTexture.texture.width) / 2, 0);
            DSamplingIncrement = (float)DTexture.texture.width / TextureResolution;
        }

        Vector2Int CharacterArtSamplingStartIndex;
        float CharacterArtSamplingIncrement;

        if (CharacterArt.texture.width > CharacterArt.texture.height)
        {
            CharacterArtSamplingStartIndex = new Vector2Int((CharacterArt.texture.width - CharacterArt.texture.height) / 2, 0);
            CharacterArtSamplingIncrement = CharacterArt.texture.height * 24.0f / (TextureResolution * 17.0f);
            CharacterArtSamplingStartIndex += (int)(CharacterArt.texture.height / 17.0f) * Vector2Int.right;
            CharacterArtSamplingStartIndex += (int)(CharacterArt.texture.height * 2.0f / 17.0f) * Vector2Int.down;
        }
        else
        {
            CharacterArtSamplingStartIndex = new Vector2Int((CharacterArt.texture.height - CharacterArt.texture.width) / 2, 0);
            CharacterArtSamplingIncrement = CharacterArt.texture.width * 24.0f / (TextureResolution * 17.0f);
            CharacterArtSamplingStartIndex += (int)(CharacterArt.texture.width / 17.0f) * Vector2Int.right;
            CharacterArtSamplingStartIndex += (int)(CharacterArt.texture.width * 2.0f / 17.0f) * Vector2Int.down;
        }

        Vector2Int EffectTextSamplingStartIndex;
        float EffectTextSamplingIncrement;

        if (EffectTextTexture.texture.width > EffectTextTexture.texture.height)
        {
            EffectTextSamplingStartIndex = new Vector2Int((EffectTextTexture.texture.width - EffectTextTexture.texture.height) / 2, 0);
            EffectTextSamplingIncrement = (float)EffectTextTexture.texture.height / TextureResolution;
        }
        else
        {
            EffectTextSamplingStartIndex = new Vector2Int((EffectTextTexture.texture.height - EffectTextTexture.texture.width) / 2, 0);
            EffectTextSamplingIncrement = (float)EffectTextTexture.texture.width / TextureResolution;
        }

        Vector2Int CardBackSamplingStartIndex;
        float CardBackSamplingIncrement;

        if (CardBackTexture.texture.width > CardBackTexture.texture.height)
        {
            CardBackSamplingStartIndex = new Vector2Int((CardBackTexture.texture.width - CardBackTexture.texture.height) / 2, 0);
            CardBackSamplingIncrement = (float)CardBackTexture.texture.height / TextureResolution;
        }
        else
        {
            CardBackSamplingStartIndex = new Vector2Int((CardBackTexture.texture.height - CardBackTexture.texture.width) / 2, 0);
            CardBackSamplingIncrement = (float)CardBackTexture.texture.width / TextureResolution;
        }

        for (int x = 0; x < TextureResolution; x++)
        {
            for (int y = 0; y < TextureResolution; y++)
            {
                Vector2Int position;
                Vector2Int samplePosition;

                //Frame texture
                position = new Vector2Int(x, y);
                samplePosition = new Vector2Int(FrameSamplingStartIndex.x + (int)(FrameSamplingIncrement * x), FrameSamplingStartIndex.y + (int)(FrameSamplingIncrement * y));
                Color frameColor = FrameTexture.texture.GetPixel(samplePosition.x, samplePosition.y);
                if (ApplyStatColors)
                {
                    Vector2 normalizedXY = new Vector2(((float)x / TextureResolution - FrameThickness) / (1.0f - 2.0f * FrameThickness), ((float)y / TextureResolution - FrameThickness) / (1.0f - 2.0f * FrameThickness));
                    if (normalizedXY.y > 7.1f / 12.0f || normalizedXY.y < 4.9f / 12.0f)
                    {
                        if (HasW && normalizedXY.x < 1.0f / 9.3f && normalizedXY.x > (4.97f / 12.0f) * (12.0f - 24.0f * normalizedXY.y) * (1.0f / 24.0f) && normalizedXY.x > -(4.97f / 12.0f) * (12.0f - 24.0f * normalizedXY.y) * (1.0f / 24.0f))
                        {
                            Recolor(ref frameColor, WColor);
                        }
                        else if (HasE && normalizedXY.x > 1.0f - 1.0f / 9.3f && normalizedXY.x - 1.0f < (4.97f / 12.0f) * (12.0f - 24.0f * normalizedXY.y) * (1.0f / 24.0f) && normalizedXY.x - 1.0f < -(4.97f / 12.0f) * (12.0f - 24.0f * normalizedXY.y) * (1.0f / 24.0f))
                        {
                            Recolor(ref frameColor, EColor);
                        }
                    }
                    if (normalizedXY.x > 7.1f / 12.0f || normalizedXY.x < 4.9f / 12.0f)
                    {
                        if (HasSAC && normalizedXY.y < 1.0f / 9.3f && normalizedXY.y > (4.97f / 12.0f) * (12.0f - 24.0f * normalizedXY.x) * (1.0f / 24.0f) && normalizedXY.y > -(4.97f / 12.0f) * (12.0f - 24.0f * normalizedXY.x) * (1.0f / 24.0f))
                        {
                            Recolor(ref frameColor, SACColor);
                        }
                        else if (HasN && normalizedXY.y > 1.0f - 1.0f / 9.3f && normalizedXY.y - 1.0f < (4.97f / 12.0f) * (12.0f - 24.0f * normalizedXY.x) * (1.0f / 24.0f) && normalizedXY.y - 1.0f < -(4.97f / 12.0f) * (12.0f - 24.0f * normalizedXY.x) * (1.0f / 24.0f))
                        {
                            Recolor(ref frameColor, NColor);
                        }
                    }
                }
                newTexture.SetPixel(position.x, position.y, frameColor);
                metalness.SetPixel(position.x, position.y, new Color(FrameMetallic, 0.0f, 0.0f, FrameGloss));

                //Name placard texture
                position += TextureResolution * Vector2Int.right;
                samplePosition = new Vector2Int(NamePlacardSamplingStartIndex.x + (int)(NamePlacardSamplingIncrement * x), NamePlacardSamplingStartIndex.y + (int)(NamePlacardSamplingIncrement * y));
                newTexture.SetPixel(position.x, position.y, NamePlacardTexture.texture.GetPixel(samplePosition.x, samplePosition.y));
                metalness.SetPixel(position.x, position.y, new Color(NamePlacardMetallic, 0.0f, 0.0f, NamePlacardGloss));

                //Type placard texture
                position += TextureResolution * Vector2Int.right;
                samplePosition = new Vector2Int(TypePlacardSamplingStartIndex.x + (int)(TypePlacardSamplingIncrement * x), TypePlacardSamplingStartIndex.y + (int)(TypePlacardSamplingIncrement * y));
                newTexture.SetPixel(position.x, position.y, TypePlacardTexture.texture.GetPixel(samplePosition.x, samplePosition.y));
                metalness.SetPixel(position.x, position.y, new Color(TypePlacardMetallic, 0.0f, 0.0f, TypePlacardGloss));

                //Stats placards texture
                position = new Vector2Int(x, TextureResolution + y);
                Vector2Int statsStartIndex;
                float statsIncrement;
                Sprite statsTexture;
                if(x < TextureResolution / 3)
                {
                    if(y > TextureResolution / 3)
                    {
                        statsStartIndex = WSamplingStartIndex;
                        statsIncrement = WSamplingIncrement;
                        statsTexture = WTexture;
                    }
                    else
                    {
                        statsStartIndex = DSamplingStartIndex;
                        statsIncrement = DSamplingIncrement;
                        statsTexture = DTexture;
                    }
                }
                else if (x < 2 * TextureResolution / 3)
                {
                    if(y > TextureResolution / 2)
                    {
                        statsStartIndex = NSamplingStartIndex;
                        statsIncrement = NSamplingIncrement;
                        statsTexture = NTexture;
                    }
                    else
                    {
                        statsStartIndex = SACSamplingStartIndex;
                        statsIncrement = SACSamplingIncrement;
                        statsTexture = SACTexture;
                    }
                }
                else
                {
                    if (y > TextureResolution / 3)
                    {
                        statsStartIndex = ESamplingStartIndex;
                        statsIncrement = ESamplingIncrement;
                        statsTexture = ETexture;
                    }
                    else
                    {
                        statsStartIndex = RSamplingStartIndex;
                        statsIncrement = RSamplingIncrement;
                        statsTexture = RTexture;
                    }
                }
                samplePosition = new Vector2Int(statsStartIndex.x + (int)(statsIncrement * x), statsStartIndex.y + (int)(statsIncrement * y));
                newTexture.SetPixel(position.x, position.y, statsTexture.texture.GetPixel(samplePosition.x, samplePosition.y));
                metalness.SetPixel(position.x, position.y, new Color(StatsMetallic, 0.0f, 0.0f, StatsGloss));

                //Art and effect text texture
                position += TextureResolution * Vector2Int.right;
                Vector2Int frontStartIndex = EffectTextSamplingStartIndex;
                float frontIncrement = EffectTextSamplingIncrement;
                Sprite frontTexture = EffectTextTexture;
                Color frontMetallic = new Color(EffectTextMetallic, 0.0f, 0.0f, EffectTextGloss);
                if(x < TextureResolution * 0.5f * (1 - FrameThickness))
                {
                    if (y > TextureResolution / 12.0f && y < 19.0f * TextureResolution / 24.0f)
                    {
                        frontStartIndex = CharacterArtSamplingStartIndex;
                        frontIncrement = CharacterArtSamplingIncrement;
                        frontTexture = CharacterArt;
                        frontMetallic = new Color(CharacterArtMetallic, 0.0f, 0.0f, CharacterArtGloss);
                    }
                }
                else if(x < TextureResolution * 0.5f * (1 + FrameThickness))
                {
                    frontStartIndex = FrameSamplingStartIndex;
                    frontIncrement = FrameSamplingIncrement;
                    frontTexture = FrameTexture;
                    frontMetallic = new Color(FrameMetallic, 0.0f, 0.0f, FrameGloss);
                }
                samplePosition = new Vector2Int(frontStartIndex.x + (int)(frontIncrement * x), frontStartIndex.y + (int)(frontIncrement * y));
                newTexture.SetPixel(position.x, position.y, frontTexture.texture.GetPixel(samplePosition.x, samplePosition.y));
                metalness.SetPixel(position.x, position.y, frontMetallic);

                //Card back texture
                position += TextureResolution * Vector2Int.right;
                samplePosition = new Vector2Int(CardBackSamplingStartIndex.x + (int)(CardBackSamplingIncrement * x), CardBackSamplingStartIndex.y + (int)(CardBackSamplingIncrement * y));
                newTexture.SetPixel(position.x, position.y, CardBackTexture.texture.GetPixel(samplePosition.x, samplePosition.y));
                metalness.SetPixel(position.x, position.y, new Color(CardBackMetallic, 0.0f, 0.0f, CardBackGloss));
            }
        }

        newTexture.Apply();
        metalness.Apply();
        return new List<Texture2D> { newTexture, metalness };
    }

    private void ApplyTexture(List<Texture2D> newTextures)
    {
        Material mat = gameObject.GetComponent<MeshRenderer>().sharedMaterial;
        if(mat == null)
        {
            mat = new Material(BaseMaterial);
        }
        mat.SetTexture("_MainTex", newTextures[0]);
        mat.SetTexture("_MetallicGlossMap", newTextures[1]);
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
        target = Color.HSVToRGB(nH, 0.8f * nS + 0.2f * tS, tV * nV);
    }
}
