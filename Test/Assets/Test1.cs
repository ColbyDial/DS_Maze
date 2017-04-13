using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1 : MonoBehaviour {
    public int timesThrough;
    public bool finished = false;
    // Use this for initialization
    void Start() {
        Debug.Log(timesThrough);
        StartCoroutine(wait_n_Seconds(10));

    }

    // Update is called once per frame
    void Update() {
        int gateKeeper = 0;
        timesThrough++;
        if (finished && gateKeeper ==0 )
        {
            Debug.Log("It has been 10 seconds");
            Debug.Log(timesThrough);
            gateKeeper++;
        }
    }


    public IEnumerator wait_n_Seconds(int n)
    {
        for(int i = 0; i < n; i++)
        {
            yield return new WaitForSeconds(1);
            Debug.Log("Have waited " + i + "seconds");
            if(i == 9)
            {
                Debug.Log("Setting finished to true");
                finished = true;
            }
        }
    }
}
