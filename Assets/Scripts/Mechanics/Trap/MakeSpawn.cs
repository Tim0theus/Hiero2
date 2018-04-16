using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeSpawn : MonoBehaviour {

    public TrapActivated[] traps;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            foreach (TrapActivated t in traps)
            {
                t.spawnPos = this.gameObject.transform;
            }
        }
    }
}
