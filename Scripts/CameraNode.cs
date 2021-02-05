using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using static CamNodeSpace.CameraMaster;

namespace CamNodeSpace
{
    public class CameraNode : MonoBehaviour
    {
        [SerializeField]
        protected List<Transform> Positions;
        protected int PositionIndex;
        protected Transform cameraTransform;

        private void Start()
        {
            if (Positions == null || Positions.Count == 0)
            {
                Positions = new List<Transform>();
                Positions.Add(this.transform);
                PositionIndex = 0;
            }
        }
        public void Activate()
        {
            CameraMaster.Instance.OnStartOpen.AddListener(StartOpen);
            cameraTransform = CameraMaster.Instance.CameraRequest(this);
        }
        public void Deactivate()
        {
            CameraMaster.Instance.CameraReturn();
        }

        private void StartOpen()
        {
            CameraMaster.Instance.OnStartOpen.RemoveAllListeners();
            CameraMaster.Instance.OnOpenDone.AddListener(OpenDone);
            onOpenStart.Invoke();
            StartCoroutine(WaitForOpenCompletion());
        }
        private void OpenDone()
        {
            CameraMaster.Instance.OnOpenDone.RemoveAllListeners();
            CameraMaster.Instance.OnStartClose.AddListener(StartClose);
            onOpenDone.Invoke();
            
        }
        private void StartClose()
        {
            CameraMaster.Instance.OnStartClose.RemoveAllListeners();
            CameraMaster.Instance.OnCloseDone.AddListener(CloseDone);
            onCloseStart.Invoke();
        }
        private void CloseDone()
        {
            CameraMaster.Instance.OnCloseDone.RemoveAllListeners();
            onCloseDone.Invoke();
            StartCoroutine(WaitForCloseCompletion());
        }
        private void FinishDeactivation()
        {
            onCloseDone.Invoke();
        }
        IEnumerator WaitForOpenCompletion()
        {
            yield return new WaitForEndOfFrame();
            UpdateCameraParent();
        }
        IEnumerator WaitForCloseCompletion()
        {
            yield return new WaitForEndOfFrame();
            FinishDeactivation();
        }

        public void ChangePositionIndex(int newIndex)
        {
            PositionIndex = newIndex;
            UpdateCameraParent();
        }
        protected void UpdateCameraParent()
        {
            if (cameraTransform != null)
            {
                cameraTransform.parent = Positions[PositionIndex];
                cameraTransform.localPosition = Vector3.zero;
                cameraTransform.localRotation = Quaternion.identity;
            }
        }

        #region ActivationEvents
        /// <summary>
        /// The UnityEvent that will be sent when this UI item is pressed down upon.
        /// </summary>
        [Serializable]
        public class CamNodeEvent : UnityEvent { }
        /// <summary>
        /// An instance of the custom UnityEvent for handling initial touch downs.
        /// </summary>
        [FormerlySerializedAs("onOpenStart")]
        [SerializeField]
        protected CamNodeEvent onOpenStart = new CamNodeEvent();
        /// <summary>
        ///  Public accessor to the event that fires on an initial touch down.
        /// </summary>
        public virtual CamNodeEvent OnOpenStart
        {
            get
            {
                return onOpenStart;
            }
            set
            {
                onOpenStart = value;
            }
        }
        /// <summary>
        /// An instance of the custom UnityEvent for handling initial touch downs.
        /// </summary>
        [FormerlySerializedAs("onOpenDone")]
        [SerializeField]
        protected CamNodeEvent onOpenDone = new CamNodeEvent();
        /// <summary>
        ///  Public accessor to the event that fires on an initial touch down.
        /// </summary>
        public virtual CamNodeEvent OnOpenDone
        {
            get
            {
                return onOpenDone;
            }
            set
            {
                onOpenDone = value;
            }
        }
        /// <summary>
        /// An instance of the custom UnityEvent for handling initial touch downs.
        /// </summary>
        [FormerlySerializedAs("onCloseStart")]
        [SerializeField]
        protected CamNodeEvent onCloseStart = new CamNodeEvent();
        /// <summary>
        ///  Public accessor to the event that fires on an initial touch down.
        /// </summary>
        public virtual CamNodeEvent OnCloseStart
        {
            get
            {
                return onCloseStart;
            }
            set
            {
                onCloseStart = value;
            }
        }
        /// <summary>
        /// An instance of the custom UnityEvent for handling initial touch downs.
        /// </summary>
        [FormerlySerializedAs("onCloseDone")]
        [SerializeField]
        protected CamNodeEvent onCloseDone = new CamNodeEvent();
        /// <summary>
        ///  Public accessor to the event that fires on an initial touch down.
        /// </summary>
        public virtual CamNodeEvent OnCloseDone
        {
            get
            {
                return onCloseDone;
            }
            set
            {
                onCloseDone = value;
            }
        }
        #endregion
    }
}