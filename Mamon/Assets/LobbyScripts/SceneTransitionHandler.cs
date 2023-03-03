using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionHandler : NetworkBehaviour
{

    public enum SceneStates
    {
        Menu,
        Lobby,
        Ingame
    }



    static public SceneTransitionHandler Instance => sceneHandler;
    static public SceneTransitionHandler sceneHandler;
    private int nGenteUniendose;
    public delegate void HandlerCargarEscena(ulong clientId);
    public event HandlerCargarEscena clienteCargaEscena;



    private SceneStates estado;
    void Awake()
    {
        if (sceneHandler != null && sceneHandler != this) {
            Destroy(gameObject);
        }
        sceneHandler = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        SceneManager.LoadScene("Menu");
    }
    private void CargaCompleta(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        nGenteUniendose += 1;
        clienteCargaEscena?.Invoke(clientId);
    }

    public void MeLlaman()
    {
        NetworkManager.Singleton.SceneManager.OnLoadComplete += CargaCompleta;

    }

    public void CambioDeEscena(string scenename)
    {
        if (NetworkManager.Singleton.IsListening)
        {
            nGenteUniendose = 0;
            NetworkManager.Singleton.SceneManager.LoadScene(scenename, LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadSceneAsync(scenename);
        }
    }
  
}
