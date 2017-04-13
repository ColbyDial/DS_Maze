using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bumpUp : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        other.transform.position = new Vector3(5, 5, 0);
    }
}
