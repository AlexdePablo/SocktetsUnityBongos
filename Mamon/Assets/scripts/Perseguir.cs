using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Perseguir : MonoBehaviour
{
    private Transform m_target;
    
    public void setTarget(Transform target)
    {
        m_target = target;
    }
    private void LateUpdate()
    {
        if(m_target)
        transform.position = m_target.position - Vector3.forward * 10;
    }
}
