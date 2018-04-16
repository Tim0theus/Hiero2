using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lexicon : MonoBehaviour {

    public GameObject[] entries;

    private List<GameObject> available = new List<GameObject>();
    private int index = 0;

    private bool deactivated;

    public UIControl back;
    public UIControl forward;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (GetComponent<Menu>().open)
        {
            if (deactivated)
                Activate();
        }
        else
        {
            Deactivate();
        }
	}

    // Show all entries for collected hieroglyphs.
    void Activate()
    {
        deactivated = false;

        foreach(GameObject o in entries)
        {
            string n = o.transform.GetChild(0).GetComponent<Image>().sprite.name;
            if (LiteralPicker.PickerGlyphs.ContainsKey(n))
                available.Add(o);
        }

        index = 0;
        for (int i = 0; i < Mathf.Min(6, available.Count); i++)
        {
            available[i].SetActive(true);
        }

        back.DeActivate();
        if (available.Count < 7) forward.DeActivate();

    }

    void Deactivate()
    {
        deactivated = true;

        foreach (GameObject o in entries)
        {
            o.SetActive(false);
        }
        available.Clear();
    }

    // Go forward in lexicon page
    public void forw()
    {
        back.Activate();
        for (int i = index; i < index +6; i++)
        {
            available[i].SetActive(false);
        }

        index += 6;

        for (int i = index; i < Mathf.Min(index + 6, available.Count); i++)
        {
            available[i].SetActive(true);
        }

        if (available.Count < index + 7) forward.DeActivate();
    }


    // go backward in lexicon page
    public void backw()
    {
        forward.Activate();
        for (int i = index; i < Mathf.Min(index + 6, available.Count); i++)
        {
            available[i].SetActive(false);
        }

        index -= 6;

        for (int i = index; i < index + 6; i++)
        {
            available[i].SetActive(true);
        }

        if (index == 0) back.DeActivate();
    }
}
