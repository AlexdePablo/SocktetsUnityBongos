using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GUIPlayer : NetworkBehaviour
{
    private static GUIPlayer Instance => m_Instance;
    private NetworkVariable<int> missionsTotals = new NetworkVariable<int>(value: 3, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<int> missionsCompletades = new NetworkVariable<int>(value:0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    
    private static GUIPlayer m_Instance;
    [SerializeField]
    private TMP_Text texto;
    [SerializeField]
    private TMP_Text sus;

    private PlayerStats m_Stats;

    private void Awake()
    {
        if (m_Instance != null && m_Instance != this) {
            Destroy(gameObject);
            return;
        }
        m_Instance= this;
        DontDestroyOnLoad(gameObject);
     
    }

    public override void OnNetworkSpawn()
    {
        missionsCompletades.OnValueChanged += (int previousValue, int newValue) =>
        {
            texto.text = "Missions Completades " + missionsCompletades.Value + "/" + missionsTotals.Value;
        };
        missionsTotals.OnValueChanged += (int previousValue, int newValue) =>
        {
            texto.text = "Missions Completades " + missionsCompletades.Value + "/" + missionsTotals.Value;
        };
    }

    private void Update()
    {
        //print("Missions completades" + missionsCompletades.Value);
    }

    [ServerRpc (RequireOwnership = false)]
    public void aumentarMissionesServerRpc()
    {
        this.missionsCompletades.Value++;
    }
    [ServerRpc(RequireOwnership = false)]
    public void aumentarMissionesTotalesServerRpc()
    {
        if(IsServer)
        this.missionsTotals.Value += 3;
    }

    [ServerRpc(RequireOwnership = false)]
    public void guiSusServerRpc()
    {
        sus.text = "ERES SUS BRO";    
    }

}
