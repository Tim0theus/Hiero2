using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopParticles : Activatable {

    ParticleSystem _particles;

    private void Awake()
    {
        _particles = GetComponent<ParticleSystem>();
    }

    public override void Activate()
    {
        _particles.Stop();
    }

    public override void DeActivate()
    {
        _particles.Play();
    }

}
