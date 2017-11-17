﻿//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX;
using HUX.Focus;
using System.Collections;
using UnityEngine;

namespace MRDL.ToolTips
{
    /// <summary>
    /// Add to any InteractibleObject to spawn ToolTips on tap or on focus, according to preference
    /// Applies its follow settings to the spawned ToolTip's ToolTipConnector component
    /// </summary>
    public class ToolTipSpawner : MonoBehaviour
    {
        public enum VanishBehaviorEnum
        {
            VanishOnFocusExit,
            VanishOnTap,
        }

        public enum AppearBehaviorEnum
        {
            AppearOnFocusEnter,
            AppearOnTap,
        }

        public enum RemainBehaviorEnum
        {
            Indefinite,
            Timeout,
        }

        [Range(0f, 5f)]
        public float AppearDelay = 1.25f;

        [Range(0f, 5f)]
        public float VanishDelay = 2f;

        [Range (0f, 1f)]
        public float PivotDistance = 0.25f;

        public AppearBehaviorEnum AppearBehavior = AppearBehaviorEnum.AppearOnFocusEnter;
        public VanishBehaviorEnum VanishBehavior = VanishBehaviorEnum.VanishOnFocusExit;
        public RemainBehaviorEnum RemainBehavior = RemainBehaviorEnum.Indefinite;

        public ToolTipConnector.FollowTypeEnum FollowType = ToolTipConnector.FollowTypeEnum.AnchorOnly;
        public ToolTipConnector.PivotModeEnum PivotMode = ToolTipConnector.PivotModeEnum.Manual;
        public ToolTipConnector.PivotDirectionEnum PivotDirection = ToolTipConnector.PivotDirectionEnum.North;
        public ToolTipConnector.OrientTypeEnum PivotDirectionOrient = ToolTipConnector.OrientTypeEnum.OrientToObject;
        public Vector3 ManualPivotDirection = Vector3.up;
        public Vector3 ManualPivotLocalPosition = Vector3.up;

        public string ToolTipText = "New Tooltip";

        public GameObject ToolTipPrefab;

        public Transform Anchor;

        public void Tapped()
        {
            tappedTime = Time.unscaledTime;
            if (toolTip == null || !toolTip.gameObject.activeSelf)
            {
                switch (AppearBehavior)
                {
                    case AppearBehaviorEnum.AppearOnTap:
                        ShowToolTip();
                        return;

                    default:
                        break;
                }
            }
        }

        public void FocusEnter()
        {
            focusEnterTime = Time.unscaledTime;
            hasFocus = true;
            if (toolTip == null || !toolTip.gameObject.activeSelf)
            {
                switch (AppearBehavior)
                {
                    case AppearBehaviorEnum.AppearOnFocusEnter:
                        ShowToolTip();
                        break;

                    default:
                        break;
                }
            }
        }

        public void FocusExit()
        {
            focusExitTime = Time.unscaledTime;
            hasFocus = false;
        }

        private void ShowToolTip()
        {
            StartCoroutine(UpdateTooltip(focusEnterTime, tappedTime));
        }

        private IEnumerator UpdateTooltip(float focusEnterTimeOnStart, float tappedTimeOnStart)
        {
            if (toolTip == null)
            {
                GameObject toolTipGo = GameObject.Instantiate(ToolTipPrefab) as GameObject;
                toolTip = toolTipGo.GetComponent<ToolTip>();                
                toolTip.transform.position = transform.position;
                toolTip.transform.parent = transform;
                toolTip.gameObject.SetActive(false);
            }

            switch (AppearBehavior)
            {
                case AppearBehaviorEnum.AppearOnFocusEnter:
                    // Wait for the appear delay
                    yield return new WaitForSeconds(AppearDelay);
                    // If we don't have focus any more, get out of here
                    if (!hasFocus)
                    {
                        yield break;
                    }
                    break;
            }
            
            toolTip.gameObject.SetActive(true);
            toolTip.ToolTipText = ToolTipText;
            ToolTipConnector connector = toolTip.GetComponent<ToolTipConnector>();
            connector.Target = (Anchor != null) ? Anchor.gameObject : gameObject;
            connector.PivotDirection = PivotDirection;
            connector.PivotDirectionOrient = PivotDirectionOrient;
            connector.ManualPivotLocalPosition = ManualPivotLocalPosition;
            connector.ManualPivotDirection = ManualPivotDirection;
            connector.FollowType = FollowType;
            connector.PivotMode = PivotMode;
            if (PivotMode == ToolTipConnector.PivotModeEnum.Manual)
                toolTip.PivotPosition = transform.TransformPoint(ManualPivotLocalPosition);

            while (toolTip.gameObject.activeSelf)
            {
                //check whether we're suppose to disappear
                switch (VanishBehavior)
                {
                    case VanishBehaviorEnum.VanishOnFocusExit:
                    default:
                        if (!hasFocus)
                        {
                            if (Time.time - focusExitTime > VanishDelay)
                            {
                                toolTip.gameObject.SetActive(false);
                            }
                        }
                        break;

                    case VanishBehaviorEnum.VanishOnTap:
                        if (tappedTime != tappedTimeOnStart)
                        {
                            toolTip.gameObject.SetActive(false);
                        }
                        break;
                }
                yield return null;
            }
            yield break;
        }

        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
                return;

            if (gameObject == UnityEditor.Selection.activeGameObject)
            {
                Gizmos.color = Color.cyan;
                Transform relativeTo = null;
                switch (PivotDirectionOrient) {
                    case ToolTipConnector.OrientTypeEnum.OrientToCamera:
                        relativeTo = Camera.main.transform;//Veil.Instance.HeadTransform;
                        break;

                    case ToolTipConnector.OrientTypeEnum.OrientToObject:
                        relativeTo = (Anchor != null) ? Anchor.transform : transform;
                        break;
                }
                if (PivotMode == ToolTipConnector.PivotModeEnum.Automatic) {
                    Vector3 targetPosition = (Anchor != null) ? Anchor.transform.position : transform.position;
                    Vector3 toolTipPosition = targetPosition + ToolTipConnector.GetDirectionFromPivotDirection(
                                    PivotDirection,
                                    ManualPivotDirection,
                                    relativeTo) * PivotDistance;
                    Gizmos.DrawLine(targetPosition, toolTipPosition);
                    Gizmos.DrawWireCube(toolTipPosition, Vector3.one * 0.05f);
                } else {
                    Vector3 targetPosition = (Anchor != null) ? Anchor.transform.position : transform.position;
                    Vector3 toolTipPosition = transform.TransformPoint (ManualPivotLocalPosition);
                    Gizmos.DrawLine(targetPosition, toolTipPosition);
                    Gizmos.DrawWireCube(toolTipPosition, Vector3.one * 0.05f);
                }
            }
        }
        #endif

        private float focusEnterTime = 0f;
        private float focusExitTime = 0f;
        private float tappedTime = 0f;
        private float targetDisappearTime = Mathf.Infinity;
        private bool hasFocus;
        private ToolTip toolTip;
    }
}
