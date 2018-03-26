using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class FillGlyph : Riddle, IPointerDownHandler, IPointerUpHandler {

    public Material target;

    private Material startMat;
    private string _requiredGlyph;
    private Collider _collider;

    private void Awake()
    {
        Renderer renderer = GetComponent<Renderer>();
        startMat = renderer.material;
        _requiredGlyph = target.ExtractGlyphFromMaterial();

        _collider = GetComponent<Collider>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Material[] arr = Resources.LoadAll("Materials/Hieroglyphs/Transparent", typeof(Material)).Cast<Material>().ToArray();
            foreach (Material m in arr)
            {
                if (m.mainTexture.name == LiteralPicker.Current.GlyphCode) GetComponent<Renderer>().material = m;
            }
            _collider.enabled = false;
            if (_requiredGlyph == LiteralPicker.Current.GlyphCode) Solved();
            else
            {
                Failed();
            }

        }
    }

    private new void Solved()
    {
        if (!_solved)
        {
            _solved = true;
            _failed = false;
            Notify(NotificationDelay);
            Indicate(IndicationDelay);
            FillPicker();
        }
    }

    private new void Failed()
    {
        if (!_failed)
        {
            _solved = false;
            _failed = true;
            Notify();
        }
    }


    public override void Reset()
    {
        base.Reset();
        GetComponent<Renderer>().material = startMat;
        _collider.enabled = true;
    }

    public override void Solve()
    {
        base.Solve();
        GetComponent<Renderer>().material = target;
        _collider.enabled = false;
    }

    public override void Disable()
    {
        GetComponent<Renderer>().enabled = false;
        base.Disable();
    }

    public override void Enable()
    {
        GetComponent<Renderer>().enabled = true;
        base.Enable();
    }

}
