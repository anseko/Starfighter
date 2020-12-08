using UnityEngine;
using Control;
public class playerScript : MonoBehaviour
{  
    GameObject Front, Back, Left, Right, Player;
    Rigidbody ship, engine;
    ParticleSystem TLM, TRM, BLM, BRM, TE;
    ConstantForce ThurstForce;
    public float ManeurSpeed, ThurstSpeed;
    public float shipSpeed, shipRotation;
    private float Time, preRotation, postRotation; 
    public GameObject ThurstsEmition, TopLeftEmition, TopRightEmition, BotLeftEmition, BotRightEmition;
    public float shipAngle;
    public Vector3 ThurstForceVector, ManeurForceVector;
    private MovementAdapter shipsBrain;

    void Start()
    {
        // всякое говно при создании объекта
        Front = GameObject.Find("Front");
        Back  = GameObject.Find("Back");
        Left  = GameObject.Find("Left");
        Right = GameObject.Find("Right");
        Player = GameObject.Find("Player");
        ship = GetComponent<Rigidbody>();
        ThurstForce = GetComponent<ConstantForce>();
        shipSpeed = 0;
        shipRotation = 0;
        preRotation = ship.transform.rotation.y;
        ManeurSpeed = 0f;
        ThurstSpeed = 0f;
        shipAngle = 0f;
        TRM = TopRightEmition.GetComponent<ParticleSystem>();
        TLM = TopLeftEmition.GetComponent<ParticleSystem>();
        BRM = BotRightEmition.GetComponent<ParticleSystem>();
        BLM = BotLeftEmition.GetComponent<ParticleSystem>();
        TE = ThurstsEmition.GetComponent<ParticleSystem>();
        shipsBrain = new PlayerControl();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        shipSpeed = ship.velocity.magnitude;
        shipRotation = ship.angularVelocity.magnitude;
        // всякое управляющее говно
        ThurstSpeed = Input.GetAxis("Jump")*2.5f+Input.GetAxis("Vertical");
        ManeurSpeed = Input.GetAxis("Horizontal");
        shipAngle = Input.GetAxis("Rotation");
        
        // всякая хуйня которая считает тягу
        ThurstForceVector = Front.transform.position-Back.transform.position; //вектор фронтальной тяги
        ManeurForceVector = Right.transform.position-Left.transform.position; //вектор боковой тяги
        ThurstForce.force = ((ThurstForceVector/ThurstForceVector.magnitude)*ThurstSpeed)+((ManeurForceVector/ManeurForceVector.magnitude)*ManeurSpeed);
        ThurstForce.torque = new Vector3(0,shipAngle,0);
        var engines = shipsBrain.getMovement();
        // зажигаем и тушим партиклы движков
        TE.Stop();
        TLM.Stop();
        TRM.Stop();
        BRM.Stop();
        BLM.Stop();
        if(engines.Thrust == true)
        {
            TE.Play(true);
        }
        if(engines.topRight == true)
        {
            TRM.Play(true);
        }
        if(engines.topLeft == true)
        {
            TLM.Play(true);
        }
        if(engines.botLeft == true)
        {
            BLM.Play(true);
        }
        if(engines.botRight == true)
        {
            BRM.Play(true);
        }
    }
}