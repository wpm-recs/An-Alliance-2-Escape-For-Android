using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

public class MaskDudeBehaviorNet : NetworkBehaviour
{
    // Start is called before the first frame update
    private Rigidbody2D Rigidbody;
    private const float Speed=5.0f;
    public  float JumpForce=8.0f;
    public Vector3 SpawnPoint;
    //private const float ShootPlace = 0.3f;
    public NetworkVariable<int> FaceDirection= new NetworkVariable<int>(1,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
    private SpriteRenderer SpriteRenderer;
    private OnGroundDector OngroundDector;
    private const float DeathHight= -40f;
    public ParticleSystem playerPS;
    public AudioSource AudioSource;
    public AudioClip shoot, jump, move, die;
    private int moveFlag = 0;
    // public FrogBehavior BoundedDieObject;
    private GameObject[] SpikeList = null;
    private GameObject[] platforms = null;
    private GameObject[] floors = null;
    public float keygap = 0.1f;
    private float gaptimer;
    private GameObject myFrog = null;
    public float countDownTime = 0.1f;
    public NetworkVariable<float> countDown = new NetworkVariable<float>(0f,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
    //public int maxHealth = 3;
    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        OngroundDector = GetComponent<OnGroundDector>();
        AudioSource = GetComponent<AudioSource>();
    }

    public override void OnNetworkSpawn(){
        if(IsOwner){
            SpikeList = GameObject.FindGameObjectsWithTag("MovingSpikeMuskDude");
            //Debug.Log("SpikeList.Length"+SpikeList.Length.ToString());
            if(SpikeList != null)
            foreach (GameObject obj in SpikeList)
            {
                obj.GetComponent<RightSpikeBehavior>().p1 = transform;
            }
            platforms = GameObject.FindGameObjectsWithTag("MovingPlatform");
            floors = GameObject.FindGameObjectsWithTag("DisappearingFloor");
        }
        else{
            Debug.Log("trying to become dynamic");
            gameObject.GetComponent<Rigidbody2D>().useFullKinematicContacts = true;
            gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            ChangeRigidbodyTypeServerRpc();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    void ChangeRigidbodyTypeServerRpc()
    {
        Debug.Log("ChangeRigidbodyTypeServerRpc called");
        // 在这里改变刚体类型
        gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
    }
    void OnCollisionEnter2D(Collision2D collision){
        if (collision.gameObject.CompareTag("Spike")) BoundedDie();
        if (collision.gameObject.CompareTag("MovingSpike")) BoundedDie();
        if (collision.gameObject.CompareTag("MovingSpikeMuskDude")) BoundedDie();
        if (collision.gameObject.CompareTag("MovingSpikeNinjaFrog")) BoundedDie();
    }
    void Update()
    {
        //Debug.Log(FaceDirection.Value);
        if(IsOwner){
            MoveALittleBit();
            JumpDetect();
            if(countDown.Value > 0){
                countDown.Value -= Time.deltaTime;
            } else{
                countDown.Value = 0f;
            }
            if(transform.position.y < DeathHight) BoundedDie();
        }
        Move();

        if (Input.GetKeyDown(KeyCode.T) && !IsHost)
        {
            Debug.Log("TestServerRpc trigger");
            TestServerRpc();
        }

        //Debug.Log("MuskDude Countdown:"+countDown.Value.ToString());
        
    }
    void Move(){
        if(IsOwner){
            if (Input.GetKey(KeyCode.D))
	        {
	            FaceDirection.Value = 1;
	            //SpriteRenderer.flipX = false;
	            Rigidbody.velocity += new Vector2(FaceDirection.Value * Speed - Rigidbody.velocity.x, 0);
	        }
	        else if (Input.GetKey(KeyCode.A))
	        {
	            //SpriteRenderer.flipX = true;
	            FaceDirection.Value = -1;
	            Rigidbody.velocity += new Vector2(FaceDirection.Value * Speed - Rigidbody.velocity.x, 0);
	        }
	        else
	        {
	            Rigidbody.velocity -= new Vector2(Rigidbody.velocity.x, 0);
	        }
            //ShootArrow();
        }
        //Debug.Log("FaceDirection:"+FaceDirection.Value.ToString());
        if(FaceDirection.Value == 1) SpriteRenderer.flipX=false;
        else SpriteRenderer.flipX=true;
    }
    void JumpDetect(){
        if(IsHost){
            if(Input.GetKeyDown(KeyCode.W)){
                JumpClientRpc();
                //Debug.Log("ForceAdded");
            }
        }
        ParticleSystem();
    }
    // void ShootArrow()
    // {
    //     if (Input.GetKeyDown(KeyCode.LeftShift))
    //     {
    //         GameObject Arrow = Instantiate(Resources.Load("Prefabs/Arrow") as GameObject);
    //         Arrow.transform.localPosition = transform.localPosition+new Vector3(0,ShootPlace,0);
    //         ArrowBehavior ArrowBehavior = Arrow.GetComponent<ArrowBehavior>();
    //         ArrowBehavior.Direction = FaceDirection.Value;
    //         Arrow.GetComponent<NetworkObject>().Spawn();
    //         AudioSource.clip = shoot;
    //         AudioSource.Play();
    //     }
    // }
    public void Die(){
        AudioSource.clip = die;
        AudioSource.Play();
        transform.position=SpawnPoint;
        Rigidbody.velocity=Vector3.zero;
        ResetSpikes();
        ResetPlatforms();
        ResetFloors();
    }

    [ClientRpc]
    private void JumpClientRpc()
    {
        GameObject frog = GameObject.Find("NinjaFrog(Clone)");
        if(IsHost){
            if(OngroundDector.TestOnGround())
            {
            	AudioSource.clip = jump;
            	AudioSource.Play();
                Rigidbody.AddForce(transform.up*JumpForce,ForceMode2D.Impulse);
                StartCountDown();
            }
            else if(frog.GetComponent<FrogBehaviorNet>().countDown.Value > 0f){
                AudioSource.clip = jump;
            	AudioSource.Play();
                Rigidbody.AddForce(transform.up*JumpForce,ForceMode2D.Impulse);
                frog.GetComponent<Rigidbody2D>().AddForce(transform.up*JumpForce,ForceMode2D.Impulse);
                frog.GetComponent<FrogBehaviorNet>().AudioSource.clip = frog.GetComponent<FrogBehaviorNet>().jump;
                frog.GetComponent<FrogBehaviorNet>().AudioSource.Play();
                //frog.GetComponent<FrogBehaviorNet>().countDown.Value = 0f;
            }
        }
        else{
            if(frog.GetComponent<OnGroundDector>().TestOnGround())
            {
                frog.GetComponent<Rigidbody2D>().AddForce(transform.up*JumpForce,ForceMode2D.Impulse);
                frog.GetComponent<FrogBehaviorNet>().AudioSource.clip = frog.GetComponent<FrogBehaviorNet>().jump;
                frog.GetComponent<FrogBehaviorNet>().AudioSource.Play();
            }
        }
    }
    
    private void StartCountDown(){
        countDown.Value = countDownTime;
    }
    
    // void Born()
    // {
    //     healthBar.SetMaxHealth(maxHealth);
    // }

    void ResetSpikes()
    {
        if(SpikeList != null)
            foreach (GameObject obj in SpikeList)
            {
                obj.GetComponent<RightSpikeBehavior>().Reset();
            }
    }
    void ResetPlatforms()
    {
        if(platforms != null)
            foreach (GameObject obj in platforms)
            {
                obj.GetComponent<MovingPlatformBehavior>().Reset();
            }
    }
    void ResetFloors()
    {
        if(floors != null)
            foreach (GameObject obj in floors)
            {
                obj.GetComponent<Disappearedplatform>().Reset();
            }
    }
    private void BoundedDie()
    {
        if(IsOwner)
            BoundedDieClientRpc();
    }

    [ClientRpc]
    private void BoundedDieClientRpc()
    {
        Debug.Log("BoundedDieClientRpc called");
        if(IsHost)
            Die();
        else{
            Debug.Log("Frog die");
            if(myFrog == null) myFrog = GameObject.Find("NinjaFrog(Clone)");
            Debug.Log("myFrog:"+myFrog.name);
            myFrog.GetComponent<FrogBehaviorNet>().Die();
        }
    }

    void ParticleSystem()
    {
        playerPS.Play();
        
    }

    private void MoveALittleBit(){
        if(moveFlag == 0) 
            transform.position += new Vector3(-0.0011f, -0.0011f, 0);
        else
            transform.position += new Vector3(0.0011f, 0.0011f, 0);
        moveFlag = 1 - moveFlag;
    }

    [ServerRpc]
    private void TestServerRpc(){
        Debug.Log("TestServerRpc called");
    }
}
