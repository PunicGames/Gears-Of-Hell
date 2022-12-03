using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MultiplayerManager : MonoBehaviour
{

    [SerializeField] GameObject player;
    [SerializeField] NetworkManager manager;
    [SerializeField] GestorUIinGame guiManager;

    public void StartHost()
    {
        manager.StartHost();
    }
    public void StartClient()
    {
        manager.StartClient();
    }
    public void StartServer()
    {
    }


}
