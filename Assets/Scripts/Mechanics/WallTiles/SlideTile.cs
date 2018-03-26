using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlideTile : Tile, IPointerDownHandler, IPointerUpHandler {
    [HideInInspector] public bool InTransition;

    [HideInInspector] public SlideTileMechanic Mechanic;

    private DynamicTwoPositionTransformer _transformer;


    public void OnPointerDown(PointerEventData eventData) { }

    public void OnPointerUp(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Left) {
            Mechanic.UpdateRiddle(this);
        }
    }


    private void Awake() {
        _transformer = DynamicTwoPositionTransformer.Create(transform, 2);
    }

    private void Start() {
        if ((CurrentCorrdinates - OriginalCooridinates).magnitude < 0.1) {
            Solved();
        }
    }

    public void SwapWith(SlideTile tile) {
        Mechanic.Lock();

        _transformer.Set(transform.position, tile.transform.position, TransitionFinished);
        _transformer.Activate();
    }

    private void TransitionFinished(object sender, EventArgs e) {
        Mechanic.UnLock();

        if ((CurrentCorrdinates - OriginalCooridinates).magnitude < 0.1) {
            Solved();
        }
        else if (IsSolved) {
            Failed();
        }
    }
}