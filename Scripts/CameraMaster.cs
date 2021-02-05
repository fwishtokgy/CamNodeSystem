using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace CamNodeSpace
{
    public class CameraMaster : Singleton<CameraMaster>
    {
        [SerializeField]
        protected GameObject CameraPrefab;

        protected Camera Camera;
        protected CamTransitions transitioner;

        protected CameraNode currentnode;

        [SerializeField]
        protected bool LoadAsStandalone;

        public bool IsTakingRequests { get; protected set; }

        protected bool IsOn;

        public bool IsInitialized = false;

        void Awake()
        {
            IsOn = false;
            IsTakingRequests = true;
            Camera = this.GetComponentInChildren<Camera>();
            transitioner = this.GetComponentInChildren<CamTransitions>();
        }
        private void Start()
        {
            if (LoadAsStandalone)
            {
                IsTakingRequests = false;
                StartLevelWithFade();
            }
            IsInitialized = true;
            if (Camera == null)
            {
                HardCameraReturn();
            }
            Camera.enabled = true;
        }

        #region CameraNodeFunctions
        /// <summary>
        /// Called by a node that wishes to use the camera
        /// </summary>
        /// <param name="node">Reference of the calling node</param>
        /// <param name="FadeToTransition">Whether to fade out from the old node to this current node</param>
        /// <returns>Returns a reference to the root transform of the camera</returns>
        public Transform CameraRequest(CameraNode node, bool FadeToTransition = true)
        {
            if (IsTakingRequests)
            {
                IsTakingRequests = false;
                StartCoroutine(SwitchCameras(node, FadeToTransition));
                return Camera.transform;
            }
            return null;
        }
        /// <summary>
        /// Called by a node that wishes to close out its use of a camera
        /// </summary>
        /// <param name="FadeToTransition">Whether to fade out from the current node 
        public void CameraReturn(bool FadeToTransition = true)
        {
            if (IsOn)
            {
                IsTakingRequests = false;
                StartCoroutine(CloseCamera(FadeToTransition));
            }
            else
            {
                HardCameraReturn();
            }
        }
        public void HardCameraReturn()
        {
            currentnode = null;
            if (Camera == null)
            {
                Camera = Instantiate(CameraPrefab).GetComponent<Camera>();
                transitioner = Camera.GetComponent<CamTransitions>();
            }
            Camera.transform.parent = this.transform;
        }

        IEnumerator SwitchCameras(CameraNode node, bool FadeToTransition )
        {
            currentnode = node;
            if (FadeToTransition)
            {
                if (IsOn)
                {
                    var fadeout = transitioner.FadeOut();
                    OnStartClose.Invoke();
                    yield return fadeout;
                    OnCloseDone.Invoke();
                    yield return new WaitForEndOfFrame();
                }
                var fadein = transitioner.FadeIn();
                OnStartOpen.Invoke();
                yield return fadein;
                OnOpenDone.Invoke();
            }
            else
            {
                if (IsOn)
                {
                    OnStartClose.Invoke();
                    yield return new WaitForEndOfFrame();
                    OnCloseDone.Invoke();
                }
                OnStartOpen.Invoke();
                yield return new WaitForEndOfFrame();
                OnOpenDone.Invoke();
            }
            IsOn = true;
            IsTakingRequests = true;
        }
        IEnumerator CloseCamera(bool FadeToTransition)
        {
            if (FadeToTransition)
            {
                var fadeout = transitioner.FadeOut();
                OnStartClose.Invoke();
                yield return fadeout;
            }
            else
            {
                OnStartClose.Invoke();
            }
            IsOn = false;
            IsTakingRequests = true;
        }
        private void Update()
        {
            if (Camera == null)
            {
                HardCameraReturn();
            }
        }
        #endregion

        public class CameraNodeEvent : UnityEvent { }
        public CameraNodeEvent OnStartOpen = new CameraNodeEvent();
        public CameraNodeEvent OnStartClose = new CameraNodeEvent();
        public CameraNodeEvent OnOpenDone = new CameraNodeEvent();
        public CameraNodeEvent OnCloseDone = new CameraNodeEvent();

        #region LevelFades
        void StartLevelWithFade()
        {
            StartCoroutine(LevelStarter());
        }
        void CloseLevelWithFade()
        {
            IsTakingRequests = false;
            StartCoroutine(LevelEnder());
        }

        IEnumerator LevelStarter()
        {
            var levelFadeIn = transitioner.FadeIn();
            yield return levelFadeIn;
            IsOn = true;
            IsTakingRequests = true;
            onLevelInitiated.Invoke();
        }
        IEnumerator LevelEnder()
        {
            IsTakingRequests = false;
            var levelFadeOut = transitioner.FadeOut();
            yield return levelFadeOut;
            IsTakingRequests = true;
            onLevelClosed.Invoke();
        }
        #endregion
        #region LevelEvents
        /// <summary>
        /// The UnityEvent that will be sent when this UI item is pressed down upon.
        /// </summary>
        [Serializable]
        public class CameraLevelEvent : UnityEvent { }
        /// <summary>
        /// An instance of the custom UnityEvent for handling initial touch downs.
        /// </summary>
        [FormerlySerializedAs("onLevelInitiated")]
        [SerializeField]
        protected CameraLevelEvent onLevelInitiated = new CameraLevelEvent();
        /// <summary>
        ///  Public accessor to the event that fires on an initial touch down.
        /// </summary>
        public virtual CameraLevelEvent OnLevelInitiated
        {
            get
            {
                return onLevelInitiated;
            }
            set
            {
                onLevelInitiated = value;
            }
        }

        /// <summary>
        /// An instance of the custom UnityEvent for handling initial touch downs.
        /// </summary>
        [FormerlySerializedAs("onLevelClosed")]
        [SerializeField]
        protected CameraLevelEvent onLevelClosed = new CameraLevelEvent();
        /// <summary>
        ///  Public accessor to the event that fires on an initial touch down.
        /// </summary>
        public virtual CameraLevelEvent OnLevelClosed
        {
            get
            {
                return onLevelClosed;
            }
            set
            {
                onLevelClosed = value;
            }
        }
        #endregion
    }
}