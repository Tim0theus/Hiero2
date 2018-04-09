#if UNIITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.EventSystems;

public static class Extentions {
    public static string ExtractGlyphName(this Renderer renderer) {
        Material[] materials = renderer.sharedMaterials;

        foreach (Material material in materials) {
            Texture texture = material.GetTexture("_MainTex");

            string textureName = string.Empty;
            if (texture) {
                textureName = texture.name;
            }

            if (ResourceLoader.IsHieroglyph(textureName)) {
                return textureName;
            }
        }

        Debug.LogWarning(renderer.transform.parent.name + " " + renderer.name + " has no valid Hieroglyph Texture set.");
        return string.Empty;
    }

    public static string ExtractGlyphFromMaterial(this Material material)
    {
        Texture texture = material.GetTexture("_MainTex");

        string textureName = string.Empty;
        if (texture)
        {
            textureName = texture.name;
        }

        if (ResourceLoader.IsHieroglyph(textureName))
        {
            return textureName;
        }

        Debug.LogWarning(material.name + " has no valid Hieroglyph Texture.");
        return string.Empty;
    }
}