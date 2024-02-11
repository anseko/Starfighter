using UnityEngine;

public class RotateStarGate : MonoBehaviour
{

    [SerializeField] private float _coefficient;
    
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.left*_coefficient);
    }
}
