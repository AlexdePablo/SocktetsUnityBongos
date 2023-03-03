using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;



public class LobbyController : NetworkBehaviour
{
    [SerializeField]
    private int maxPlayers = 4;
    private bool HayAlguien;

    public TMP_Text Texto;
    private Dictionary<ulong, bool> Clientes;
    private string LobbyTexto;
    public override void OnNetworkSpawn()
    {
        Clientes = new Dictionary<ulong, bool>();

        Clientes.Add(NetworkManager.Singleton.LocalClientId, false);

        if (IsServer)
        {
            HayAlguien = false;

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
            SceneTransitionHandler.sceneHandler.clienteCargaEscena += CargarEscena;
        }

        EntrarLobbyTexto();

       // SceneTransitionHandler.sceneHandler.NombreDeEscena(SceneTransitionHandler.SceneStates.Lobby);
    }
    private void CheckearPlayers()
    {
        HayAlguien = Clientes.Count >= 2;

        foreach (var clientLobbyStatus in Clientes)
        {
            ClienteReadyClientRpc(clientLobbyStatus.Key, clientLobbyStatus.Value);
            if (!NetworkManager.Singleton.ConnectedClients.ContainsKey(clientLobbyStatus.Key))
                HayAlguien = false;

        }

        CheckearPlayersListos();
    }
    private void OnGUI()
    {
        if (Texto != null) Texto.text = LobbyTexto;
    }
    private void CargarEscena(ulong clientId)
    {
        if (IsServer)
        {
            if (!Clientes.ContainsKey(clientId) && Clientes.Count <= maxPlayers)
            {
                Clientes.Add(clientId, false);
                EntrarLobbyTexto();
            }

            CheckearPlayers();
        }
    }
    private void EntrarLobbyTexto()
    {
        LobbyTexto = string.Empty;
        foreach (var clientLobbyStatus in Clientes)
        {

            
            LobbyTexto += "PLAYER" + clientLobbyStatus.Key + "          ";


            if (clientLobbyStatus.Value)
                LobbyTexto += "READY\n";
            else
                LobbyTexto += "NOT READY\n";
        }
    }


    private void OnClientConnectedCallback(ulong clientId)
    {
        if (IsServer)
        {
            if (!Clientes.ContainsKey(clientId)) Clientes.Add(clientId, false);


            EntrarLobbyTexto();

            CheckearPlayers();
        }
    }
    private void CheckearPlayersListos()
    {
        if (HayAlguien)
        {
            var EveryOneIsReady = true;

            foreach (var StatusClient in Clientes)
                if (!StatusClient.Value)
                    EveryOneIsReady = false;

            if (EveryOneIsReady)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
                SceneTransitionHandler.sceneHandler.clienteCargaEscena -= CargarEscena;
                SceneTransitionHandler.sceneHandler.CambioDeEscena("Alex");
            }
        }
    }
    public void PlayerListo()
    {
        Clientes[NetworkManager.Singleton.LocalClientId] = true;
        if (IsServer)
        {

            CheckearPlayers();

        }
        else
        {

            ClienteReadyRecibidoServerRpc(NetworkManager.Singleton.LocalClientId);

        }

        EntrarLobbyTexto();
    }
    [ClientRpc]
    private void ClienteReadyClientRpc(ulong clientId, bool isReady)
    {
        if (!IsServer)
        {
            if (!Clientes.ContainsKey(clientId))
                Clientes.Add(clientId, isReady);
            else
                Clientes[clientId] = isReady;
            EntrarLobbyTexto();
        }
    }



    [ServerRpc(RequireOwnership = false)]
    private void ClienteReadyRecibidoServerRpc(ulong clientid)
    {
        if (Clientes.ContainsKey(clientid))
        {
            Clientes[clientid] = true;
            CheckearPlayers();
            EntrarLobbyTexto();
        }
    }

}
