using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeSpawn : MonoBehaviour {

    public TrapActivated trap;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            trap.spawnPos = this.gameObject.transform;
        }
    }
}
