using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour {
    public Vector3 begin, end;
    public float t_born, t_lerp = 3f;
	
    void Awake()
    {
        t_born = Time.time;
    }

    public void SetRotation(Vector3 pointTo)
    {
        Vector3 direction = (pointTo - this.transform.position).normalized;
        Quaternion rot = Quaternion.LookRotation(direction);
        this.transform.rotation = rot;
    }

    void FixedUpdate()
    {
        float timeSinceBirth = Time.time - t_born;
        float percentage = timeSinceBirth / t_lerp;
        transform.position = Vector3.Lerp(begin, end, percentage);
        if(percentage >= 1f)
        {
            transform.position = begin;
            t_born = Time.time;
        }
    }

    public static Quaternion GetRotation(Vector3 begin, Vector3 pointTo)
    {
        Vector3 direction = (pointTo - begin).normalized;
        Quaternion rot = Quaternion.LookRotation(direction);
        return rot;
    }
}
