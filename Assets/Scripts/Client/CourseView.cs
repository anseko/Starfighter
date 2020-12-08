using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CourseView : MonoBehaviour
{
    GameObject arrow, ship;
    Rigidbody shipRB;
    public Vector3 velocity;
    // Start is called before the first frame update
    void Start()
    {
        ship = GameObject.Find("Player");
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
