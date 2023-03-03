using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerStats : NetworkBehaviour
{
    private static PlayerStats Instance => m_Instance;
    private NetworkVariable<int> missionsTotals = new NetworkVariable<int>(value: 10, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<int> totalJugadors = new NetworkVariable<int>(value: 0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<bool> susAsignado = new NetworkVariable<bool>(value:false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private static PlayerStats m_Instance;
    private GUIPlayer guiPlayer;
    private PlayerNetwork pn;

    private void Awake()
    {
        if (m_Instance != null && m_Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        m_Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        guiPlayer = FindObjectOfType<GUIPlayer>();
        if (IsServer) { 
        totalJugadors.OnValueChanged += (int previousValue, int newValue) =>
        {
            guiPlayer.aumentarMissionesTotalesServerRpc();
        };
        missionsTotals.OnValueChanged += (int previousValue, int newValue) =>
        {
           
        };
        }
    }

 public bool susActivado()
    {
        return susAsignado.Value;
    }

    private void Update()
    {
        
    }

    [ServerRpc(RequireOwnership = false)]
    public void aumentarJugadorsTotalsServerRpc()
    {
        if(IsServer) 
            this.totalJugadors.Value++;
    }
    public bool asignarSus()
    {
        if (!IsServer) return false;
        if (!susAsignado.Value)
        {
            int rnd = Random.Range(0, 1);
            if (rnd == 1)
            {
                print(rnd);
                susAsignado.Value = true;
                return true;
            }
            else return false;
        }
        else return false;
    }

}
