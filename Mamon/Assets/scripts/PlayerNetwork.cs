using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Multiplayer.Samples.Utilities;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerNetwork : NetworkBehaviour
{
    NetworkVariable<float> m_Speed = new NetworkVariable<float>(10);
    private NetworkVariable<int> velocity = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    // Start is called before the first frame update
    private NetworkVariable<Color> color = new NetworkVariable<Color>(Color.blue, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<Vector3> posicion = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Owner, NetworkVariableWritePermission.Owner);
    [SerializeField]
    private TMP_Text missionesText;
    private NetworkVariable<bool> sus = new NetworkVariable<bool>(value:false, NetworkVariableReadPermission.Owner, NetworkVariableWritePermission.Owner);

    [SerializeField] public TMPro.TMP_InputField input;
    private new Perseguir camera;
    [SerializeField]
    private int m_MoveSpeed;
    [SerializeField]
    private float m_addVelocity;
    private bool m_inMission;
    private bool canMission;
    private GameObject quadrat;
    //[SerializeField]
    //  Camera m_Camera;
    Rigidbody2D m_rigidBody;
    // Start is called before the first frame update
    //NetworkVariable<Camera[]> m_cameres = new NetworkVariable<Camera[]>();







    [HideInInspector]
    public PlayerStats m_Stats;
    [HideInInspector]
    public GUIPlayer guiPlayer;




    void Start()
    {
        if (IsOwner)
            posicion.Value = transform.position;

        m_rigidBody = GetComponent<Rigidbody2D>();  
        m_inMission = false;
        canMission = false;
    }

    public override void OnNetworkSpawn()
    {

        if(!IsOwner) return;
        camera = FindObjectOfType<Perseguir>();
        guiPlayer = FindObjectOfType<GUIPlayer>();
        m_Stats = FindObjectOfType<PlayerStats>();
       

        // missionesText = FindObjectOfType<TMP_Text>();   
        m_Stats.aumentarJugadorsTotalsServerRpc();

        camera.setTarget(transform);

        /*missionsCompletades.OnValueChanged += (int previousValue, int newValue) =>
        {
            print("MISIONES TOTALES" + newValue);
           // missionesText.text = "Missions realitzades: " + missionsCompletades.Value + "/" + missionsTotals;
        };*/

        posicion.OnValueChanged += (Vector3 previousValue, Vector3 newValue) =>
        {
           // print("new value->" + newValue);
            camera.transform.position += newValue;
        };


    }

    [ClientRpc]
    void PongClientRpc(int somenumber, string sometext) 
    { 
        //print(somenumber + " " + sometext);
    }

    // Update is called once per frame
    void Update()
    {
        //print("Missions completades" + guiPlayer.missionsCompletades.Value);

        //Aquest update només per a qui li pertany
        if (!IsOwner) return;
        m_rigidBody.velocity = Vector3.zero;
        _ = Vector3.zero;


        Vector3 movement = Vector3.zero;

        if (Input.GetKey(KeyCode.I))
            movement += Vector3.up;
        if (Input.GetKey(KeyCode.K))
            movement -= Vector3.up;
        if (Input.GetKey(KeyCode.J))
            movement -= Vector3.right;
        if (Input.GetKey(KeyCode.L))
            movement += Vector3.right;

        if (movement != Vector3.zero)
            //Qui farà els moviments serà el servidor, alleugerim i només canvis quan hi hagi input
            MoveCharacterPhysicsServerRpc(movement.normalized * m_Speed.Value);


        if (Input.GetKey(KeyCode.Space))
        {
            if(canMission)
            StartCoroutine("Mision");
        }
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            PongClientRpc(Time.frameCount, "hello, world"); // Server -> Client
        }

        if (!m_inMission)
        {

        
        if (Input.GetKey(KeyCode.A))
        {
            m_rigidBody.velocity = new Vector2(m_MoveSpeed * -m_addVelocity, m_rigidBody.velocity.y);
            posicion.Value = transform.position;
        }
           
        if (Input.GetKey(KeyCode.D))
        {
            m_rigidBody.velocity = new Vector2(m_MoveSpeed * m_addVelocity, m_rigidBody.velocity.y);
            posicion.Value = transform.position;
        }
           
        if (Input.GetKey(KeyCode.W))
        {
            m_rigidBody.velocity = new Vector2(m_rigidBody.velocity.x, m_MoveSpeed * m_addVelocity);
            posicion.Value = transform.position;
        }
           
        if (Input.GetKey(KeyCode.S))
        {
            m_rigidBody.velocity = new Vector2(m_rigidBody.velocity.x, m_MoveSpeed * -m_addVelocity);
            posicion.Value = transform.position;
        }
           

        if (Input.GetKey(KeyCode.C))
            color.Value = Color.red;
        }
        else
        {
           // print("treballant");
        }
    }

    private void LateUpdate()
    {
        if (!IsOwner) return;

        if(camera)
        camera.transform.position = transform.position - Vector3.forward*10;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!IsOwner) return;
        if (collision.transform.tag == "Mision")
        {
            
          
        }
            
    }

    IEnumerator Mision()
    {
        if (!m_inMission) {
            m_inMission = true;
            yield return new WaitForSeconds(2f);
            quadrat.GetComponent<SpriteRenderer>().color = Color.green;
            guiPlayer.aumentarMissionesServerRpc();
            m_inMission = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (!IsOwner)
            return;

        // If the collider hit a power-up
        if (collider.gameObject.CompareTag("Mision"))
        {
            if(collider.gameObject.GetComponent<SpriteRenderer>().color != Color.green)
            {
                canMission = true;
                quadrat = collider.gameObject;
            }

        }
    }

    [ServerRpc]
    private void MoveCharacterPhysicsServerRpc(Vector3 velocity, ServerRpcParams serverRpcParams = default)
    {
        Debug.Log(string.Format("{0} => El client {1} vol modificar la velocitat {2}", OwnerClientId, serverRpcParams.Receive.SenderClientId, m_Speed.Value));
        m_rigidBody.velocity = velocity;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!IsOwner)
            return;

        // If the collider hit a power-up
        if (collision.gameObject.CompareTag("Mision"))
        {
            canMission = false;
        }
    }
}
