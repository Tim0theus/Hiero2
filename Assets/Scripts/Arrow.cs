using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour {

    public float speed = 10;
    public Transform spawnPos;
    public FullscreenOverlay yellowscreen;

    private bool _stop = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            SoundController.instance.Play("die");
            PlayerMechanics.Instance.SetControlMode(2);
            yellowscreen.ActiveImmediately();
            StartCoroutine(Die());
        }
        else if (collision.collider.gameObject.layer != 8)
        {
            _stop = true;
            StartCoroutine(Despawn());
        }
    }

    public IEnumerator Despawn()
    {
        yield return new WaitForSeconds(2);
        Destroy(this.gameObject);
    }

    public IEnumerator Die()
    {
        PlayerMechanics.Instance.transform.position = spawnPos.position;
        yield return new WaitForSeconds(2);
        yellowscreen.DeActivate();
        PlayerMechanics.Instance.UnSetControlMode(2);
    }

    void Update () {
        if (!_stop)
            GetComponent<Rigidbody>().MovePosition(transform.position + transform.forward * Time.deltaTime * speed);
		
	}
}
