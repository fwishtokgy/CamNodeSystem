using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CamNodeSpace
{
    public class DemandCameraOnAwake : MonoBehaviour
    {
        [SerializeField]
        protected CameraNode cameraNode;
        // Start is called before the first frame update
        void Awake()
        {
            cameraNode.Activate();
        }
    }
}