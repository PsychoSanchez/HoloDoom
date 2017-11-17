﻿//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;

namespace MRDL.Design
{
    public class LineUnity : LineRenderer
    {
        const string DefaultLineShader = "Particles/Alpha Blended";
        const string DefaultLineShaderColor = "_TintColor";

        public Material LineMaterial;

        public bool RoundedEdges = true;
        public bool RoundedCaps = true;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (lineRenderer == null)
                lineRenderer = gameObject.GetComponent<UnityEngine.LineRenderer>();
            if (lineRenderer == null)
                lineRenderer = gameObject.AddComponent<UnityEngine.LineRenderer>();

            if (LineMaterial == null) {
                LineMaterial = new Material(Shader.Find(DefaultLineShader));
                LineMaterial.SetColor(DefaultLineShaderColor, Color.white);
            } 
        }

        private void Update()
        {
            if (!source.enabled)
            {
                lineRenderer.enabled = false;
                return;
            }

            lineRenderer.enabled = true;

            switch (this.StepMode)
            {
                case StepModeEnum.FromSource:
                    lineRenderer.positionCount = source.NumPoints;
                    if (positions == null || positions.Length != source.NumPoints)
                    {
                        positions = new Vector3[source.NumPoints];
                    }
                    for (int i = 0; i < positions.Length; i++)
                    {
                        positions[i] = source.GetPoint(i);
                    }
                    break;

                case StepModeEnum.Interpolated:
                    lineRenderer.positionCount = NumLineSteps;
                    if (positions == null || positions.Length != NumLineSteps)
                    {
                        positions = new Vector3[NumLineSteps];
                    }
                    for (int i = 0; i < positions.Length; i++)
                    {
                        float normalizedDistance = (1f / (NumLineSteps - 1)) * i;
                        positions[i] = source.GetPoint(normalizedDistance);
                    }
                    break;
            }

            // Set line renderer properties
            lineRenderer.loop = source.Loops;
            lineRenderer.numCapVertices = RoundedCaps ? 8 : 0;
            lineRenderer.numCornerVertices = RoundedEdges ? 8 : 0;
            lineRenderer.useWorldSpace = true;
            lineRenderer.startWidth = 1;
            lineRenderer.endWidth = 1;
            lineRenderer.startColor = Color.white;
            lineRenderer.endColor = Color.white;
            lineRenderer.sharedMaterial = LineMaterial;
            lineRenderer.widthCurve = LineWidth;
            lineRenderer.widthMultiplier = WidthMultiplier;
            lineRenderer.colorGradient = LineColor;
            lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lineRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            // Set positions
            lineRenderer.SetPositions(positions);
        }

        [SerializeField]
        private UnityEngine.LineRenderer lineRenderer;
        private Vector3[] positions;
    }
}
