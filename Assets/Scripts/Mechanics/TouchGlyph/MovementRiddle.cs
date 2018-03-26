using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovementRiddle : Riddle, IActivatable, IPointerDownHandler, IPointerUpHandler
{
    public string _requiredGlyph;
    public Vector3 moveDirection;
    public int iterations;

    private Collider _collider;
    private int it;
    private bool moving = false;

    AudioSource _audio;

    private void Start()
    {
        _audio = GetComponent<AudioSource>();

        _collider = GetComponent<Collider>();

        it = 0;
    }

    public IEnumerator Move(Vector3 moveDirection)
    {
        Vector3 targetPosition = transform.position + moveDirection;
        while ((transform.position - targetPosition).magnitude > 0.005f)
        {
            transform.position += Time.deltaTime * (moveDirection);
            yield return null;
        }
        moving = false;
    }


    public void Activate()
    {
        _collider.enabled = true;
    }

    public void DeActivate()
    {
        _collider.enabled = false;
    }

    public override void Reset()
    {
        base.Reset();
        _collider.enabled = true;
    }

    public override void Solve()
    {
        if (!IsSolved)
        {
            transform.position += iterations * moveDirection;
        }
        base.Solve();
        DeActivate();
    }

    public void OnPointerDown(PointerEventData eventData) { }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (LiteralPicker.Current.GlyphCode != _requiredGlyph)
        {
            SoundController.instance.Play("error");
            GameControl.instance.SubtractPoint(null, null);
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (!moving)
            {
                moving = true;
                if (_audio)
                    _audio.Play();
                it++;
                StartCoroutine(Move(moveDirection));
            }
            if (it == iterations)
            {
                DeActivate();
                Solved();
                _collider.enabled = false;
            }
        }
    }

}
