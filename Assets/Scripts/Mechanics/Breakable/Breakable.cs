using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.EventSystems;


[RequireComponent(typeof(AudioSource))]
public class Breakable : Riddle, IPointerDownHandler, IPointerUpHandler {
    private const float FreezeTimer = 5;

    public GameObject SimplifiedMesh;
    public List<Rigidbody> Fixed;

    [HideInInspector] public bool RemoteBreak;
    public List<AudioClip> BreakSounds;

    protected List<Rigidbody> Rigidbodies;
    protected bool FreezeUp;

    private AudioSource _audioSource;
    private float _timer;

    public void OnPointerDown(PointerEventData eventData) { }

    public virtual void OnPointerUp(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Left) {
            RaycastResult raycastResult = eventData.pointerCurrentRaycast;

            if (raycastResult.distance < Global.Constants.TouchDistance && !FreezeUp && !RemoteBreak) {
                Break();
            }
        }
    }

    protected void Start() {
        Rigidbodies = GetComponentsInChildren<Rigidbody>().ToList();

        foreach (Rigidbody rigidbody in Rigidbodies) {
            rigidbody.gameObject.SetActive(false);
        }

        foreach (Rigidbody rigidbody in Fixed) {
            rigidbody.GetComponent<Collider>().enabled = false;
            rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            Rigidbodies.Remove(rigidbody);
        }

        _audioSource = GetComponent<AudioSource>();
    }


    public virtual void Break() {
        FreezeUp = true;

        foreach (Rigidbody rigidbody in Rigidbodies) {
            rigidbody.gameObject.SetActive(true);
        }

        SimplifiedMesh.SetActive(false);
        GetComponent<Collider>().enabled = false;

        if (BreakSounds.Count > 0) {
            int index = Random.Range(0, BreakSounds.Count);
            _audioSource.PlayOneShot(BreakSounds[index]);
        }

        Solved();
    }

    private void LateUpdate() {
        if (FreezeUp) {
            _timer += Time.deltaTime;

            if (_timer > FreezeTimer) {
                foreach (Rigidbody rigidbody in Rigidbodies) {
                    rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                    rigidbody.GetComponent<Collider>().enabled = false;
                }
                enabled = false;
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Breakable), true)]
    [CanEditMultipleObjects]
    public class BreakableMultiEditor : Editor {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            if (Application.isPlaying) {
                if (GUILayout.Button("Break")) {
                    foreach (Breakable breakable in targets) {
                        breakable.Break();
                    }
                }
            }
        }
    }
#endif

}