using UnityEngine;

public abstract class UIControl : Activatable {
    public float FadeDuration = 0.1f;
    public bool StartInactive;

    public Color InactiveColor = Color.white.Transparent();
}

public abstract class HighlightableUIControl : UIControl {
    public Color HighlightColor = Global.Colors.HighlightYellow;

    public abstract void Highlight();
    public abstract void DeHighlight();
}