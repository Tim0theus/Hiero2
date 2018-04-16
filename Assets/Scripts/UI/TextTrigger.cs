using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TextTrigger : MonoBehaviour, IPointerEnterHandler {

    [TextArea]
    public string text;

    public Text _field;
    public void OnPointerEnter(PointerEventData eventData)
    {
        _field.text = text;
    }
}
