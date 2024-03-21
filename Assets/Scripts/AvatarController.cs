using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using ED.SC;

public class AvatarController : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SmartConsole.Log($"NetID{OwnerClientId}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
