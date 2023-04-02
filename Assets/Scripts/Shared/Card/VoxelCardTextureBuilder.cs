using UnityEngine;

public static class VoxelCardTextureBuilder
{

    public const float CharacterArtLowerBoundBase = 0f;//3.0f / 12.0f;
    public const float CharacterArtUpperBoundFullArt = 11.0f / 12.0f;
    public const float CharacterArtUpperBoundHasText = 1f;// 19.0f / 24.0f;

    public const float CharacterArtSamplingIncrementRatioFullArt = 1f;//24.0f / 19.0f;
    public const float CharacterArtSamplingIncrementRatioHasText = 1f;//24.0f / 21.0f;

    public const float CharacterArtSamplingStartIndexRatioBase = 1.0f / 17.0f;

    private const string MainTextureName = "_MainTex";
    private const string MainMetalnessName = "_MetallicGlossMap";

    public static float GetCharacterArtSamplingIncrementRatio(bool fullArt) => fullArt
        ? CharacterArtSamplingIncrementRatioFullArt
        : CharacterArtSamplingIncrementRatioHasText;
        
    public static float CharacterArtLowerBound => CharacterArtLowerBoundBase;
    public static float GetCharacterArtUpperBound(bool fullArt) => fullArt ? CharacterArtUpperBoundFullArt : CharacterArtUpperBoundHasText;

    public class TextureParams
    {
        public static TextureParams Default => new TextureParams()
        {
            TextureResolution = 256,

            //FrameThickness = 0.0225f,

            HasSAC = true, //Duh
            ApplyStatColors = true,

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

        public class Textures
        {
            public Sprite FrameTexture;
            public Color32? FrameColorOverride;

            public Sprite NamePlacardTexture;
            public Sprite TypePlacardTexture;
            public Sprite EffectTextTexture;
            public Sprite CharacterArt;
            public Sprite CardBackTexture;
            public Sprite NTexture;
            public Sprite ETexture;
            public Sprite SACTexture;
            public Sprite WTexture;
            public Sprite RTexture;
            public Sprite DTexture;

            public Color GetFrameColor(int x, int y) => FrameColorOverride ?? FrameTexture.texture.GetPixel(x, y);
        }

        public static TextureParams Params(bool isZoomed, bool isChar, Textures textures)
        {
            var ret = isZoomed ? DefaultZoomed : DefaultUnzoomed;

            ret.textures = textures;
            ret.HasN = ret.HasE = ret.HasW = isChar;

            return ret;
        }

        public Textures textures;

        public int TextureResolution;
        public float CharacterArtSamplingIncrementRatio;
        public bool fullArt;

        public bool ApplyStatColors;
        public float FrameThickness;

        public bool HasN;
        public bool HasE;
        public bool HasSAC;
        public bool HasW;
        public bool HasR;
        public bool HasD;

        public Color NColor;
        public Color EColor;
        public Color SACColor;
        public Color WColor;

        public float FrameMetallic;
        public float FrameGloss;
        public float NamePlacardMetallic;
        public float NamePlacardGloss;
        public float TypePlacardMetallic;
        public float TypePlacardGloss;
        public float StatsMetallic;
        public float StatsGloss;
        public float EffectTextMetallic;
        public float EffectTextGloss;
        public float CharacterArtMetallic;
        public float CharacterArtGloss;
        public float CardBackMetallic;
        public float CardBackGloss;
        public float CharacterArtUpperBound;
    }

    public static (Texture2D, Texture2D) BuildTexture(Texture2D oldTexture, Texture2D oldMetalness, TextureParams textureParams, bool rebuildAll = true)
    {
        Texture2D newTexture = rebuildAll || oldTexture == default
            ? new Texture2D(textureParams.TextureResolution * 3, textureParams.TextureResolution * 2, TextureFormat.ARGB32, false)
            : oldTexture;
        Texture2D metalness = rebuildAll || oldMetalness == default
            ? new Texture2D(textureParams.TextureResolution * 3, textureParams.TextureResolution * 2, TextureFormat.ARGB32, false)
            : oldMetalness;

        (Vector2Int FrameSamplingStartIndex, float FrameSamplingIncrement) = SamplingInformation(textureParams.textures.FrameTexture, textureParams);
        (Vector2Int NamePlacardSamplingStartIndex, float NamePlacardSamplingIncrement) = SamplingInformation(textureParams.textures.NamePlacardTexture, textureParams);
        (Vector2Int TypePlacardSamplingStartIndex, float TypePlacardSamplingIncrement) = SamplingInformation(textureParams.textures.TypePlacardTexture, textureParams);

        var charArtTex = textureParams.textures.CharacterArt?.texture ?? textureParams.textures.NamePlacardTexture.texture;
        int squaringFactor = Mathf.Abs(charArtTex.width - charArtTex.height) / 2;
        float shorterDimension = Mathf.Min(charArtTex.width, charArtTex.height);

        float CharacterArtSamplingIncrement = (shorterDimension / (float)textureParams.TextureResolution) * textureParams.CharacterArtSamplingIncrementRatio;
        var CharacterArtSamplingStartIndexRatio = GetCharacterArtSamplingIncrementRatio(fullArt: textureParams.fullArt);
        Vector2Int CharacterArtSamplingStartIndex = textureParams.fullArt
            ? (Vector2Int.right * (int)(squaringFactor + (shorterDimension * CharacterArtSamplingStartIndexRatio)))
            + (Vector2Int.down * (int)(shorterDimension * 2.0f * CharacterArtSamplingStartIndexRatio))
            : (Vector2Int.right * 0)//(int)(squaringFactor + (shorterDimension * CharacterArtSamplingStartIndexRatio)))
            + (Vector2Int.down * (int)(shorterDimension * -2.0f * CharacterArtSamplingStartIndexRatio));
        //Debug.Log($"Character art {CharacterArtSamplingIncrement}, {CharacterArtSamplingStartIndex}");

        if (textureParams.fullArt) CharacterArtSamplingStartIndex = new Vector2Int(-2 * CharacterArtSamplingStartIndex.x, CharacterArtSamplingStartIndex.y);

        (Vector2Int EffectTextSamplingStartIndex, float EffectTextSamplingIncrement) = SamplingInformation(textureParams.textures.EffectTextTexture, textureParams);
        (Vector2Int CardBackSamplingStartIndex, float CardBackSamplingIncrement) = SamplingInformation(textureParams.textures.CardBackTexture, textureParams);

        Debug.Log($"Recolored color is from {textureParams.NColor}/{textureParams.EColor}/{textureParams.SACColor}/{textureParams.WColor}");
        for (int x = 0; x < textureParams.TextureResolution; x++)
        {
            for (int y = 0; y < textureParams.TextureResolution; y++)
            {
                Vector2Int position = new Vector2Int(x, y);
                Vector2Int samplePosition;

                //Frame texture
                samplePosition = new Vector2Int(FrameSamplingStartIndex.x + (int)(FrameSamplingIncrement * x), FrameSamplingStartIndex.y + (int)(FrameSamplingIncrement * y));
                Color frameColor = textureParams.textures.GetFrameColor(samplePosition.x, samplePosition.y);
                var textureResolutionOffset = 0.0225f; //Matches thin frame offset
                if (textureParams.ApplyStatColors)
                {
                    Vector2 normalizedXY = new Vector2(((float)x / textureParams.TextureResolution - textureResolutionOffset) / (1.0f - 2.0f * textureResolutionOffset), ((float)y / textureParams.TextureResolution - textureResolutionOffset) / (1.0f - 2.0f * textureResolutionOffset));
                    if (normalizedXY.y > 7.1f / 12.0f || normalizedXY.y < 4.9f / 12.0f)
                    {
                        if (textureParams.HasW && normalizedXY.x < 1.0f / 9.3f && normalizedXY.x > (4.97f / 12.0f) * (12.0f - 24.0f * normalizedXY.y) * (1.0f / 24.0f) && normalizedXY.x > -(4.97f / 12.0f) * (12.0f - 24.0f * normalizedXY.y) * (1.0f / 24.0f))
                        {
                            frameColor = Recolor(frameColor, textureParams.WColor);
                            //Debug.Log($"Recoloring W to {frameColor}");
                            //break;
                        }
                        else if (textureParams.HasE && normalizedXY.x > 1.0f - 1.0f / 9.3f && normalizedXY.x - 1.0f < (4.97f / 12.0f) * (12.0f - 24.0f * normalizedXY.y) * (1.0f / 24.0f) && normalizedXY.x - 1.0f < -(4.97f / 12.0f) * (12.0f - 24.0f * normalizedXY.y) * (1.0f / 24.0f))
                        {
                            frameColor = Recolor(frameColor, textureParams.EColor);
                            //Debug.Log($"Recoloring E to {frameColor}");
                            //break;
                        }
                    }
                    if (normalizedXY.x > 7.1f / 12.0f || normalizedXY.x < 4.9f / 12.0f)
                    {
                        if (textureParams.HasSAC && normalizedXY.y < 1.0f / 9.3f && normalizedXY.y > (4.97f / 12.0f) * (12.0f - 24.0f * normalizedXY.x) * (1.0f / 24.0f) && normalizedXY.y > -(4.97f / 12.0f) * (12.0f - 24.0f * normalizedXY.x) * (1.0f / 24.0f))
                        {
                            frameColor = Recolor(frameColor, textureParams.SACColor);
                            //Debug.Log($"Recoloring S to {frameColor}");
                            //break;
                        }
                        else if (textureParams.HasN && normalizedXY.y > 1.0f - 1.0f / 9.3f && normalizedXY.y - 1.0f < (4.97f / 12.0f) * (12.0f - 24.0f * normalizedXY.x) * (1.0f / 24.0f) && normalizedXY.y - 1.0f < -(4.97f / 12.0f) * (12.0f - 24.0f * normalizedXY.x) * (1.0f / 24.0f))
                        {
                            frameColor = Recolor(frameColor, textureParams.NColor);
                            //Debug.Log($"Recoloring N to {frameColor}");
                            //break;
                        }
                    }
                }
                newTexture.SetPixel(position.x, position.y, frameColor);
                metalness.SetPixel(position.x, position.y, new Color(textureParams.FrameMetallic, 0.0f, 0.0f, textureParams.FrameGloss));

                //continue;

                void Paint(Vector2Int startIndex, float samplingIncrement, Sprite sprite, float metallic, float gloss)
                {
                    samplePosition = new Vector2Int(startIndex.x + (int)(samplingIncrement * x), startIndex.y + (int)(samplingIncrement * y));
                    newTexture.SetPixel(position.x, position.y, sprite.texture.GetPixel(samplePosition.x, samplePosition.y));
                    metalness.SetPixel(position.x, position.y, new Color(metallic, 0.0f, 0.0f, gloss));
                }

                //Name placard texture
                position += textureParams.TextureResolution * Vector2Int.right;
                Paint(NamePlacardSamplingStartIndex, NamePlacardSamplingIncrement, textureParams.textures.NamePlacardTexture, textureParams.NamePlacardMetallic, textureParams.NamePlacardGloss);

                //Type placard texture
                position += textureParams.TextureResolution * Vector2Int.right;
                Paint(TypePlacardSamplingStartIndex, TypePlacardSamplingIncrement, textureParams.textures.TypePlacardTexture, textureParams.TypePlacardMetallic, textureParams.TypePlacardGloss);

                //Stats placards texture
                position = new Vector2Int(x, textureParams.TextureResolution + y);
                (Vector2Int statsStartIndex, float statsIncrement, Sprite statsTexture) = DetermineRelevantStatValues(x, y, textureParams);
                Paint(statsStartIndex, statsIncrement, statsTexture, textureParams.StatsMetallic, textureParams.StatsGloss);

                //Art and effect text texture
                position = new Vector2Int(textureParams.TextureResolution + x, textureParams.TextureResolution + y);
                Vector2Int frontStartIndex = EffectTextSamplingStartIndex;
                float frontIncrement = EffectTextSamplingIncrement;
                Sprite frontTexture = textureParams.textures.EffectTextTexture;
                Color frontMetallic = new Color(textureParams.EffectTextMetallic, 0.0f, 0.0f, textureParams.EffectTextGloss);
                float effTextZoneRatio = 0.37f;
                float artRightBound = textureParams.fullArt ? 1.0f : effTextZoneRatio * (1f + textureParams.FrameThickness);

                if (textureParams.fullArt)
                {
                    frontStartIndex = CharacterArtSamplingStartIndex;
                    frontIncrement = CharacterArtSamplingIncrement;
                    frontTexture = textureParams.textures.CharacterArt;
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
                            frontTexture = textureParams.textures.CharacterArt;
                            frontMetallic = new Color(textureParams.CharacterArtMetallic, 0.0f, 0.0f, textureParams.CharacterArtGloss);
                        }
                    }
                    else if (y > textureParams.TextureResolution * effTextZoneRatio * (1 - textureParams.FrameThickness))
                    {
                        frontStartIndex = FrameSamplingStartIndex;
                        frontIncrement = FrameSamplingIncrement;
                        frontTexture = textureParams.textures.FrameTexture;
                        frontMetallic = new Color(textureParams.FrameMetallic, 0.0f, 0.0f, textureParams.FrameGloss);
                    }
                }

                samplePosition = new Vector2Int(frontStartIndex.x + (int)(frontIncrement * x), frontStartIndex.y + (int)(frontIncrement * y));
                var pixel = frontTexture?.texture.GetPixel(samplePosition.x, samplePosition.y)
                    ?? textureParams.textures.FrameColorOverride
                    ?? Color.white;
                newTexture.SetPixel(position.x, position.y, pixel);
                metalness.SetPixel(position.x, position.y, frontMetallic);

                //Card back texture
                position += textureParams.TextureResolution * Vector2Int.right;
                samplePosition = new Vector2Int(CardBackSamplingStartIndex.x + (int)(CardBackSamplingIncrement * x), CardBackSamplingStartIndex.y + (int)(CardBackSamplingIncrement * y));
                newTexture.SetPixel(position.x, position.y, textureParams.textures.CardBackTexture.texture.GetPixel(samplePosition.x, samplePosition.y));
                metalness.SetPixel(position.x, position.y, new Color(textureParams.CardBackMetallic, 0.0f, 0.0f, textureParams.CardBackGloss));
            }
        }

        newTexture.Apply();
        metalness.Apply();
        return (newTexture, metalness);
    }

    private static (Vector2Int, float, Sprite) DetermineRelevantStatValues(int x, int y, TextureParams textureParams)
    {

        (Vector2Int NSamplingStartIndex, float NSamplingIncrement) = SamplingInformation(textureParams.textures.NTexture, textureParams);
        (Vector2Int ESamplingStartIndex, float ESamplingIncrement) = SamplingInformation(textureParams.textures.ETexture, textureParams);
        (Vector2Int SACSamplingStartIndex, float SACSamplingIncrement) = SamplingInformation(textureParams.textures.SACTexture, textureParams);
        (Vector2Int WSamplingStartIndex, float WSamplingIncrement) = SamplingInformation(textureParams.textures.WTexture, textureParams);
        (Vector2Int RSamplingStartIndex, float RSamplingIncrement) = SamplingInformation(textureParams.textures.RTexture, textureParams);
        (Vector2Int DSamplingStartIndex, float DSamplingIncrement) = SamplingInformation(textureParams.textures.DTexture, textureParams);

        if (x < textureParams.TextureResolution / 3)
        {
            if (y > textureParams.TextureResolution / 3)
            {
                return (WSamplingStartIndex, WSamplingIncrement, textureParams.textures.WTexture);
            }
            else
            {
                return (DSamplingStartIndex, DSamplingIncrement, textureParams.textures.DTexture);
            }
        }
        else if (x < 2 * textureParams.TextureResolution / 3)
        {
            if (y > textureParams.TextureResolution / 2)
            {
                return (NSamplingStartIndex, NSamplingIncrement, textureParams.textures.NTexture);
            }
            else
            {
                return (SACSamplingStartIndex, SACSamplingIncrement, textureParams.textures.SACTexture);
            }
        }
        else
        {
            if (y > textureParams.TextureResolution / 3)
            {
                return (ESamplingStartIndex, ESamplingIncrement, textureParams.textures.ETexture);
            }
            else
            {
                return (RSamplingStartIndex, RSamplingIncrement, textureParams.textures.RTexture);
            }
        }
    }

    private static (Vector2Int, float) SamplingInformation(Sprite sprite, TextureParams textureParams)
    {
        if (sprite == default) return (Vector2Int.zero, 0f);
        int maxDim = Mathf.Max(sprite.texture.height, sprite.texture.width);
        int minDim = Mathf.Min(sprite.texture.height, sprite.texture.width);

        return (new Vector2Int((maxDim - minDim) / 2, 0), (float)minDim / textureParams.TextureResolution);
    }

    public static float CharacterArtSamplingStartIndexRatio => CharacterArtSamplingStartIndexRatioBase;

    public static void BuildTextureCardArt(int TextureResolution, bool fullArt, float FrameThickness, float CharacterArtUpperBound,
        float CharacterArtSamplingIncrementRatio, Sprite CharacterArt, float CharacterArtMetallic, float CharacterArtGloss, Texture2D newTexture, Texture2D metalness)
    {
        int squaringFactor = Mathf.Abs(CharacterArt.texture.width - CharacterArt.texture.height) / 2;
        float shorterDimension = Mathf.Min(CharacterArt.texture.width, CharacterArt.texture.height);

        float CharacterArtSamplingIncrement = (shorterDimension / (float)TextureResolution) * CharacterArtSamplingIncrementRatio;
        Vector2Int CharacterArtSamplingStartIndex =
            (Vector2Int.right * (int)(squaringFactor + (shorterDimension * CharacterArtSamplingStartIndexRatio)))
            + (Vector2Int.down * (int)(shorterDimension * 2.0f * CharacterArtSamplingStartIndexRatio));
        //Debug.Log($"Character art {CharacterArtSamplingIncrement}, {CharacterArtSamplingStartIndex}");

        if (fullArt) CharacterArtSamplingStartIndex = new Vector2Int(-2 * CharacterArtSamplingStartIndex.x, CharacterArtSamplingStartIndex.y);
        Debug.Log($"Character art sampling increment {CharacterArtSamplingIncrement}, ratio {CharacterArtSamplingStartIndexRatio} start index {CharacterArtSamplingStartIndex}");

        float effTextZoneRatio = 0.37f;
        float artRightBound = fullArt ? 1.0f : effTextZoneRatio * (1f + FrameThickness);

        Color frontMetallic = new Color(CharacterArtMetallic, 0.0f, 0.0f, CharacterArtGloss);
        Debug.Log($"Bounds {artRightBound}, {CharacterArtLowerBound}, {CharacterArtUpperBound}\n"
            + $"Aka {TextureResolution * artRightBound}, {TextureResolution * CharacterArtLowerBound}x{TextureResolution * CharacterArtUpperBound}");

        string samplingPositions = "";
        string samplingPositionsY = "";

        for (int x = TextureResolution; x < 2 * TextureResolution; x++)
        {
            for (int y = TextureResolution; y < 2 * TextureResolution; y++)
            {
                Vector2Int? samplePosition = null;
                //Art and effect text texture
                if (fullArt)
                {
                    samplePosition = new Vector2Int(CharacterArtSamplingStartIndex.x + (int)(CharacterArtSamplingIncrement * x),
                        CharacterArtSamplingStartIndex.y + (int)(CharacterArtSamplingIncrement * y));
                }
                else if (y > TextureResolution * artRightBound)
                {
                    if (x > TextureResolution * CharacterArtLowerBound && x < TextureResolution * CharacterArtUpperBound)
                    {
                        samplePosition = new Vector2Int(CharacterArtSamplingStartIndex.x + (int)(CharacterArtSamplingIncrement * x), CharacterArtSamplingStartIndex.y + (int)(CharacterArtSamplingIncrement * y));
                    }
                }
                if (samplePosition.HasValue)
                {
                    newTexture.SetPixel(x, y, CharacterArt.texture.GetPixel(samplePosition.Value.x, samplePosition.Value.y));
                    metalness.SetPixel(x, y, frontMetallic);
                }
            }
        }
        //Debug.Log(samplingPositions);
        //Debug.Log(samplingPositionsY);
        newTexture.Apply();
        metalness.Apply();
    }

    public static Color Recolor(Color target, Color newColor)
    {
        //Color.RGBToHSV(target, out float tH, out float tS, out float tV);
        //Color.RGBToHSV(newColor, out float nH, out float nS, out float nV);
        //target = Color.HSVToRGB(nH, 0.8f * nS + 0.2f * tS, tV * nV);
        return newColor;
    }
}