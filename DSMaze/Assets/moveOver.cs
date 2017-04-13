using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveOver : MonoBehaviour {

	// Use this for initialization
	void Start () {
        this.transform.position = this.transform.position + new Vector3(2.5f, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {
        // this.transform.Translate(Vector3.right * Time.deltaTime);
    }
}
