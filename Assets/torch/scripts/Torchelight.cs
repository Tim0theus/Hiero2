using UnityEngine;
using System.Collections;

public class Torchelight : Activatable {
	
	public Light TorchLight;
	public ParticleSystem MainFlame;
	public ParticleSystem BaseFlame;
	public ParticleSystem Etincelles;
	public ParticleSystem Fumee;
	public float MaxLightIntensity;
	public float IntensityLight;
    public bool ActiveAtStart;

    private bool active;

    public override void Activate()
    {
        active = true;
    }

    public override void DeActivate()
    {
        active = false;
        TorchLight.intensity = 0;
        ParticleSystem.EmissionModule temp;
        temp = MainFlame.emission;
        temp.rateOverTime = 0;
        temp = BaseFlame.emission;
        temp.rateOverTime = 0;
        temp = Etincelles.emission;
        temp.rateOverTime = 0;
        temp = Fumee.emission;
        temp.rateOverTime = 0;
    }

    void Start () {
        if (ActiveAtStart)
        {
            Activate();
        }
        else DeActivate();
	}
	

	void Update () {
        if (active)
        {
            if (IntensityLight < 0) IntensityLight = 0;
            if (IntensityLight > MaxLightIntensity) IntensityLight = MaxLightIntensity;

            TorchLight.GetComponent<Light>().intensity = IntensityLight / 2f + Mathf.Lerp(IntensityLight - 0.1f, IntensityLight + 0.1f, Mathf.Cos(Time.time * 30));

            ParticleSystem.EmissionModule temp;
            temp = MainFlame.emission;
            temp.rateOverTime = IntensityLight * 20f;
            temp = BaseFlame.emission;
            temp.rateOverTime = IntensityLight * 15f;
            temp = Etincelles.emission;
            temp.rateOverTime = IntensityLight * 7f;
            temp = Fumee.emission;
            temp.rateOverTime = IntensityLight * 12f;
        }
    }
}
