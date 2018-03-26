using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Message : MonoBehaviour, IPointerUpHandler, IPointerDownHandler {

    public Menu menu;
    public Text text;
    public TextAnchor anchor;

    [TextArea(3,10)]
    public string message;

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                RaycastResult raycastResult = eventData.pointerCurrentRaycast;

                if (raycastResult.distance < Global.Constants.TouchDistance)
                {
                    if (LiteralPicker.Current.GlyphCode == "13079")
                    {
                        text.text = message;
                        text.alignment = anchor;
                        menu.Open();
                    }
                    else
                    {
                        SoundController.instance.Play("error");
                    }
                }
            }
    }
}
