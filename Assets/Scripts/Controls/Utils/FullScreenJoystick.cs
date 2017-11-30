using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FullScreenJoystick : InputControlElement, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {
    private Image _eventInput;
    private Vector2 _lastMousePosition;

    private readonly List<RaycastResult> _results = new List<RaycastResult>();
    private RaycastResult _raycastResult;

    private bool _forwardEvent;
    private GameObject _eventOutput;

    public void OnBeginDrag(PointerEventData eventData) {
        _forwardEvent = TryToForward(eventData, ExecuteEvents.beginDragHandler);
    }

    public void OnDrag(PointerEventData eventData) {
        if (_forwardEvent) {
            Forward(eventData, ExecuteEvents.dragHandler);
        }
        else {
            Vector.x = eventData.delta.x;
            Vector.y = eventData.delta.y;

            Vector = Vector / Screen.dpi * 5;
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (_forwardEvent) Forward(eventData, ExecuteEvents.endDragHandler);
        else Vector = Vector2.zero;
    }

    public void OnPointerDown(PointerEventData eventData) {
        _forwardEvent = TryToForward(eventData, ExecuteEvents.pointerDownHandler);
    }

    public void OnPointerUp(PointerEventData eventData) {
        if (_forwardEvent) Forward(eventData, ExecuteEvents.pointerUpHandler);
    }

    private void Awake() {
        _eventInput = GetComponent<Image>();
    }

    private void Start() {
        int current = EventSystem.current.pixelDragThreshold;
        EventSystem.current.pixelDragThreshold = Math.Max(current, (int)(current * Screen.dpi / 160f));
    }

    private void Update() {
        Vector2 mousePosition = Input.mousePosition;

        if ((mousePosition - _lastMousePosition).sqrMagnitude < 0.1f) Vector = Vector2.zero;

        _lastMousePosition = mousePosition;
    }

    private bool TryToForward<T>(PointerEventData eventData, ExecuteEvents.EventFunction<T> functor) where T : IEventSystemHandler {
        bool forward = false;
        _eventOutput = null;
        EventSystem.current.RaycastAll(eventData, _results);

        if (_results.Count > 1) {
            RaycastResult result = _results[1];

            if (result.distance < Global.Constants.TouchDistance) {
                _eventOutput = result.gameObject;
                _raycastResult = result;
            }

            if (_eventOutput != null) forward = Forward(eventData, functor);
        }

        return forward;
    }

    private bool Forward<T>(PointerEventData eventData, ExecuteEvents.EventFunction<T> functor) where T : IEventSystemHandler {
        if (_eventOutput != gameObject) {
            eventData.pointerCurrentRaycast = _raycastResult;
            return ExecuteEvents.Execute(_eventOutput, eventData, functor);
        }
        return false;
    }

    public override void Activate() {
        _eventInput.enabled = true;
        enabled = true;
    }

    public override void DeActivate() {
        _eventInput.enabled = false;
        enabled = false;

        Vector = Vector2.zero;
    }
}