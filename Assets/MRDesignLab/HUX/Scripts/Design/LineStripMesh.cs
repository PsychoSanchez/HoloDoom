﻿//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System.Collections.Generic;
using UnityEngine;

namespace MRDL.Design
{
    public class LineStripMesh : LineRenderer
    {
        public Material LineMaterial;

        public Vector3 Forward;

        protected override void OnEnable()
        {
            base.OnEnable();

            lineMatInstance = new Material(LineMaterial);

            // Create a mesh
            if (stripMesh == null)
            {
                stripMesh = new Mesh();
            }
            if (stripMeshRenderer == null)
                stripMeshRenderer = gameObject.GetComponent<MeshRenderer>();

            if (stripMeshRenderer == null)
                stripMeshRenderer = gameObject.AddComponent<MeshRenderer>();

            stripMeshRenderer.sharedMaterial = lineMatInstance;
            stripMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            stripMeshRenderer.receiveShadows = false;
            stripMeshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;

            MeshFilter stripMeshFilter = gameObject.GetComponent<MeshFilter>();
            if (stripMeshFilter == null)
                stripMeshFilter = gameObject.AddComponent<MeshFilter>();

            stripMeshFilter.sharedMesh = stripMesh;
        }

        private void OnDisable()
        {
            if (lineMatInstance != null)
                GameObject.Destroy(lineMatInstance);
        }

        public void Update()
        {
            if (!source.enabled)
            {
                stripMeshRenderer.enabled = false;
                return;
            }

            stripMeshRenderer.enabled = true;
            positions.Clear();
            colors.Clear();
            widths.Clear();

            float normalizedLengthPerStep = 1f / NumLineSteps;
            for (int i = 0; i <= NumLineSteps; i++)
            {
                float normalizedDistance = (1f / (NumLineSteps - 1)) * i;
                positions.Add(source.GetPoint(normalizedDistance));
                colors.Add(GetColor(normalizedDistance));
                widths.Add(GetWidth(normalizedDistance));
            }

            GenerateStripMesh(positions, colors, widths, uvOffset, Forward, stripMesh);
        }

        public static void GenerateStripMesh(List<Vector3> positionList, List<Color> colorList, List<float> thicknessList, float uvOffsetLocal, Vector3 forward, Mesh mesh)
        {
            // TODO store these as local variables to reduce allocations
            Vector3[] vertices = new Vector3[positionList.Count * 2];
            Color[] colors = new Color[colorList.Count * 2];
            Vector2[] uvs = new Vector2[positionList.Count * 2];
            Vector3 right = Vector3.Cross(forward, Vector3.up).normalized;

            for (int x = 0; x < positionList.Count; x++)
            {
                float thickness = thicknessList[x / 2] / 2;
                vertices[2 * x] = positionList[x] - right * thickness;
                vertices[2 * x + 1] = positionList[x] + right * thickness;
                colors[2 * x] = colorList[x];
                colors[2 * x + 1] = colorList[x];

                float uv = uvOffsetLocal;
                if (x == positionList.Count - 1 && x > 1)
                {
                    float dist_last = (positionList[x - 2] - positionList[x - 1]).magnitude;
                    float dist_cur = (positionList[x] - positionList[x - 1]).magnitude;
                    uv += 1 - dist_cur / dist_last;
                }

                uvs[2 * x] = new Vector2(0, x - uv);
                uvs[2 * x + 1] = new Vector2(1, x - uv);
            }

            int numTriangles = ((positionList.Count * 2 - 2) * 3);
            int[] triangles = new int[numTriangles];
            int j = 0;
            for (int i = 0; i < positionList.Count * 2 - 3; i += 2, j++)
            {
                triangles[i * 3] = j * 2;
                triangles[i * 3 + 1] = j * 2 + 1;
                triangles[i * 3 + 2] = j * 2 + 2;

                triangles[i * 3 + 3] = j * 2 + 1;
                triangles[i * 3 + 4] = j * 2 + 3;
                triangles[i * 3 + 5] = j * 2 + 2;
            }

            mesh.Clear();
            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.triangles = triangles;
            mesh.colors = colors;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
        }

        [SerializeField]
        private float uvOffset = 0f;

        private Mesh stripMesh;
        private MeshRenderer stripMeshRenderer;
        private Material lineMatInstance;
        private List<Vector3> positions = new List<Vector3>();
        private List<Color> colors = new List<Color>();
        private List<float> widths = new List<float>();
    }
}