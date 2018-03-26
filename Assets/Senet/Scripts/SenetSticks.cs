using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SenetSticks : Activatable, IPointerDownHandler, IPointerUpHandler {

    public GameObject[] sticks;

    public AudioSource sticks_floor;

    public Transform target;

    private bool active = false;

    private double throwTimer = 0;
    private double endTimer = 0;
    private bool awaitingResult = false;

    private SenetMain senet;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!active) return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            RaycastResult raycastResult = eventData.pointerCurrentRaycast;

            if (raycastResult.distance < Global.Constants.TouchDistance)
            {
                throwTimer = float.MaxValue;
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!active) return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            RaycastResult raycastResult = eventData.pointerCurrentRaycast;

            if (raycastResult.distance < Global.Constants.TouchDistance)
            {
                ThrowSticks();
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Stick")
        {
            other.gameObject.transform.position = target.position;
        }
    }

    void Awake () {
        senet = GetComponentInParent<SenetMain>();
        foreach (GameObject stick in sticks)
        {
            MaterialFader.Create(stick.GetComponent<Renderer>(), new Color(0.2f, 0.2f, 0.2f), Color.black, true, 1, true);
        }
	}

	void FixedUpdate () {

        if (throwTimer > 0)
        {
            throwTimer -= Time.deltaTime;
            foreach (GameObject stick in sticks)
            {
                Vector3 result = target.position - stick.transform.position;
                float yVec = target.position.y - stick.transform.position.y;
                yVec = yVec > 0 ? yVec : 0;
                stick.GetComponent<Rigidbody>().velocity = result * Time.deltaTime * 100;
                stick.GetComponent<Rigidbody>().rotation = Quaternion.Lerp(stick.GetComponent<Rigidbody>().rotation, UnityEngine.Random.rotation, Time.deltaTime);
            }
        }
        else if (endTimer > 0) {
            endTimer -= Time.deltaTime;
            foreach (GameObject stick in sticks)
            {
                Vector3 result = stick.transform.position - target.position;
                stick.GetComponent<Rigidbody>().AddForce(result.normalized * 100 * Time.deltaTime);
            }
            awaitingResult = true;
            DeActivate();
        }
        else if (awaitingResult)
        {
            awaitingResult = false;
            foreach (GameObject stick in sticks)
            {
                if (!stick.GetComponent<Rigidbody>().IsSleeping()) awaitingResult = true;
            }
            if (!awaitingResult)
            {
                int result;
                if ((result = getNumber()) == -1)
                {
                    ThrowSticks();
                    awaitingResult = true;
                }
                else
                {
                    senet.OnPointsCalculated(result);
                }
            }
        }
    }

    public void ThrowSticks()
    {
        throwTimer = 1;
        endTimer = 0.2f;
        sticks_floor.Play();
    }

    public int getNumber()
    {
        int sum = 0;
        float epsilon = 80f;

        foreach (GameObject stick in sticks)
        {
            Vector3 referenceObjectSpace = stick.transform.TransformDirection(Vector3.left);

            float min = float.MaxValue;
            Vector3 minKey = Vector3.zero;

            float a = Vector3.Angle(referenceObjectSpace, Vector3.up);
            if (a <= epsilon && a < min)
            {
                min = a;
                minKey = Vector3.up;
            }


            a = Vector3.Angle(referenceObjectSpace, Vector3.down);
            if (a <= epsilon && a < min)
            {
                min = a;
                minKey = Vector3.down;
            }

            if (min > epsilon) return -1;
            sum += minKey == Vector3.up ? 1 : 0;
        }

        return sum == 0 ? 5 : sum;
    }

    public override void Activate()
    {
        this.active = true;
        foreach (GameObject stick in sticks)
        {
            stick.GetComponent<Fader>().Activate();
        }
    }

    public override void DeActivate()
    {
        this.active = false;
        foreach (GameObject stick in sticks)
        {
            stick.GetComponent<Fader>().DeActivate();
        }
    }
}
