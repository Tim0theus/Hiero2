using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioActivatable : Activatable {
    private AudioSource _audioSource;

    float initVolume;

    Coroutine fadeCoroutine;

    private void Start() {
        _audioSource = GetComponent<AudioSource>();
        Riddle riddle = GetComponent<Riddle>();

        initVolume = _audioSource.volume;

        if (riddle) {
            riddle.Indicators.Add(this);
        }
    }

    public override void Activate() {
        if (!_audioSource.isPlaying)
        {
            _audioSource.Play();
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);
            _audioSource.volume = initVolume;
        }
    }

    public override void DeActivate() {
        fadeCoroutine = StartCoroutine(FadeOut(_audioSource, 5));
    }

    public static IEnumerator FadeOut (AudioSource audioSource, float fadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeTime;

            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }

}
