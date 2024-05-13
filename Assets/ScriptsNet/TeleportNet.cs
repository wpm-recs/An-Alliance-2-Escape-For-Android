using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TeleportNet : NetworkBehaviour
{
    private NetworkVariable<bool> teleportFlag= new NetworkVariable<bool>(false,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
    private float teleportCoolDown = 5f;
    private OnGroundDector OngroundDector;
    private Rigidbody2D Rigidbody;
    private float eps = 0.05f;
    private float negMax = -20f;
    // Start is called before the first frame update
    void Start()
    {
        OngroundDector=GetComponent<OnGroundDector>();
        Rigidbody=GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(IsOwner){
            if(Input.GetKey(KeyCode.K)){
                Debug.Log("MaskDude On Ground: "+GameObject.Find("MaskDude(Clone)").GetComponent<OnGroundDector>().TestOnGround().ToString());
                Debug.Log("MaskDude Velocity: "+GameObject.Find("MaskDude(Clone)").GetComponent<Rigidbody2D>().velocity.ToString());
                Debug.Log("Frog On Ground: "+OngroundDector.TestOnGround().ToString());
                Debug.Log("Frog Velocity: "+Rigidbody.velocity.ToString());
                Debug.Log("Teleport Flag: "+(GameObject.Find("MaskDude(Clone)").GetComponent<OnGroundDector>().TestOnGround() && ((GameObject.Find("MaskDude(Clone)").GetComponent<Rigidbody2D>().velocity.y < eps && GameObject.Find("MaskDude(Clone)").GetComponent<Rigidbody2D>().velocity.y > -eps) || GameObject.Find("MaskDude(Clone)").GetComponent<Rigidbody2D>().velocity.y < negMax) && GameObject.Find("MaskDude(Clone)").GetComponent<Rigidbody2D>().velocity.x < eps && GameObject.Find("MaskDude(Clone)").GetComponent<Rigidbody2D>().velocity.x > -eps).ToString());
                //if(GameObject.Find("MaskDude(Clone)").GetComponent<OnGroundDector>().TestOnGround() && ((GameObject.Find("MaskDude(Clone)").GetComponent<Rigidbody2D>().velocity.y < eps && GameObject.Find("MaskDude(Clone)").GetComponent<Rigidbody2D>().velocity.y > -eps) || GameObject.Find("MaskDude(Clone)").GetComponent<Rigidbody2D>().velocity.y < negMax) && GameObject.Find("MaskDude(Clone)").GetComponent<Rigidbody2D>().velocity.x < eps && GameObject.Find("MaskDude(Clone)").GetComponent<Rigidbody2D>().velocity.x > -eps){
                if(Rigidbody.velocity.y == 0f && Rigidbody.velocity.x == 0f){
                    teleportFlag.Value = true;
                }
            }
            else{
                teleportFlag.Value = false;
            }
        }
        else{
            if(Input.GetKey(KeyCode.S)){
                Debug.Log("MaskDude On Ground: "+GameObject.Find("MaskDude(Clone)").GetComponent<OnGroundDector>().TestOnGround().ToString());
                Debug.Log("MaskDude Velocity: "+GameObject.Find("MaskDude(Clone)").GetComponent<Rigidbody2D>().velocity.ToString());
                Debug.Log("Frog On Ground: "+OngroundDector.TestOnGround().ToString());
                Debug.Log("Frog Velocity: "+Rigidbody.velocity.ToString());
                Debug.Log("Teleport Flag: "+(teleportFlag.Value && OngroundDector.TestOnGround() && teleportCoolDown == 0f && ((Rigidbody.velocity.y < eps && Rigidbody.velocity.y > -eps) || Rigidbody.velocity.y < negMax) && Rigidbody.velocity.x < eps && Rigidbody.velocity.x > -eps).ToString());
                //if(teleportFlag.Value && OngroundDector.TestOnGround() && teleportCoolDown == 0f && ((Rigidbody.velocity.y < eps && Rigidbody.velocity.y > -eps) || Rigidbody.velocity.y < negMax) && Rigidbody.velocity.x < eps && Rigidbody.velocity.x > -eps){
                if(teleportFlag.Value && teleportCoolDown == 0f && GameObject.Find("MaskDude(Clone)").GetComponent<Rigidbody2D>().velocity.y == 0f && GameObject.Find("MaskDude(Clone)").GetComponent<Rigidbody2D>().velocity.x == 0f){
                    Debug.Log("Teleport Flag[inside]: "+(teleportFlag.Value && OngroundDector.TestOnGround() && teleportCoolDown == 0f && ((Rigidbody.velocity.y < eps && Rigidbody.velocity.y > -eps) || Rigidbody.velocity.y < negMax) && Rigidbody.velocity.x < eps && Rigidbody.velocity.x > -eps).ToString());
                    teleportCoolDown = 5f;
                    GameObject maskDude = GameObject.Find("MaskDude(Clone)");
                    Vector3 maskDudePosition = maskDude.transform.position;
                    maskDude.transform.position = transform.position;
                    Debug.Log("Teleport trigger");
                    TeleportClientRpc(maskDudePosition);
                }
            }
        }
        if(teleportCoolDown > 0){
            teleportCoolDown -= Time.deltaTime;
        }
        else{
            teleportCoolDown = 0f;
        }
    }
    [ClientRpc]
    private void TeleportClientRpc(Vector3 maskDudePosition){
        if(!IsHost){
            Debug.Log("TeleportClientRpc called");
            transform.position = maskDudePosition;
        }
    }
}
