using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menuManager : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
    }
    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
    }
    public void StartJoin()
    {
        NetworkManager.Singleton.StartClient();
    }
}
