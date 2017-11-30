using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VirtualJoystick : InputControlElement, IPointerDownHandler, IPointerUpHandler, IDragHandler {
    public Color ActiveColor = new Color(1, 1, 1, 0.25f);
    public Image Background;

    public Image Glyph;
    public Color InactiveColor = new Color(0, 0, 0, 0);
    public Image Joystick;
    public Image StickBase;

    private Fader _glyphFader;
    private Fader _backgroundFader;

    private Vector2 _origin;
    private float _maxLength;

    private Image _touchArea;

    public void OnDrag(PointerEventData eventData) {
        Vector2 position = eventData.position;

        position = ClampDistance(_origin, position, _maxLength);
        Vector = (position - _origin) / _maxLength;

        Joystick.rectTransform.position = position;
    }

    public void OnPointerDown(PointerEventData eventData) {
        _origin = eventData.position;

        Joystick.color = ActiveColor;
        Joystick.rectTransform.position = _origin;

        StickBase.color = ActiveColor;
        StickBase.rectTransform.position = _origin;

        _backgroundFader.DeActivate();
    }

    public void OnPointerUp(PointerEventData eventData) {
        Vector = Vector2.zero;

        Joystick.color = InactiveColor;
        StickBase.color = InactiveColor;
    }

    private void Awake() {
        StickBase.color = InactiveColor;
        Joystick.color = InactiveColor;


        _glyphFader = ImageFader.Create(Glyph, ActiveColor);
        _backgroundFader = ImageFader.Create(Background, ActiveColor, 3, 8);

        _touchArea = GetComponent<Image>();
        _touchArea.alphaHitTestMinimumThreshold = 0.1f;
    }

    private void Start() {
        _maxLength = StickBase.rectTransform.rect.width / 2;
    }

    private Vector2 ClampDistance(Vector2 startPoint, Vector2 endPoint, float maxDistance) {
        Vector2 distance = endPoint;
        if (Vector2.Distance(startPoint, endPoint) > maxDistance) {
            distance = endPoint - startPoint;
            distance.Normalize();
            return distance * maxDistance + startPoint;
        }
        return distance;
    }


    public override void Activate() {
        Joystick.enabled = true;
        StickBase.enabled = true;
        _glyphFader.Activate();
        _backgroundFader.Activate();
        _touchArea.enabled = true;
        enabled = true;
    }

    public override void DeActivate() {
        Joystick.enabled = false;
        StickBase.enabled = false;
        _glyphFader.DeActivate();
        _backgroundFader.DeActivate();
        _touchArea.enabled = false;
        enabled = false;

        Vector = Vector2.zero;
    }
}