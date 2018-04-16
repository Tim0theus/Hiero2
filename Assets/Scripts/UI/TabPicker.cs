using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabPicker : MonoBehaviour {

    public static TabPicker instance;

    public GameObject prefabGlyph;

    List<GameObject> list = new List<GameObject>();
    PickerGlyph[] arr;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Switch to selected hieroglyph on literal picker
    public void Click(GameObject o)
    {
        int index = 0;
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i].GetComponent<Image>().sprite == o.transform.GetChild(1).GetComponent<Image>().sprite) index = i;
        }
        LiteralPicker.FocusOnGlyph(index);
        Close();
    }

    // Open tabular hieroglyph selection
    public void Open()
    {
        GetComponent<Image>().raycastTarget = true;

        arr = new PickerGlyph[LiteralPicker.PickerGlyphs.Count];

        foreach (var k in LiteralPicker.PickerGlyphs)
        {
            arr[k.Value.Slot] = k.Value;
        }

        for (int i = arr.Length -1; i > -1; i--)
        {
            GameObject temp =  Instantiate(prefabGlyph,this.gameObject.transform);
            temp.SetActive(true);
            temp.transform.GetChild(1).GetComponent<Image>().sprite = arr[i].GetComponent<Image>().sprite;
            temp.transform.GetChild(1).GetComponent<Image>().raycastTarget = true;
            list.Add(temp);

        }
    }

    public void Close()
    {
        GetComponent<Image>().raycastTarget = false;

        foreach (GameObject p in list)
        {
            Destroy(p);
        }
        list.Clear();
    }
}
