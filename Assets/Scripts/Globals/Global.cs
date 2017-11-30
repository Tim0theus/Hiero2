using UnityEngine;

public static class Global {
    public static class Constants {
        public const float TouchDistance = 6;
    }

    public static class Colors {
        public static Color HighlightYellow {
            get { return new Color(1f, 0.87f, 0f); }
        }

        public static Color PickerGlyph {
            get { return Color.white; }
        }

        public static Color GlyphWhite {
            get { return Color.white; }
        }
    }

    public static Color Transparent(this Color color) {
        return new Color(color.r, color.g, color.b, 0);
    }
}


