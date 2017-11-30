using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public enum Fit {
    Small,
    Flat,
    Tall,
    Big,
    Undefined
}

public static class ResourceLoader {
    private static readonly Dictionary<string, ExtendedGlyph> ExtendedHieroglyphs = new Dictionary<string, ExtendedGlyph>();

    private static readonly Dictionary<string, Sprite> HieroglyphSprites = new Dictionary<string, Sprite>();
    private static readonly Dictionary<string, Sprite> TransliterationSprites = new Dictionary<string, Sprite>();

    static ResourceLoader() {
        Sprite[] hieroglyphTextures = Resources.LoadAll<Sprite>("Glyphs/Hieroglyphs");
        foreach (Sprite sprite in hieroglyphTextures) {
            sprite.texture.mipMapBias = -0.25f;
            HieroglyphSprites.Add(sprite.name, sprite);
        }

        Sprite[] transliterationTextures = Resources.LoadAll<Sprite>("Glyphs/Transliterations");
        foreach (Sprite sprite in transliterationTextures) {
            sprite.texture.mipMapBias = -0.25f;
            TransliterationSprites.Add(sprite.name, sprite);
        }

        TextAsset uniliteralsContent = Resources.Load<TextAsset>("Mappings/Uniliterals");
        ProcessMapping(uniliteralsContent);

        TextAsset additionalHieroglyphs = Resources.Load<TextAsset>("Mappings/Hieroglyphs");
        ProcessMapping(additionalHieroglyphs);
    }

    private static void ProcessMapping(TextAsset textFileContent) {
        string text = textFileContent.text.Trim();
        string[] lines = text.Split('\n');

        foreach (string line in lines) {
            string[] elements = line.Split('\t');

            string glyphCode = elements[0].ToGlyphCode();
            Fit fit = elements[1].ToFit();
            string name = elements.Length > 2 ? elements[2] : glyphCode;
            string transliterationCode = elements.Length > 3 ? elements[3].ToGlyphCode() : string.Empty;
            int alphabeticalIndex = elements.Length > 4 ? int.Parse(elements[4]) : -1;

            ExtendedHieroglyphs.Add(glyphCode, new ExtendedGlyph {
                Name = name,
                Hieroglyph = HieroglyphSprites[glyphCode],
                Transliteration = TransliterationSprites.ContainsKey(transliterationCode) ? TransliterationSprites[transliterationCode] : null,
                Fit = fit,
                GlyphCode = glyphCode,
                AlphabeticIndex = alphabeticalIndex
            });
        }
    }

    public static bool IsHieroglyph(string value) {
        return ExtendedHieroglyphs.ContainsKey(value);
    }

    public static ExtendedGlyph Get(string glyphCode) {
        if (ExtendedHieroglyphs.ContainsKey(glyphCode))
            return ExtendedHieroglyphs[glyphCode];

        Debug.Log(string.IsNullOrEmpty(glyphCode)
            ? "GlyphCode is empty."
            : string.Format("Mapping does not contain code '{0}'.", glyphCode));

        return null;
    }

    private static string ToGlyphCode(this string value) {
        byte[] unicodeBytes = Encoding.UTF32.GetBytes(value);
        int utf32EntryPoint = BitConverter.ToInt32(unicodeBytes, 0);

        return string.Format("{0:x}", utf32EntryPoint);
    }

    private static Fit ToFit(this string value) {
        switch (value) {
            case "big": return Fit.Big;
            case "tall": return Fit.Tall;
            case "flat": return Fit.Flat;
            case "small": return Fit.Small;
            default: return Fit.Undefined;
        }
    }
}
