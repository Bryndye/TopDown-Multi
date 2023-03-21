using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ManagerServer : NetworkBehaviour
{

    public Transform SpawnPoint;
    public Transform ObjectPrefabToInstantiate;
    bool did = false;

    //void Update()
    //{
    //    if (!IsServer)
    //        { return; }
    //    if (did)
    //        return;
    //    var T =  Instantiate(ObjectPrefabToInstantiate);
    //    T.GetComponent<NetworkObject>().Spawn(true);
    //    //Despawn existe 
    //    did = true;
    //    //Destroy(T.gameObject);
    //}
}
