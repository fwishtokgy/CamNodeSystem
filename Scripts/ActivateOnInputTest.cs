using CamNodeSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CameraNode))]
public class ActivateOnInputTest : MonoBehaviour
{
    protected CameraNode camNode;
    public KeyCode input;
    // Start is called before the first frame update
    void Start()
    {
        camNode = this.GetComponent<CameraNode>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(input))
        {
            camNode.Activate();
        }
    }
}
