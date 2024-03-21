using System.Collections;
using System.Collections.Generic;
using ED.SC;
using Unity.Netcode;
using UnityEngine;

public class AvatarController : NetworkBehaviour
{
    private List<Color> colors = new List<Color>() { Color.blue, Color.green, Color.red };

    // Start is called before the first frame update
    void Start()
    {
        SmartConsole.Log($"ClientId {OwnerClientId}");
        transform.position += Vector3.right * 2 * OwnerClientId;
        GetComponent<SpriteRenderer>().color = colors[(int)OwnerClientId];
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        // movement
        if (Input.GetKeyDown(KeyCode.LeftArrow)) transform.position += new Vector3(-1, 0, 0);
        if (Input.GetKeyDown(KeyCode.RightArrow)) transform.position += new Vector3(1, 0, 0);
        if (Input.GetKeyDown(KeyCode.DownArrow)) transform.position += new Vector3(0, -1, 0);
        if (Input.GetKeyDown(KeyCode.UpArrow)) transform.position += new Vector3(0, 1, 0);

        // color change
        // if you need to change something not in transform, it's needed RPC calling
        // RPC calling executes the method in several clients (specified by RpcTarget)
        if (Input.GetKeyDown(KeyCode.RightControl)) GetComponent<SpriteRenderer>().color = colors[2];
    }
}