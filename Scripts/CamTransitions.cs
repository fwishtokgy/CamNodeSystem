using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CamNodeSpace
{
    public class CamTransitions : MonoBehaviour
    {
        [SerializeField]
        protected float FadeTime;

        public bool IsFading { get; protected set; }

        protected Material fadeMaterial = null;
        protected Color currentColor;
        protected float alpha;

        private void Awake()
        {
            if (fadeMaterial == null)
            {
                // Unity has a built-in shader that is useful for drawing
                // simple colored things.
                Shader shader = Shader.Find("Hidden/Internal-Colored");
                fadeMaterial = new Material(shader);
                fadeMaterial.hideFlags = HideFlags.HideAndDontSave;
                // Turn on alpha blending
                fadeMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                fadeMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                // Turn backface culling off
                fadeMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                // Turn off depth writes
                fadeMaterial.SetInt("_ZWrite", 0);
            }
            var canvases = FindObjectsOfType<Canvas>();
            var camera = this.GetComponent<Camera>();
            foreach (var canvas in canvases)
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = camera;
            }
            currentColor = Color.black;
            DrawQuad(Color.black);
            IsFading = true;
        }

        public Coroutine FadeIn()
        {
            DrawQuad(Color.black);
            return StartCoroutine(Fade(1, 0));
        }
        public Coroutine FadeOut()
        {
            DrawQuad(Color.clear);
            return StartCoroutine(Fade(0, 1));
        }

        IEnumerator Fade(float startAlpha, float endAlpha)
        {
            var timer = 0f;
            IsFading = true;
            while (timer < FadeTime)
            {
                timer += Time.deltaTime;
                currentColor.a = Mathf.Lerp(startAlpha, endAlpha, (timer / FadeTime));
                yield return new WaitForEndOfFrame();
            }
            if (endAlpha > 0)
            {
                currentColor.a = endAlpha;
                DrawQuad(currentColor);
            }
            IsFading = false;
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        void OnCustomPostRender()
#else
        void OnPostRender()
#endif
        {
            if (IsFading)
            {
                DrawQuad(currentColor);
            }
        }
        private void DrawQuad(Color aColor)
        {
            fadeMaterial.SetPass(0);
            GL.PushMatrix();
            GL.LoadOrtho();
            GL.Begin(GL.QUADS);
            GL.Color(aColor);
            GL.Vertex3(0, 0, -1);
            GL.Vertex3(0, 1, -1);
            GL.Vertex3(1, 1, -1);
            GL.Vertex3(1, 0, -1);
            GL.End();
            GL.PopMatrix();
        }
    }
}