using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ShootArrowNet : NetworkBehaviour
{
    private const float ShootPlace = 0.3f;
    MaskDudeBehaviorNet playerScript = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public override void OnNetworkSpawn()
    {
        playerScript = gameObject.GetComponent<MaskDudeBehaviorNet>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsHost && Input.GetKeyDown(KeyCode.RightShift)){
            Debug.Log("Shoot trigger");
            ShootServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ShootServerRpc(){
        Debug.Log("ShootServerRpc called");
        GameObject Arrow = Instantiate(Resources.Load("Prefabs/ArrowNet") as GameObject);
        Arrow.transform.localPosition = transform.localPosition+new Vector3(0,ShootPlace,0);
        ArrowBehaviorNet ArrowBehavior = Arrow.GetComponent<ArrowBehaviorNet>();
        ArrowBehavior.Direction = playerScript.FaceDirection.Value;
        if(ArrowBehavior.Direction == -1) Arrow.transform.Rotate(0,0,180,Space.Self);
        Arrow.GetComponent<NetworkObject>().Spawn();
        playerScript.AudioSource.clip = playerScript.shoot;
        playerScript.AudioSource.Play();
    }
}
