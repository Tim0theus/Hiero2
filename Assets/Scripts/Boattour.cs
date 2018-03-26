﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boattour : Activatable {

    public GameObject target;
    public GameObject player;

    public Transform[] tour;
    private int current;

    public bool finishTour;

    public Collider wall;

    private NavMeshAgent agent;

    private bool once = true;

    private void OnTriggerEnter(Collider other)
    {
        if (once)
        {
            once = false;

            StartTour();

        }

    }

    // Use this for initialization
    void Awake () {
        agent = GetComponent<NavMeshAgent>();
        current = 0;
        finishTour = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (player.transform.parent == gameObject.transform)
            if (!agent.pathPending)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (current == 0 && finishTour)
                    {
                        current = -1;
                        agent.SetDestination(target.transform.position);
                    }
                    else if (current > -1)
                    {
                        current = ++current % tour.Length;
                        agent.SetDestination(tour[current % tour.Length].position);
                    }

                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
 
                        if (current == -1)
                        {
                            player.transform.parent = null;
                            wall.enabled = false;
                        }
                    }
                }
            }
    }

    public void StartTour()
    {
        agent.enabled = true;
        player.transform.parent = gameObject.transform;
        agent.SetDestination(tour[0].position);
        GetComponent<Collider>().enabled = false;

    }

    public override void Activate()
    {
        finishTour = true;
    }

    public override void DeActivate()
    {
        finishTour = false;
    }
}