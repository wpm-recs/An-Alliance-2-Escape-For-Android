using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Networking;
using Unity.Services.Authentication;

public class FrogBehaviorNet : NetworkBehaviour
{
    // Start is called before the first frame update
    private Rigidbody2D Rigidbody;
    private const float Speed=5.0f;
    public  float JumpForce=8.0f;
    public Vector3 SpawnPoint;
    private const float ShootPlace = 0.3f;
    private NetworkVariable<int> FaceDirection= new NetworkVariable<int>(1,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
    private NetworkVariable<bool> teleportFlag= new NetworkVariable<bool>(false,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
    private SpriteRenderer SpriteRenderer;
    private OnGroundDector OngroundDector;
    private GameObject playerPrefab;
    // private int respawnTimes = 5;
    // private int respawnCount = 0;
    private const float DeathHight= -40f;

    public ParticleSystem playerPS;
    public AudioSource AudioSource;
    public AudioClip jump, die;
    //public MaskDudeBehavior BoundedDieObject;

    //public int maxHealth = 3;
    private GameObject[] SpikeList = null;
    private GameObject[] platforms = null;
    private GameObject[] floors = null;
    private int moveFlag = 0;
    public float keygap = 0.1f;
    private float gaptimer;
    public float countDownTime = 0.1f;
    public NetworkVariable<float> countDown = new NetworkVariable<float>(0f,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);

    void Awake()
    {
        Rigidbody=GetComponent<Rigidbody2D>();
        SpriteRenderer=GetComponent<SpriteRenderer>();
        OngroundDector=GetComponent<OnGroundDector>();
        AudioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Spike")) BoundedDie();
        if (collision.gameObject.CompareTag("MovingSpike")) BoundedDie();
        if (collision.gameObject.CompareTag("MovingSpikeMuskDude")) BoundedDie();
        if (collision.gameObject.CompareTag("MovingSpikeNinjaFrog")) BoundedDie();
    }
    void Update()
    {
        if(IsOwner){
            MoveALittleBit();
            JumpDetect();
            if(countDown.Value>0){
                countDown.Value-=Time.deltaTime;
            }
            else{
                countDown.Value=0;
            }
            if(transform.position.y<DeathHight) BoundedDie();
            //Debug.Log("Frog Countdown:"+countDown.Value.ToString());
        }
        Move();
    }
    void Move(){
        if(IsOwner){
            if (Input.GetKey(KeyCode.L))
	        {
	            FaceDirection.Value = 1;
	            //SpriteRenderer.flipX = false;
	            Rigidbody.velocity += new Vector2(FaceDirection.Value * Speed - Rigidbody.velocity.x, 0);
	        }
	        else if (Input.GetKey(KeyCode.J))
	        {
	            //SpriteRenderer.flipX = true;
	            FaceDirection.Value = -1;
	            Rigidbody.velocity += new Vector2(FaceDirection.Value * Speed - Rigidbody.velocity.x, 0);
	        }
	        else
	        {
	            Rigidbody.velocity -= new Vector2(Rigidbody.velocity.x, 0);
	        }
        }
        if(FaceDirection.Value == 1) SpriteRenderer.flipX=false;
        else SpriteRenderer.flipX=true;
    }
    void JumpDetect(){
        GameObject guy = GameObject.Find("MaskDude(Clone)");
        if(!IsHost){
            if((Input.GetKeyDown(KeyCode.I))){
                if(OngroundDector.TestOnGround()){
                    JumpServerRpc();
                    Rigidbody.AddForce(transform.up*JumpForce,ForceMode2D.Impulse);
                    AudioSource.clip = jump;
                    AudioSource.Play();
                    StartCountDown();
                }
                else{
                    if(guy.GetComponent<MaskDudeBehaviorNet>().countDown.Value>0){
                        JumpServerRpc();
                        Rigidbody.AddForce(transform.up*JumpForce,ForceMode2D.Impulse);
                        AudioSource.clip = jump;
                        AudioSource.Play();
                        guy.GetComponent<MaskDudeBehaviorNet>().countDown.Value=0;
                    }
                }
                //Debug.Log("ForceAdded");
            }
        }
        ParticleSystem();
    }
    private void StartCountDown(){
        countDown.Value = countDownTime;
    }
    public void Die(){
        Debug.Log("Frog really die");
        AudioSource.clip = die;
        AudioSource.Play();
        transform.position=SpawnPoint;
        Rigidbody.velocity=Vector3.zero;
        ResetSpikes();
        ResetPlatforms();
        ResetFloors();
    }
    private void MoveALittleBit(){
        if(moveFlag == 0)
            transform.position += new Vector3(0.0011f, 0.0011f, 0);
        else
            transform.position -= new Vector3(0.0011f, 0.0011f, 0);
        moveFlag = 1 - moveFlag;
    }
    
    [ServerRpc]
    private void JumpServerRpc(){
        GameObject Guy = GameObject.Find("MaskDude(Clone)");
        if(Guy.GetComponent<OnGroundDector>().TestOnGround() || Guy.GetComponent<MaskDudeBehaviorNet>().countDown.Value>0)
        {
            Guy.GetComponent<Rigidbody2D>().AddForce(transform.up*JumpForce,ForceMode2D.Impulse);
            Guy.GetComponent<MaskDudeBehaviorNet>().AudioSource.clip = Guy.GetComponent<MaskDudeBehaviorNet>().jump;
            Guy.GetComponent<MaskDudeBehaviorNet>().AudioSource.Play();
        }
    }

    private void BoundedDie(){
        Debug.Log("BoundedDie called " + "IsOwner: "+IsOwner.ToString());
        if(IsOwner){
            Die();
            BoundedDieServerRpc();
        }
    }

    [ServerRpc]
    private void BoundedDieServerRpc(){
        Debug.Log("BoundedDieServerRpc called");
        GameObject Guy = GameObject.Find("MaskDude(Clone)");
        Guy.GetComponent<MaskDudeBehaviorNet>().Die();
    }

    private void SceneManager_OnSceneEvent(SceneEvent sceneEvent){
        Debug.Log($"Scene event {sceneEvent.SceneEventType} " +
                $"for scene {sceneEvent.SceneName} ");

        switch (sceneEvent.SceneEventType)
        {
        case SceneEventType.LoadComplete:
            {
                switch(sceneEvent.SceneName){
                    case("Level_1_Net"):{
                        Debug.Log("Level 1 loaded " + "sceneEventClientID: "+sceneEvent.ClientId.ToString()+"IsHost: "+IsHost.ToString()+"OwnerClientId: "+OwnerClientId.ToString());
                        if(OwnerClientId != 0){
                            if(sceneEvent.ClientId == 0 && IsHost){
                                gameObject.GetComponent<SpriteRenderer>().enabled = true;
                                gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                            }
                            if(sceneEvent.ClientId != 0 && !IsHost) {
                                Debug.Log("initializing level 1");
                                gameObject.GetComponent<SpriteRenderer>().enabled = true;
                                //获取场景中名为camera1的对象
                                GameObject camera = GameObject.Find("Main Camera");
                                camera.GetComponent<Camera>().enabled = false;
                                camera = GameObject.Find("Camera1");
                                camera.GetComponent<CameraFollow>().target = gameObject.transform;
                                GameObject vcam = GameObject.Find("vcam2");
                                vcam.GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = gameObject.transform;
                                SpikeList = GameObject.FindGameObjectsWithTag("MovingSpikeNinjaFrog");
                                platforms = GameObject.FindGameObjectsWithTag("MovingPlatform");
                                foreach (GameObject gmobj in SpikeList)
                                {
                                    gmobj.GetComponent<RightSpikeBehavior>().p1 = transform;
                                }
                                GameObject obj = GameObject.Find("MaskDude");
                                obj.SetActive(false);
                                transform.position = new Vector3(-12.74f,-3.69f,0);
                            }
                        }
                        if(IsHost){
                            if(sceneEvent.ClientId == 0 && IsOwner){
                                GameObject hero = Instantiate(Resources.Load("Prefabs/MaskDude") as GameObject);
                                hero.GetComponent<ShootArrowNet>().enabled = false;
                                hero.GetComponent<NetworkObject>().Spawn(true);
                                hero.transform.position = new Vector3(-37f,-3.5f,0);

                                //hero.transform.position = new Vector3(-37f,12f,0);

                                GameObject endpoint = Instantiate(Resources.Load("Prefabs/EndPointNet") as GameObject);
                                endpoint.GetComponent<NetworkObject>().Spawn(true);
                                endpoint.transform.position = new Vector3(-24.5f,8.03f,0);

                                //endpoint.transform.position = new Vector3(-24.5f,12f,0);

                                GameObject camera = GameObject.Find("Camera1");
                                camera.GetComponent<Camera>().enabled = false;
                                camera = GameObject.Find("Main Camera");
                                camera.GetComponent<CameraFollow>().target = hero.transform;
                                GameObject vcam = GameObject.Find("vcam1");
                                vcam.GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = hero.transform;
                                GameObject obj = GameObject.Find("NinjaFrog");
                                obj.SetActive(false);
                                if(!IsOwner){
                                    Debug.Log("owner: "+OwnerClientId.ToString()+"ishost: "+IsHost.ToString());
                                    //gameObject.GetComponent<SpriteRenderer>().enabled = true;
                                }
                            }
                            else{
                            }
                        }
                        else{
                            if(sceneEvent.ClientId != 0){
                                if(IsOwner){
                                    gameObject.GetComponent<DoubleJump>().enabled = false;
                                    transform.position = new Vector3(-12.74f,-3.69f,0);

                                    //transform.position = new Vector3(-12.74f,12f,0);

                                    //SpawnPoint = new Vector3(-12.74f,12f,0);

                                    Debug.Log("owner: "+OwnerClientId.ToString()+"ishost: "+IsHost.ToString());
                                    //gameObject.GetComponent<SpriteRenderer>().enabled = true;
                                }
                            }
                        }
                        break;
                    }
                    case("Level_2_Net"):{
                        Debug.Log("Level 2 loaded " + "sceneEventClientID: "+sceneEvent.ClientId.ToString()+"IsHost: "+IsHost.ToString()+"OwnerClientId: "+OwnerClientId.ToString());
                        if(OwnerClientId != 0){
                            if(sceneEvent.ClientId == 0 && IsHost){
                                gameObject.GetComponent<SpriteRenderer>().enabled = true;
                                gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                            }
                            if(sceneEvent.ClientId != 0 && !IsHost) {
                                Debug.Log("initializing level 1");
                                gameObject.GetComponent<SpriteRenderer>().enabled = true;
                                //获取场景中名为camera1的对象
                                GameObject camera = GameObject.Find("Main Camera");
                                camera.GetComponent<Camera>().enabled = false;
                                camera = GameObject.Find("Camera1");
                                camera.GetComponent<CameraFollow>().target = gameObject.transform;
                                GameObject vcam = GameObject.Find("vcam2");
                                vcam.GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = gameObject.transform;
                                //SpikeList = GameObject.FindGameObjectsWithTag("MovingSpikeNinjaFrog");
                                platforms = GameObject.FindGameObjectsWithTag("MovingPlatform");
                                floors = GameObject.FindGameObjectsWithTag("DisappearingFloor");
                                GameObject obj = GameObject.Find("MaskDude");
                                obj.SetActive(false);
                                transform.position = new Vector3(2.9f,34f,0);
                            }
                        }
                        if(IsHost){
                            if(sceneEvent.ClientId == 0 && IsOwner){
                                GameObject hero = Instantiate(Resources.Load("Prefabs/MaskDude") as GameObject);
                                hero.GetComponent<NetworkObject>().Spawn(true);
                                hero.transform.position = new Vector3(-54.7f,29.3f,0);

                                GameObject endpoint = Instantiate(Resources.Load("Prefabs/EndPointNet") as GameObject);
                                endpoint.GetComponent<NetworkObject>().Spawn(true);
                                endpoint.transform.position = new Vector3(2.9f,-4.1f,0);

                                generateWalls(new Vector3(-42.6f,-2.9f,0), new Vector3(1f,1f,1f), new Vector3(0.9274721f,-1.189456f,0f));
                                generateWalls(new Vector3(-46.22f,14.48f,0), new Vector3(1.81962f,1f,1f), new Vector3(0.9274721f,-1.189456f,0f));
                                generateWalls(new Vector3(-1f,-24.8f,0), new Vector3(0.7f,1f,1f), new Vector3(0.9274721f,-1.189456f,0f));
                                generateWalls(new Vector3(-22.2f,-10f,0), new Vector3(1.81962f,1f,1f), new Vector3(0.9274721f,-1.189456f,0f));
                                generateWalls(new Vector3(-50.8f,-22.3f,0), new Vector3(1.5f,1f,1f), new Vector3(0.9274721f,-1.189456f,0f));
                                generateWalls(new Vector3(31.4f,5.8f,0), new Vector3(1.2f,1f,1f), new Vector3(0.9274721f,-1.189456f,0f));
                                generateWalls(new Vector3(-22.2f,-10f,0), new Vector3(1.81962f,1f,1f), new Vector3(0.9274721f,-1.189456f,0f));

                                GameObject camera = GameObject.Find("Camera1");
                                camera.GetComponent<Camera>().enabled = false;
                                camera = GameObject.Find("Main Camera");
                                camera.GetComponent<CameraFollow>().target = hero.transform;
                                GameObject vcam = GameObject.Find("vcam1");
                                vcam.GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = hero.transform;
                                GameObject obj = GameObject.Find("NinjaFrog");
                                obj.SetActive(false);
                                if(!IsOwner){
                                    Debug.Log("owner: "+OwnerClientId.ToString()+"ishost: "+IsHost.ToString());
                                    //gameObject.GetComponent<SpriteRenderer>().enabled = true;
                                }
                            }
                            else{
                            }
                        }
                        else{
                            if(sceneEvent.ClientId != 0){
                                if(IsOwner){
                                    transform.position = new Vector3(2.9f,34f,0);

                                    Debug.Log("owner: "+OwnerClientId.ToString()+"ishost: "+IsHost.ToString());
                                    //gameObject.GetComponent<SpriteRenderer>().enabled = true;
                                }
                            }
                        }
                        break;
                    }
                    case("Level_3_Net"):{
                        Debug.Log("Level 3 loaded " + "sceneEventClientID: "+sceneEvent.ClientId.ToString()+"IsHost: "+IsHost.ToString()+"OwnerClientId: "+OwnerClientId.ToString());
                        if(OwnerClientId != 0){
                            if(sceneEvent.ClientId == 0 && IsHost){
                                gameObject.GetComponent<SpriteRenderer>().enabled = true;
                                gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                            }
                            if(sceneEvent.ClientId != 0 && !IsHost) {
                                Debug.Log("initializing level 3");
                                gameObject.GetComponent<SpriteRenderer>().enabled = true;
                                //获取场景中名为camera1的对象
                                GameObject camera = GameObject.Find("Main Camera");
                                camera.GetComponent<Camera>().enabled = false;
                                camera = GameObject.Find("Camera1");
                                camera.GetComponent<CameraFollow>().target = gameObject.transform;
                                GameObject vcam = GameObject.Find("vcam2");
                                vcam.GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = gameObject.transform;
                                //SpikeList = GameObject.FindGameObjectsWithTag("MovingSpikeNinjaFrog");
                                platforms = GameObject.FindGameObjectsWithTag("MovingPlatform");
                                floors = GameObject.FindGameObjectsWithTag("DisappearingFloor");
                                GameObject obj = GameObject.Find("MaskDude");
                                obj.SetActive(false);
                                transform.position = new Vector3(-43.3f,57.7f,0);
                            }
                        }
                        if(IsHost){
                            if(sceneEvent.ClientId == 0 && IsOwner){
                                GameObject hero = Instantiate(Resources.Load("Prefabs/MaskDude") as GameObject);
                                hero.GetComponent<NetworkObject>().Spawn(true);
                                hero.transform.position = new Vector3(-43.3f,57.7f,0);

                                GameObject endpoint = Instantiate(Resources.Load("Prefabs/EndPointNet") as GameObject);
                                endpoint.GetComponent<NetworkObject>().Spawn(true);
                                endpoint.transform.position = new Vector3(23.6f,47.73f,0);

                                generateWalls(new Vector3(-10.47f,2.56f,0), new Vector3(1.2f,1f,1f), new Vector3(-24.10058f,45.79883f,0f));
                                generateWalls(new Vector3(3.5f,-0.01f,0), new Vector3(0.4f,1f,1f), new Vector3(-24.10058f,45.79883f,0f));
                                generateWalls(new Vector3(13.51f,-2.19f,0), new Vector3(1f,1f,1f), new Vector3(-24.10058f,45.79883f,0f));
                                generateWalls(new Vector3(23.44f,1.91f,0), new Vector3(0.7f,1f,1f), new Vector3(-24.10058f,45.79883f,0f));
                                generateWalls(new Vector3(-2.81f,-14.53f,0), new Vector3(1.2f,1f,1f), new Vector3(-24.10058f,45.79883f,0f));
                                generateWalls(new Vector3(13.23f,-14.53f,0), new Vector3(1.2f,1f,1f), new Vector3(-24.10058f,45.79883f,0f));
                                generateWalls(new Vector3(25.72f,-3.93f,0), new Vector3(0.4f,1f,1f), new Vector3(-24.10058f,45.79883f,0f));
                                generateWalls(new Vector3(44.86f,-14.72f,0), new Vector3(1.4f,1f,1f), new Vector3(-24.10058f,45.79883f,0f));
                                generateWalls(new Vector3(62.27f,-13.71f,0), new Vector3(1.1f,1f,1f), new Vector3(-24.10058f,45.79883f,0f));

                                GameObject camera = GameObject.Find("Camera1");
                                camera.GetComponent<Camera>().enabled = false;
                                camera = GameObject.Find("Main Camera");
                                camera.GetComponent<CameraFollow>().target = hero.transform;
                                GameObject vcam = GameObject.Find("vcam1");
                                vcam.GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = hero.transform;
                                GameObject obj = GameObject.Find("NinjaFrog");
                                obj.SetActive(false);
                                if(!IsOwner){
                                    Debug.Log("owner: "+OwnerClientId.ToString()+"ishost: "+IsHost.ToString());
                                    //gameObject.GetComponent<SpriteRenderer>().enabled = true;
                                }
                            }
                            else{
                            }
                        }
                        else{
                            if(sceneEvent.ClientId != 0){
                                if(IsOwner){
                                    transform.position = new Vector3(-43.3f,57.7f,0);

                                    Debug.Log("owner: "+OwnerClientId.ToString()+"ishost: "+IsHost.ToString());
                                    //gameObject.GetComponent<SpriteRenderer>().enabled = true;
                                }
                            }
                        }
                        break;
                    }
                    case("whatever"):{
                        Debug.Log("Level 3 loaded " + "sceneEventClientID: "+sceneEvent.ClientId.ToString()+"IsHost: "+IsHost.ToString()+"OwnerClientId: "+OwnerClientId.ToString());
                        if(OwnerClientId != 0){
                            if(sceneEvent.ClientId == 0 && IsHost) gameObject.GetComponent<SpriteRenderer>().enabled = true;
                            if(sceneEvent.ClientId != 0 && !IsHost) {
                                Debug.Log("initializing level 3");
                                gameObject.GetComponent<SpriteRenderer>().enabled = true;
                                //获取场景中名为camera1的对象
                                GameObject camera = GameObject.Find("Main Camera");
                                camera.GetComponent<Camera>().enabled = false;
                                camera = GameObject.Find("Camera1");
                                camera.GetComponent<CameraFollow>().target = gameObject.transform;
                                GameObject vcam = GameObject.Find("vcam2");
                                vcam.GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = gameObject.transform;
                                floors = GameObject.FindGameObjectsWithTag("DisapperingFloor");
                                GameObject obj = GameObject.Find("MaskDude");
                                obj.SetActive(false);
                            }
                        }
                        if(IsHost){
                            if(sceneEvent.ClientId == 0 && IsOwner){
                                GameObject hero = Instantiate(Resources.Load("Prefabs/MaskDude") as GameObject);
                                hero.GetComponent<NetworkObject>().Spawn(true);
                                hero.transform.position = new Vector3(-80.632f+37.332f,43.5f+14.22f,0);

                                //hero.transform.position = new Vector3(-37f,12f,0);

                                GameObject endpoint = Instantiate(Resources.Load("Prefabs/EndPointNet") as GameObject);
                                endpoint.GetComponent<NetworkObject>().Spawn(true);
                                endpoint.transform.position = new Vector3(-43.3f,57.7f,0);

                                generateWalls(new Vector3(-10.47f,2.56f,0), new Vector3(1.2f,1f,1f), new Vector3(-24.10058f,45.79883f,0f));
                                generateWalls(new Vector3(3.5f,-0.01f,0), new Vector3(0.4f,1f,1f), new Vector3(-24.10058f,45.79883f,0f));
                                generateWalls(new Vector3(13.51f,-2.19f,0), new Vector3(1f,1f,1f), new Vector3(-24.10058f,45.79883f,0f));
                                generateWalls(new Vector3(23.44f,1.91f,0), new Vector3(0.7f,1f,1f), new Vector3(-24.10058f,45.79883f,0f));
                                generateWalls(new Vector3(-2.81f,-14.53f,0), new Vector3(1.2f,1f,1f), new Vector3(-24.10058f,45.79883f,0f));
                                generateWalls(new Vector3(13.23f,-14.53f,0), new Vector3(1.2f,1f,1f), new Vector3(-24.10058f,45.79883f,0f));
                                generateWalls(new Vector3(25.72f,-3.93f,0), new Vector3(0.4f,1f,1f), new Vector3(-24.10058f,45.79883f,0f));
                                generateWalls(new Vector3(44.86f,-14.72f,0), new Vector3(1.4f,1f,1f), new Vector3(-24.10058f,45.79883f,0f));
                                generateWalls(new Vector3(62.27f,-13.71f,0), new Vector3(1.1f,1f,1f), new Vector3(-24.10058f,45.79883f,0f));

                                //endpoint.transform.position = new Vector3(-24.5f,12f,0);

                                GameObject camera = GameObject.Find("Camera1");
                                camera.GetComponent<Camera>().enabled = false;
                                camera = GameObject.Find("Main Camera");
                                camera.GetComponent<CameraFollow>().target = hero.transform;
                                GameObject vcam = GameObject.Find("vcam1");
                                vcam.GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = hero.transform;
                                GameObject obj = GameObject.Find("NinjaFrog");
                                obj.SetActive(false);
                                if(!IsOwner){
                                    Debug.Log("owner: "+OwnerClientId.ToString()+"ishost: "+IsHost.ToString());
                                    //gameObject.GetComponent<SpriteRenderer>().enabled = true;
                                }
                            }
                            else{
                            }
                        }
                        else{
                            if(sceneEvent.ClientId != 0){
                                if(IsOwner){
                                    transform.position = new Vector3(-43.3f,57.7f,0);

                                    //transform.position = new Vector3(-12.74f,12f,0);

                                    Debug.Log("owner: "+OwnerClientId.ToString()+"ishost: "+IsHost.ToString());
                                    //gameObject.GetComponent<SpriteRenderer>().enabled = true;
                                }
                            }
                        }
                        break;
                    }
                    // case("MainMenu"):{
                    //     Debug.Log("MainMenu loaded " + "sceneEventClientID: "+sceneEvent.ClientId.ToString()+"IsHost: "+IsHost.ToString()+"Isowner: "+IsOwner.ToString());
                    //     if(sceneEvent.ClientId == 0 && IsHost){
                    //         if(!IsOwner){
                    //             Debug.Log("Host signed out");
                    //             AuthenticationService.Instance.SignOut();
                    //         }
                    //     }
                    //     if(sceneEvent.ClientId != 0 && !IsHost){
                    //         if(IsOwner){
                    //             Debug.Log("client signed out");
                    //             AuthenticationService.Instance.SignOut();
                    //         }
                    //     }
                    //     break;
                    // }
                }
                break;
            }
        // Handle Client to Server Unload Complete Notification(s)
        case SceneEventType.UnloadComplete:
            {
                switch(sceneEvent.SceneName){
                    case("NetworkStartPage"):{
                        Debug.Log("NetworkStartPage Unloaded " + sceneEvent.ClientId.ToString());
                        if(IsHost){
                            if(sceneEvent.ClientId == 0){
                                if(IsOwner){
                                    gameObject.GetComponent<NetworkObject>().Despawn(true);
                                }
                            }
                        }
                        break;
                    }
                    case("Lobby"):{
                        Debug.Log("Lobby Unloaded " + sceneEvent.ClientId.ToString());
                        if(IsHost){
                            if(sceneEvent.ClientId == 0){
                                if(IsOwner){
                                    gameObject.GetComponent<NetworkObject>().Despawn(true);
                                }
                            }
                        }
                        break;
                    }
                }
                break;
            }
        // Handle Server to Client Load Complete (all clients finished loading notification)
        // case SceneEventType.LoadEventCompleted:
        //     {
        //         // This will let you know when all clients have finished loading a scene
        //         // Received on both server and clients
        //         // foreach (var clientId in sceneEvent.ClientsThatCompleted)
        //         // {
        //         //     // Example of parsing through the clients that completed list
        //         //     if (IsServer)
        //         //     {
        //         //         // Handle any server-side tasks here
        //         //     }
        //         //     else
        //         //     {
        //         //         // Handle any client-side tasks here
        //         //     }
        //         // }
        //         // break;
        //         switch(sceneEvent.SceneName){
        //             case("MainMenu"):{
        //                 Debug.Log("signed out");
        //                 AuthenticationService.Instance.SignOut();
        //                 break;
        //             }
        //         }
        //         break;
        //     }
        // Handle Server to Client unload Complete (all clients finished unloading notification)
        case SceneEventType.UnloadEventCompleted:
            {
                // This will let you know when all clients have finished unloading a scene
                // Received on both server and clients
                foreach (var clientId in sceneEvent.ClientsThatCompleted)
                {
                    // Example of parsing through the clients that completed list
                    if (IsServer)
                    {
                        // Handle any server-side tasks here
                    }
                    else
                    {
                        // Handle any client-side tasks here
                    }
                }
                break;
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log("OnNetworkSpawn");
        //playerPrefab = NetworkManager.Singleton.NetworkConfig.PlayerPrefab;

        //playerPrefab = GameObject.Find("NinjaFrog(Clone)");
        //playerPrefab.GetComponent<SpriteRenderer>().enabled = false;

        gameObject.GetComponent<SpriteRenderer>().enabled = false;

        // playerPrefab.GetComponent<NetworkObject>().NetworkHide(0);
        // playerPrefab.GetComponent<NetworkObject>().NetworkHide(1);
        //Debug.Log("OwnerClientId: " + OwnerClientId.ToString() + "Spawn");
        Debug.Log("OnSceneEventHooked");
        NetworkManager.Singleton.SceneManager.OnSceneEvent += SceneManager_OnSceneEvent;
        //NetworkServer.RegisterHandler(MsgType.Connect, OnPlayerConnected);
    }
    // void Born()
    // {
    //     healthBar.SetMaxHealth(maxHealth);
    // }

    void ResetSpikes()
    {
        if(SpikeList != null){
            foreach (GameObject obj in SpikeList)
            {
                obj.GetComponent<RightSpikeBehavior>().Reset();
            }
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
        Debug.Log("ResetFloors called:"+floors.Length.ToString());
        if(floors != null)
            foreach (GameObject obj in floors)
            {
                obj.GetComponent<Disappearedplatform>().Reset();
            }
    }
    void ParticleSystem()
    {
        playerPS.Play();
    }
    private void generateWalls(Vector3 pos, Vector3 scale, Vector3 bias)
    {
        GameObject wall = Instantiate(Resources.Load("Prefabs/DisappearwallNet") as GameObject);
        wall.transform.position = pos+bias;
        wall.transform.localScale = scale;
        wall.GetComponent<NetworkObject>().Spawn(true);
    }
}

