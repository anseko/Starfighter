using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CourseView : MonoBehaviour
{
    public GameObject ship;
    Rigidbody shipRB;
    public Vector3 velocity;
    // Start is called before the first frame update
    void Start()
    {
        shipRB = ship.GetComponent<Rigidbody>();   
    }

    // Update is called once per frame
    void Update()
    {
        velocity = (shipRB.velocity+transform.position);
        transform.position = ship.transform.position;
        transform.LookAt(velocity, Vector3.up);
    }
}
