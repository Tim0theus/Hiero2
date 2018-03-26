using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GroupControl : HighlightableUIControl, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, ISubmitHandler, IPointerDownHandler, IPointerUpHandler, IMoveHandler {
    public Slider Slider;
    public Toggle Toggle;

    private Selectable _selectable;
    private Image _image;

    private List<HighlightableUIControl> _uiElements;

    public void OnPointerEnter(PointerEventData eventData) {
        Highlight();
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (_selectable.interactable) DeHighlight();
    }

    public void OnPointerDown(PointerEventData eventData) { }

    public void OnPointerUp(PointerEventData eventData) {
        if (Toggle) Toggle.OnPointerClick(eventData);
    }

    public void OnMove(AxisEventData eventData) {
        if (Slider) Slider.OnMove(eventData);
    }

    public void OnSelect(BaseEventData eventData) {
        Highlight();
    }

    public void OnDeselect(BaseEventData eventData) {
        if (_selectable.interactable) DeHighlight();
    }

    public void OnSubmit(BaseEventData eventData) {
        if (Toggle) Toggle.OnSubmit(eventData);
    }

    private void Awake() {
        _uiElements = gameObject.GetComponentsInChildren<HighlightableUIControl>().ToList();
        _uiElements.Remove(this);

        _image = GetComponent<Image>();
        _selectable = GetComponent<Selectable>();
    }

    private void Start()
    {
        if (StartInactive) DeActivate();
    }

    public override void Activate() {
        foreach (HighlightableUIControl uiElement in _uiElements) {
            uiElement.Activate();
        }

        _image.raycastTarget = true;
        _selectable.interactable = true;

        enabled = true;
    }

    public override void DeActivate() {
        foreach (HighlightableUIControl uiElement in _uiElements) {
            uiElement.DeActivate();
        }

        _image.raycastTarget = false;
        _selectable.interactable = false;

        enabled = false;
    }

    public override void Highlight() {
        foreach (HighlightableUIControl uiElement in _uiElements) {
            uiElement.Highlight();
        }
    }

    public override void DeHighlight() {
        foreach (HighlightableUIControl uiElement in _uiElements) {
            uiElement.DeHighlight();
        }
    }
}