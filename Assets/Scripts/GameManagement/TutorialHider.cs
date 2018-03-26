using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialHider : MonoBehaviour {

    public ControlType[] typeToShow;

	// Use this for initialization
	void Start () {
        bool active = false;
		foreach (ControlType t in typeToShow)
        {
            if (PlayerControls.Instance.CurrentType == t) active = true;
        }
        if (!active) gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
