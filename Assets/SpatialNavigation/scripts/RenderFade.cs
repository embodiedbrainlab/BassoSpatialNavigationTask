using UnityEngine;
using System.Collections;

public class RenderFade : MonoBehaviour {
    public float minViewDistance = 30.0f;
    public float maxViewDistance = 35.0f;
    public GameObject player;

    float dist;
    float r, g, b, a;
    MeshRenderer rend;
    Color col;

	// Use this for initialization
	void Start () {
        rend = GetComponent<MeshRenderer>();
        if (rend == null)
        {
            Debug.Log("Could not get MeshRenderer!");
        } else
        {
            col = rend.material.color;
            r = col.r;
            g = col.g;
            b = col.b;
        }
	}
	
	
	void FixedUpdate () {
        dist = Vector3.Distance(transform.position, player.transform.position);
        if (dist < minViewDistance)
        {
            a = 1.0f;
            rend.material.color = new Color(r, g, b, a);
            //continue;
        } else if (dist > maxViewDistance)
        {
            a = 0.0f;
            rend.material.color = new Color(r, g, b, a);
            //continue;
        } else
        {
            a = 1 - (((dist - minViewDistance) / (maxViewDistance - minViewDistance)));
            rend.material.color = new Color(r, g, b, a);
        }

	}
}
