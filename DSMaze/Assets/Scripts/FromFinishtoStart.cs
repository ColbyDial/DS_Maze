using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FromFinishtoStart : MonoBehaviour {
    public Vector3 teleportToHere = new Vector3();
	// Use this for initialization
	void Start () {
        teleportToHere = this.transform.position;

        teleportToHere = teleportToHere - new Vector3(0, 25, 0);

    }

    // Update is called once per frame
    void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("The Finish was found!");
        other.transform.position = this.teleportToHere;
    }

    public void changeTeleportLocation(Vector3 newLocation)
    {
        this.teleportToHere = newLocation;
    }
}
