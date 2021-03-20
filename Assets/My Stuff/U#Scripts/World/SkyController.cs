
using UdonSharp;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VRC.SDKBase;
using VRC.Udon;

namespace zerithax
{
    [ExecuteInEditMode, RequireComponent(typeof(Light))]
    public class SkyController : UdonSharpBehaviour
    {
        [Header("Sky colors")]
        public bool overrideSkyColors;
        [GradientUsage(true)]
        public Gradient topColor;
        [GradientUsage(true)]
        public Gradient middleColor;
        [GradientUsage(true)]
        public Gradient bottomColor;

        [Header("Sun color")]
        public bool overrideSunColor;
        [GradientUsage(true)]
        public Gradient sunColor;

        [Header("Sun light color")]
        public bool overrideLightColor;
        public Gradient lightColor;

        [Header("Moon light color")]
        public bool overrideMoonlightColor;
        public Gradient moonlightColor;

        [Header("Ambient sky color")]
        public bool overrideAmbientSkyColor;
        [GradientUsage(true)]
        public Gradient ambientSkyColor;

        [Header("Clouds color")]
        public bool overrideCloudsColor;
        [GradientUsage(true)]
        public Gradient cloudsColor;

        [Header("Debug scrub")]
        public bool useSrub = false;
        [Range(0.0f, 1.0f)]
        public float scrub;

        [SerializeField] private Light sun;
        [SerializeField] private Light moon;
        [SerializeField] private Material skyMaterial;

        private void Start()
        {
            sun = gameObject.GetComponent<Light>();
            moon = gameObject.GetComponentsInChildren<Light>()[1];
            skyMaterial = RenderSettings.skybox;
        }

        public void OnValidate()
        {
            if (useSrub)
            {
                UpdateGradients(scrub);
            }
        }

        private void Update()
        {
            if (!useSrub && sun.transform.hasChanged)
            {
                float pos = Vector3.Dot(sun.transform.forward.normalized, Vector3.up) * 0.5f + 0.5f;
                UpdateGradients(pos);
            }
        }

        public void UpdateGradients(float pos)
        {
            if (overrideSkyColors)
            {
                skyMaterial.SetColor("_ColorTop", topColor.Evaluate(pos));
                skyMaterial.SetColor("_ColorMiddle", middleColor.Evaluate(pos));
                skyMaterial.SetColor("_ColorBottom", bottomColor.Evaluate(pos));
            }
            if (overrideSunColor)
            {
                skyMaterial.SetColor("_SunColor", sunColor.Evaluate(pos));
            }
            if (overrideLightColor)
            {
                sun.color = lightColor.Evaluate(pos);
            }
            if (overrideMoonlightColor)
            {
                moon.color = moonlightColor.Evaluate(pos);
            }

            if (overrideAmbientSkyColor)
            {
                if (RenderSettings.ambientMode == UnityEngine.Rendering.AmbientMode.Trilight)
                {
                    RenderSettings.ambientSkyColor = topColor.Evaluate(pos);
                    RenderSettings.ambientEquatorColor = middleColor.Evaluate(pos);
                    RenderSettings.ambientGroundColor = bottomColor.Evaluate(pos);
                }
                else if (RenderSettings.ambientMode == UnityEngine.Rendering.AmbientMode.Flat)
                {
                    RenderSettings.ambientSkyColor = ambientSkyColor.Evaluate(pos);
                }
            }
            if (overrideCloudsColor)
            {
                skyMaterial.SetColor("_CloudsColor", cloudsColor.Evaluate(pos));
            }
        }
    }
}