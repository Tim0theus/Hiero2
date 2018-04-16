using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishAI : MonoBehaviour {

    public GameObject boundary;
    public float speed = 1.0f;

    private Vector3 _target;
    private float _timer;

    // Check if Point is in Boundaries
    bool PointInOABB(Vector3 point, BoxCollider box)
    {
        point = box.transform.InverseTransformPoint(point) - box.center;

        float halfX = (box.size.x * 0.5f);
        float halfY = (box.size.y * 0.5f);
        float halfZ = (box.size.z * 0.5f);
        if (point.x < halfX && point.x > -halfX &&
           point.y < halfY && point.y > -halfY &&
           point.z < halfZ && point.z > -halfZ)
            return true;
        else
            return false;
    }

    // Create new waypoint for fish
    private Vector3 GetNewWaypoint()
    {
        Vector3 point;
        point = new Vector3(Random.Range(boundary.transform.position.x - boundary.GetComponent<BoxCollider>().bounds.extents.x, boundary.transform.position.x + boundary.GetComponent<BoxCollider>().bounds.extents.x), transform.position.y, Random.Range(boundary.transform.position.z - boundary.GetComponent<BoxCollider>().bounds.extents.z, boundary.transform.position.z + boundary.GetComponent<BoxCollider>().bounds.extents.z));

        while (!PointInOABB(point, boundary.GetComponent<BoxCollider>())) 
            point = new Vector3(Random.Range(boundary.transform.position.x - boundary.GetComponent<BoxCollider>().bounds.extents.x, boundary.transform.position.x + boundary.GetComponent<BoxCollider>().bounds.extents.x), transform.position.y, Random.Range(boundary.transform.position.z - boundary.GetComponent<BoxCollider>().bounds.extents.z, boundary.transform.position.z + boundary.GetComponent<BoxCollider>().bounds.extents.z));

        return point;
    }


	// Use this for initialization
	void Start () {
        Random.InitState((int)Time.time);
        _timer = 0;
	}
	
	// Update is called once per frame
	void Update () {
        _timer -= Time.deltaTime;

        if (_timer < 0)
        {
            _timer = Random.Range(1.0f, 5.0f);
            _target = GetNewWaypoint();
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(_target - transform.position), Time.deltaTime);
        transform.position += transform.forward * Time.deltaTime * speed;

		
	}
}
