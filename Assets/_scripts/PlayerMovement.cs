using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Multiplayer.Tools.NetStatsMonitor;
using Unity.Netcode;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    private Rigidbody2D rb;
    public Transform MyCamera;
    public Transform Turret;

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

    [Header("Movement")]
    private Vector2 inputZQSD;
    [SerializeField] private float forceMove = 10f;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (IsOwner)
        {
            //AddScoreTestServerRpc("Ca gaz jules roualt?");
            //AddScoreTestServerRpc(new ServerRpcParams());
            TestClientRpc(Random.Range(0,10));
        }
    }

    private void FixedUpdate()
    {
        Movement();
    }


    private void Movement()
    {
        rb.velocity = inputZQSD * forceMove * Time.fixedDeltaTime;
    }


    #region Handler
    //context.phase = Started performed canceled
    public void OnMove(InputAction.CallbackContext context)
    {
        //Debug.Log(context.phase);
        if (!IsOwner)
        {
            return;
        }
        inputZQSD = context.ReadValue<Vector2>();
    }
    public void LookMouse(InputAction.CallbackContext context)
    {
        if (!IsOwner)
        {
            return;
        }
        Vector2 mousePosition = context.ReadValue<Vector2>();
        Vector2 worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector2 target = worldMousePosition - (Vector2)transform.position;

        float angle = 0;

        angle = Mathf.Atan2(target.y, target.x)* Mathf.Rad2Deg;
        Turret.rotation = Quaternion.Euler(0, 0, angle -90);
    }

    // FCT TO TEST NETWORKVARIABLE
    public void AddScoreTest()
    {
        Debug.Log(OwnerClientId + ' ' + score.ToString());
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
        Debug.Log(OwnerClientId + " fct rpc call : "+message);
    }

    // ServerRpcParams -> SenderClientId sert aussi à montrer qui call
    [ServerRpc]
    public void AddScoreTestServerRpc(ServerRpcParams paramsRpc)
    {
        // seul le owner peut call server
        Debug.Log(OwnerClientId + " fct rpc call : " + paramsRpc.Receive.SenderClientId);
    }

    // fcts Server -> Clients
    [ClientRpc]
    public void TestClientRpc(int a)
    {
        Debug.Log(OwnerClientId + " fct rpc call Client" + a);
    }

    [ClientRpc]
    public void TestToClientSpecificClientRpc(ClientRpcParams paramsRpc)
    {
        Debug.Log(OwnerClientId + " to " + paramsRpc.Send.TargetClientIds);
    }
    #endregion
}
