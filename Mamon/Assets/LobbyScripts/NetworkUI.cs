using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class NetworkUI : MonoBehaviour
{
    [SerializeField] private Button hostB;
    [SerializeField] private Button clientB;
    [SerializeField] public TMPro.TMP_InputField input;
    void Awake()
    {

       

        clientB.onClick.AddListener(() =>
        {
            try
            {

                if (input.text.Length > 0)
                {
                    int a = int.Parse(input.text);
                    NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Port = (ushort)int.Parse(input.text);
                    NetworkManager.Singleton.StartClient();
             

                }
            }
            catch (System.Exception e)
            {
                print(e.Message);
                input.text = "";
            }
        });

        hostB.onClick.AddListener(() =>
        {
            try
            {

                if (input.text.Length > 0)
                {
                    int a = int.Parse(input.text);
                    NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Port = (ushort)int.Parse(input.text);
                    NetworkManager.Singleton.StartHost();
                    SceneTransitionHandler.sceneHandler.MeLlaman();
                    SceneTransitionHandler.sceneHandler.CambioDeEscena("Lobby");

                }
            }
            catch (System.Exception e)
            {
                print(e.Message);
                input.text = "";
            }
        });
    }

}
