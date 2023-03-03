using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
public class NetworkmanagerUI : MonoBehaviour
{
    [SerializeField] private Button serverB;
    [SerializeField] private Button hostB;
    [SerializeField] private Button clientB;
    void Awake()
    {

        serverB.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
        });

        clientB.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        });

        hostB.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
        });
    }
}
