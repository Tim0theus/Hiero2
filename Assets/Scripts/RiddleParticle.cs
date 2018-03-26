using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiddleParticle : MonoBehaviour {

    public ParticleSystem particle;
    public Riddle riddle;
    public Transform target;


    private void Awake()
    {
        riddle.onSolved += OnSolved;
    }

    public void OnSolved(object o, EventArgs e)
    {
        StartParticles();
    }

    void StartParticles()
    {
        var main = particle.main;
        main.startLifetime = Vector3.Distance(target.position, transform.position)*2 / particle.main.startSpeed.constant;
        particle.Play();
    }

    // Update is called once per frame
    void Update () {
        transform.LookAt(target);

    }
}
