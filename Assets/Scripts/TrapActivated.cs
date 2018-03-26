using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapActivated : Activatable
{
    public GameObject arrow;
    public Transform[] trapwalls;

    public Transform spawnPos;
    public FullscreenOverlay yellowscreen;

    public AudioSource[] _audio;

    public override void Activate()
    {
        for (int j = 0; j < trapwalls.Length; j++)
            for (int i = 0; i < 10; i++)
            {
                _audio[j].Play();
                Arrow temp = Instantiate(arrow, trapwalls[j].position + trapwalls[j].transform.forward + new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f),0), trapwalls[j].rotation).GetComponent<Arrow>();
                temp.spawnPos = spawnPos;
                temp.yellowscreen = yellowscreen;
            }

    }

    public override void DeActivate()
    {
    }

}
