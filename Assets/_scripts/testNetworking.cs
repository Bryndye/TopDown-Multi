using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class testNetworking : NetworkBehaviour
{
    //Chaque variable Network doivent etre init sinon ERROR
    public NetworkVariable<int> score = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    // Pas le droit d'uitiliser les functions "classiques" => awake ou start pour les variables Network mais celles des varNetwork par override
    public override void OnNetworkSpawn()
    {
        score.OnValueChanged += (int previousValue, int newValue) =>
        {
            Debug.Log(OwnerClientId + " " + score.Value.ToString());
        };
    }

    // IsOwner permet de savoir si l'objet est au joueur donc a utilisé pour les inputs

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            //AddScoreTestServerRpc("Ca gaz jules roualt?");
            //AddScoreTestServerRpc(new ServerRpcParams());
            TestClientRpc(Random.Range(0, 10));
        }
    }

    // FCT TO TEST NETWORKVARIABLE
    public void AddScoreTest()
    {
        //Debug.Log(OwnerClientId + ' ' + score.ToString());
        if (IsOwner)
        {
            score.Value++;
            TestToClientSpecificClientRpc(new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { 0 } } });
        }
    }

    // fcts ServerRPC servent a appeler des fcts au Server donc les clients ne recevront pas lappel
    [ServerRpc]
    public void TestServerRpc(string message)
    {
        // seul le owner peut call server
        //Debug.Log(OwnerClientId + " fct rpc call : "+message);
    }

    // ServerRpcParams -> SenderClientId sert aussi à montrer qui call
    [ServerRpc]
    public void AddScoreTestServerRpc(ServerRpcParams paramsRpc)
    {
        // seul le owner peut call server
        //Debug.Log(OwnerClientId + " fct rpc call : " + paramsRpc.Receive.SenderClientId);
    }

    // fcts Server -> Clients
    [ClientRpc]
    public void TestClientRpc(int a)
    {
        //Debug.Log(OwnerClientId + " fct rpc call Client" + a);
    }

    [ClientRpc]
    public void TestToClientSpecificClientRpc(ClientRpcParams paramsRpc)
    {
        //Debug.Log(OwnerClientId + " to " + paramsRpc.Send.TargetClientIds);
    }
}
