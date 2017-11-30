// Only works on ARGB32, RGB24 and Alpha8 textures that are marked READABLE
// Source: https://wiki.unity3d.com/index.php?title=TextureScale#TextureScale.cs
using UnityEngine;

public static class TextureScale {

    private static Color[] _texColors;
    private static Color[] _newColors;
    private static float _ratioX;
    private static float _ratioY;
    private static int _width;
    private static int _newWidth;


    public static Texture2D Bilinear(Texture2D tex, int newWidth, int newHeight) {
        _texColors = tex.GetPixels();
        _newColors = new Color[newWidth * newHeight];

        _ratioX = 1.0f / ((float)newWidth / (tex.width - 1));
        _ratioY = 1.0f / ((float)newHeight / (tex.height - 1));

        _width = tex.width;
        _newWidth = newWidth;

        BilinearScale(newHeight);

        Texture2D newTex = new Texture2D(newWidth, newHeight);
        newTex.SetPixels(_newColors);
        newTex.Apply();

        _texColors = null;
        _newColors = null;

        return newTex;
    }

    private static void BilinearScale(int newHeight) {
        for (int y = 0; y < newHeight; y++) {
            int yFloor = (int)Mathf.Floor(y * _ratioY);
            int y1 = yFloor * _width;
            int y2 = (yFloor + 1) * _width;
            int yw = y * _newWidth;

            for (int x = 0; x < _newWidth; x++) {
                int xFloor = (int)Mathf.Floor(x * _ratioX);
                float xLerp = x * _ratioX - xFloor;
                _newColors[yw + x] = ColorLerpUnclamped(
                    ColorLerpUnclamped(_texColors[y1 + xFloor], _texColors[y1 + xFloor + 1], xLerp),
                    ColorLerpUnclamped(_texColors[y2 + xFloor], _texColors[y2 + xFloor + 1], xLerp),
                    y * _ratioY - yFloor);
            }
        }
    }

    private static Color ColorLerpUnclamped(Color c1, Color c2, float value) {
        return new Color(c1.r + (c2.r - c1.r) * value,
            c1.g + (c2.g - c1.g) * value,
            c1.b + (c2.b - c1.b) * value,
            c1.a + (c2.a - c1.a) * value);
    }
}
