using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FullscreenOverlay : UIControl {
    private Image _image;

    private void Awake() {
        _image = GetComponent<Image>();
        _image.color = NormalColor;

        if (StartInactive) {
            DeActivate(0);
        }
    }

    public override void Activate() {
        _image.raycastTarget = true;

        _image.CrossFadeColor(NormalColor, FadeDuration, false, true);
    }

    public override void DeActivate() {
        DeActivate(FadeDuration);
    }

    private void DeActivate(float fadeDuration) {
        _image.raycastTarget = false;

        _image.CrossFadeColor(InactiveColor, fadeDuration, false, true);
    }
}