﻿//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;

namespace MRDL.ToolTips
{
    /// <summary>
    /// Renders a background mesh for a tool tip using a mesh renderer
    /// If the mesh has an offset anchor point you will get odd results
    /// </summary>
    public class ToolTipBackgroundMesh : ToolTipBackground
    {
        /// <summary>
        /// Transform that scale and offset will be applied to.
        /// </summary>
        public Transform BackgroundTransform;

        /// <summary>
        /// Mesh renderer button for mesh background.
        /// </summary>
        public MeshRenderer BackgroundRenderer;

        /// <summary>
        /// The z depth of the mesh background
        /// </summary>
        public float Depth = 1f;

        protected override void ScaleToFitContent ()
        {
            if (BackgroundRenderer == null)
                return;

            // Get the local size of the content - this is the scale of the text under the content parent
            Vector3 localContentSize = toolTip.LocalContentSize;
            Vector3 localContentOffset = toolTip.LocalContentOffset;

            // Get the size of the mesh and use this to adjust the local content size on the x / y axis
            // This will accomodate meshes that aren't built to 1,1 scale
            Bounds meshBounds = BackgroundRenderer.GetComponent<MeshFilter>().sharedMesh.bounds;
            localContentSize.x /= meshBounds.size.x;
            localContentSize.y /= meshBounds.size.y;
            localContentSize.z = Depth;

            localContentBounds = new Bounds(localContentOffset, localContentSize);

            // Don't use the mesh bounds for local content since an offset center may be used for design effect
            BackgroundTransform.localScale = localContentSize;
            BackgroundTransform.localPosition = localContentOffset;
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
                return;

            if (toolTip == null)
                toolTip = gameObject.GetComponent<ToolTip>();

            if (toolTip == null)
                return;

            ScaleToFitContent();
        }

        protected Bounds localContentBounds;
    }
}
