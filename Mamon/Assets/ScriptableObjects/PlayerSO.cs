using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PlayerSO/PlayerSO")]
public class PlayerSO : ScriptableObject
{
    public int idJugador;
    public string nombre;

    private void OnEnable()
    {
        VaciarData();
    }
    public void VaciarData() { 
        nombre=string.Empty;
        idJugador=-1;
    }

}
