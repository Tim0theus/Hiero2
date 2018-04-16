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

    private float _timer;

    private void Awake()
    {
        _timer = 0;
    }

    public override void Activate()
    {
        _timer -= Time.deltaTime;

        if (_timer < 0)
        {
            for (int j = 0; j < trapwalls.Length; j++)
                for (int i = 0; i < 10; i++)
                {
                    _audio[j].Play();
                    Arrow temp = Instantiate(arrow, trapwalls[j].position + trapwalls[j].transform.forward + new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0), trapwalls[j].rotation).GetComponent<Arrow>();
                    temp.spawnPos = spawnPos;
                    temp.yellowscreen = yellowscreen;
                }
            _timer = 0.1f;
        }

    }

    public override void DeActivate()
    {
    }

}
