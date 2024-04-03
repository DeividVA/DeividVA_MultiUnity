using System.Collections;
using System.Collections.Generic;
using ED.SC;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class AvatarController : NetworkBehaviour
{
    [SerializeField] private float movementSpeed = 3.0f;
    [SerializeField] private GameObject doorPrefab;
    [SerializeField] private GameObject buttonPrefab;
    private readonly List<Color> m_Colors = new List<Color>() { Color.blue, Color.green, Color.red };
    private SpriteRenderer m_Renderer;
    private static NetworkVariable<Vector3> m_Position = new NetworkVariable<Vector3>(Vector3.zero,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<Color> m_Color = new NetworkVariable<Color>(Color.black,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


    private static NetworkVariable<Vector3> l_Position = new NetworkVariable<Vector3>(Vector3.zero,
    NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private static NetworkVariable<Vector3> r_Position = new NetworkVariable<Vector3>(Vector3.zero,
    NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private static NetworkVariable<int> doorCount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);


    // Start is called before the first frame update
    void Start()
    {
        SmartConsole.Log($"ClientId {OwnerClientId}");
        if (OwnerClientId > 2)
        {
            NetworkManager.Singleton.DisconnectClient(OwnerClientId);
            Destroy(gameObject);
            return;
        }
        // player id defines player position
        transform.position += Vector3.right * 2 * OwnerClientId;
        // get component for color change
        m_Renderer = GetComponent<SpriteRenderer>();
        // assign initial color to network variable
        if (IsOwner) m_Color.Value = m_Colors[(int)OwnerClientId];
        //ChangeColorServerRpc(m_Colors[(int) OwnerClientId]);
        if (IsServer && IsOwner) 
        {
            SpawnButtonsServerRpc();
            SmartConsole.Log("Initializing buttons");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        // sets color to network variable value
        if (m_Renderer.color != m_Color.Value) m_Renderer.color = m_Color.Value;

        if (!IsOwner) return;

        // movement
        if (Input.GetKey(KeyCode.LeftArrow)) LerpPosition(Vector3.left, movementSpeed);
        if (Input.GetKey(KeyCode.RightArrow)) LerpPosition(Vector3.right, movementSpeed);
        if (Input.GetKey(KeyCode.UpArrow)) LerpPosition(Vector3.up, movementSpeed);
        if (Input.GetKey(KeyCode.DownArrow)) LerpPosition(Vector3.down, movementSpeed);

        // color change
        // if you need to change something not in transform, it's needed RPC calling
        // RPC calling executes the method in several clients (specified by RpcTarget)
        // if (Input.GetKeyDown(KeyCode.RightControl)) m_Color.Value = m_Colors[2];
        //if (Input.GetKeyDown(KeyCode.RightControl)) SpawnDoorServerRpc();



    }

    void LerpPosition(Vector3 offset, float speed)
    {
        Vector3 positionNow = transform.position;
        transform.position = Vector3.Lerp(positionNow, positionNow + offset, Time.deltaTime * speed);
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnDoorServerRpc()
    {

        m_Position.Value += Vector3.up * 3;
        SmartConsole.Log("spawn door");
        GameObject door = Instantiate(doorPrefab, m_Position.Value, Quaternion.identity);

        door.GetComponent<NetworkObject>().Spawn();
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnButtonsServerRpc()
    {

        l_Position.Value += Vector3.left * 5;
        r_Position.Value += Vector3.right * 5;

        SmartConsole.Log("spawn buttons");
        GameObject button1 = Instantiate(buttonPrefab, l_Position.Value, Quaternion.identity);
        GameObject button2 = Instantiate(buttonPrefab, r_Position.Value, Quaternion.identity);


        button1.GetComponent<NetworkObject>().Spawn();
        button2.GetComponent<NetworkObject>().Spawn();

    }


    [ServerRpc(RequireOwnership = false)]
    void DoorCountIncreasingServerRpc()
    {

        doorCount.Value++;
    }

    [ServerRpc(RequireOwnership = false)]
    void DoorCountDecreasingServerRpc()
    {

        doorCount.Value--;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) return;
        
        //collision.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        DoorCountIncreasingServerRpc();
        SmartConsole.Log($"Door is {doorCount.Value}");
        if (doorCount.Value == 2) SpawnDoorServerRpc();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!IsServer) return;

        //collision.gameObject.GetComponent<BoxCollider2D>().enabled = true;
        DoorCountDecreasingServerRpc();
        SmartConsole.Log($"Door is {doorCount.Value}");
        //if (doorCount.Value >= 2) SpawnDoorServerRpc();
    }



    // [ServerRpc(RequireOwnership = false)]
    // void ChangeColorServerRpc(Color newColor)
    // {
    //     ChangeColorClientRpc(newColor);
    // }
    //
    // [ClientRpc]
    // void ChangeColorClientRpc(Color newColor)
    // {
    //     SmartConsole.Log($"New color {newColor}");
    //     if(m_Renderer!=null) m_Renderer.color = newColor;
    // }
}