using UnityEngine;
using System.Collections;

public class Target : MonoBehaviour {

    controller control;

	// Use this for initialization
	void Start () {
        control = GameObject.Find("FPSController").GetComponent<controller>();
	}

    void OnTriggerEnter (Collider other)
    {
        control.triggerWaypointEntered(gameObject.transform.parent.parent.gameObject);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
