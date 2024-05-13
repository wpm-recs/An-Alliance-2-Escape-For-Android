using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskDudeBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody2D Rigidbody;
    public float Speed = 5.0f;
    public float JumpForce = 6.0f;
    public Vector3 SpawnPoint;
    private const float ShootPlace = 0.3f;
    public int FaceDirection = 1;
    private SpriteRenderer SpriteRenderer;
    private OnGroundDector OngroundDector;
    private const float DeathHight = -40f;
    public ParticleSystem playerPS;
    private AudioSource AudioSource;
    public AudioClip shoot, jump, die;
    public FrogBehavior BoundedDieObject;

    public int maxHealth = 3;
    public HealthBarBehavior healthBar;

    private GameObject[] SpikeList;
    private GameObject[] platforms;
    private GameObject[] DisappearingFloors;
    public float keygap = 0.1f;
    private float gaptimer;


    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        OngroundDector = GetComponent<OnGroundDector>();
        AudioSource = GetComponent<AudioSource>();
        SpikeList = GameObject.FindGameObjectsWithTag("MovingSpikeMuskDude");
        platforms = GameObject.FindGameObjectsWithTag("MovingPlatform");
        DisappearingFloors=GameObject.FindGameObjectsWithTag("DisappearingFloor");
        Debug.Log(DisappearingFloors.Length);
    }

    void Start()
    {
        Born();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("collision happen");
        if (collision.gameObject.CompareTag("Spike")) Die();
        if (collision.gameObject.CompareTag("MovingSpike")) Die();
        if (collision.gameObject.CompareTag("MovingSpikeMuskDude")) Die();
        if (collision.gameObject.CompareTag("MovingSpikeNinjaFrog")) Die();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Particle"))
            if(healthBar.TakeDemageAndDie(1)){
                Die();
            }
    }
    void Update()
    {
        Move();
        JumpDetect();
        if (transform.position.y < DeathHight) Die();

    }
    void Move()
    {
        if (Input.GetKey(KeyCode.D))
        {
            FaceDirection = 1;
            SpriteRenderer.flipX = false;
            Rigidbody.velocity += new Vector2(FaceDirection * Speed - Rigidbody.velocity.x, 0);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            SpriteRenderer.flipX = true;
            FaceDirection = -1;
            Rigidbody.velocity += new Vector2(FaceDirection * Speed - Rigidbody.velocity.x, 0);
        }
        else
        {
            Rigidbody.velocity -= new Vector2(Rigidbody.velocity.x, 0);
        }

        Vector2 p = transform.position;
        ShootArrow();
    }
    void AdditionalJump()
    {
        Rigidbody.AddForce(transform.up * JumpForce, ForceMode2D.Impulse);
    }

    void JumpDetect()
    {
        if (gaptimer > 0) gaptimer -= Time.deltaTime;
        else gaptimer += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.W) && OngroundDector.TestOnGround())
        {
            AudioSource.clip = jump;
            AudioSource.Play();
            Rigidbody.AddForce(transform.up * JumpForce, ForceMode2D.Impulse);
            gaptimer = keygap;
        }
        else if (Input.GetKeyDown(KeyCode.I) && OngroundDector.TestOnGround())
        {
            AudioSource.clip = jump;
            AudioSource.Play();
            Rigidbody.AddForce(transform.up * JumpForce, ForceMode2D.Impulse);
            gaptimer = -keygap;
        }

        ParticleSystem();
    }

    void ShootArrow()
    {
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            GameObject Arrow = Instantiate(Resources.Load("Prefabs/Arrow") as GameObject);
            AudioSource.clip = shoot;
            AudioSource.Play();
            Arrow.transform.localPosition = transform.localPosition + new Vector3(0, ShootPlace, 0);
            ArrowBehavior ArrowBehavior = Arrow.GetComponent<ArrowBehavior>();
            ArrowBehavior.Direction = FaceDirection;
        }
    }
    public void Die()
    {
        AudioSource.clip = die;
        AudioSource.Play();
        transform.position = SpawnPoint;
        BoundedDieObject.transform.position = BoundedDieObject.SpawnPoint;
        BoundedDieObject.ResetSpikes();
        Born();
        ResetSpikes();
        ResetPlatforms();
        ResetFloors();
    }

    void Born()
    {
        healthBar.SetMaxHealth(maxHealth);
    }

    public void ResetSpikes()
    {
        foreach (GameObject obj in SpikeList)
        {
            obj.GetComponent<RightSpikeBehavior>().Reset();
        }
    }
    public void ResetPlatforms()
    {
        foreach (GameObject obj in platforms)
        {
            obj.GetComponent<MovingPlatformBehavior>().Reset();
        }
    }
    public void ResetFloors(){
        foreach (GameObject obj in DisappearingFloors)
        {            
            obj.GetComponent<Disappearedplatform>().Reset();
        }
    }

    void ParticleSystem()
    {
        playerPS.Play();
    }
}
