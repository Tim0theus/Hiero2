using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

[RequireComponent(typeof(Renderer))]
public class SenetField : Riddle, IPointerDownHandler, IPointerUpHandler {

    public int id = -1;
    public SenetFigure figure = null;
    public SenetField before = null;
    public SenetField after = null;

    public string RiddleCode;
    private bool active = false;

    private AudioSource _audioSource;
    private Transform _target;
    private SenetMain senet;

    public bool outField = false;

    private Collider _collider;

    public void OnPointerDown(PointerEventData eventData) { }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!active) return;
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            RaycastResult raycastResult = eventData.pointerCurrentRaycast;

            if (raycastResult.distance < Global.Constants.TouchDistance)
            {
                if (Inventory.Contains(RiddleCode))
                {
                    Item item = Inventory.Item;
                    item.Putdown(_target, FigurePlaced);
                    Inventory.Clear();
                    Deactivate();

                    if (_audioSource)
                    {
                        _audioSource.Play();
                    }

                }
            }
        }
    }

    private void Awake()
    {
        GetComponent<MeshRenderer>().enabled = false;
        _audioSource = GetComponent<AudioSource>();

        _target = transform.Find("Target");
        senet = GetComponentInParent<SenetMain>();
        _collider = GetComponent<Collider>();
    }

    private void Start()
    {

    }

    // Check if Figure on Field is opponent
    public bool isEnemy(bool figureTypeToMove)
    {
        if (this.figure && this.figure.flat != figureTypeToMove) return true;
        return false;
    }

    // Check if field can be moved to or not, due to a blockade of 3 enemy figures
    bool isBlockaded(bool figureTypeToMove)
    {
        if (id < 4 || id == 10 || id == 11 || id == 12 || id == 20 || id == 21 || id == 22 || id > 25) return false;

        if (before.isEnemy(figureTypeToMove) && before.before.isEnemy(figureTypeToMove) && before.before.before.isEnemy(figureTypeToMove)) return true;
        if (before.before.isEnemy(figureTypeToMove) && before.before.before.isEnemy(figureTypeToMove) && before.before.before.before.isEnemy(figureTypeToMove)) return true;

        return false;
    }

    // Check if field is blocked, because of 2 enemy figures or special field
    bool isBlocked(bool figureTypeToMove)
    {
        if (this.figure == null) return false;
        if (this.figure.flat == figureTypeToMove) return true;
        if (id < 1) return false;
        if (id > 25 || id == 14) return true;
        if (before.isEnemy(figureTypeToMove)) return true;
        if (after.isEnemy(figureTypeToMove)) return true;
        return false;
    }

    // Check if the field can be moved to, from the currentID
    bool canMoveHere(bool figureTypeToMove, int currentId)
    {
        if (currentId > id) return false;

        if (currentId < 25 && id > 25) return false;

        if (isBlockaded(figureTypeToMove)) return false;

        if (isBlocked(figureTypeToMove)) return false;

        return true;
    }

    // Check if the figure on the field can be moved by points
    public bool canMove(int points)
    {
        if (points < 1) return false;
        if (outField) return false;

        SenetField tmp = this;

        if (this.id > 26)
        {
            if (30 - this.id == points) return true;
        }

        for (int i = 0; i < points; i++)
        {
            if (tmp.after) tmp = tmp.after;
            else return false;
        }

        return tmp.canMoveHere(this.figure.flat, this.id);
    }

    // Returns target field when moving points. Null if out of field.
    public SenetField GetTarget(int points)
    {
        SenetField tmp = this;

        if (this.id + points > 29)
        {
            return null;
        }

        for (int i = 0; i < points; i++)
        {
            if (tmp.after) tmp = tmp.after;
            else return null;
        }

        return tmp;
    }

    public void FigurePlaced(object sender, EventArgs e)
    {
        senet.OnPutDown();
    }


    public void Activate()
    {
        active = true;
        _collider.enabled = true;
    }

    public void Deactivate()
    {
        active = false;
        _collider.enabled = false;
    }
}
