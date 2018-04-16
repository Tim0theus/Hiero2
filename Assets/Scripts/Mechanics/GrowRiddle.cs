using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowRiddle : Activatable {

    private bool growing;

	// Use this for initialization
	void Start () {
        growing = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public IEnumerator Grow()
    {
        while(growing)
        {
            yield return null;
        }
        growing = true;
        Vector3 targetScale = 2*transform.localScale;
        while ((transform.localScale - targetScale).magnitude > 0.005f)
        {
            transform.localScale += Time.deltaTime * targetScale / 10.0f;
            yield return null;
        }
        growing = false;
    }

    public override void Activate()
    {
        StartCoroutine(Grow());
    }

    public override void DeActivate()
    {

    }
}
