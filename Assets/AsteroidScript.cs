using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidScript : MonoBehaviour
{
    public float rotationSpeed;
    public int val;

    // Start is called before the first frame update
    void Start()
    {
        //WorldCreateScript val1 = (WorldCreateScript)GameObject.Find("WorldCreate").GetComponent(typeof(WorldCreateScript));
        //val = val1.ID;
        Rigidbody asteroid = GetComponent<Rigidbody>();
        asteroid.angularVelocity = Random.insideUnitSphere * rotationSpeed;
    }

    // срабатывает при начале столкновения, и в переменной other содержится второй объект (с которым столкнулись)
    //private void OnTriggerEnter(Collider other)
    //{
        //Destroy(gameObject); //уничтожаем текущий игровой объект
    //}
}
