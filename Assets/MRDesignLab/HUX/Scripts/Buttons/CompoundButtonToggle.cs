﻿//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Focus;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace HUX.Buttons
{
    /// <summary>
    /// Class that can be used to toggle between to button profiles for any target component inheriting from ProfileButtonBase
    /// </summary>
    [RequireComponent(typeof(CompoundButton))]
    public class CompoundButtonToggle : MonoBehaviour
    {
        public enum ToggleBehaviorEnum
        {
            Manual,     // Toggle manually by changing the State property
            OnTapped,   // Toggle automatically when button is tapped
            OnFocus,    // Toggle automatically when button focus is entered;
        }

        /// <summary>
        /// Toggle behavior
        /// </summary>
        public ToggleBehaviorEnum Behavior = ToggleBehaviorEnum.OnTapped;
        
        /// <summary>
        /// Profile to use when State is TRUE
        /// </summary>
        public ButtonProfile OnProfile;

        /// <summary>
        /// Profile to use when State is FALSE
        /// </summary>
        public ButtonProfile OffProfile;

        /// <summary>
        /// Component to target
        /// Must inherit from ProfileButtonBase
        /// </summary>
        public MonoBehaviour Target;

        public bool State {
            get {
                return state;
            }
            set {
                SetState(value);
            }
        }

        [SerializeField]
        private bool state;

        private void OnEnable() {
            // Force initial state setting
            SetState(state, true);
        }

        public void Tapped () {
            switch (Behavior) {
                default:
                    break;

                case ToggleBehaviorEnum.OnTapped:
                    State = !State;
                    break;
            }
        }

        public void FocusEnter (FocusArgs args) {
            switch (Behavior) {
                default:
                    break;

                case ToggleBehaviorEnum.OnFocus:
                    State = !State;
                    break;
            }
        }

        private void SetState (bool newState, bool force = false) {
            if ((!force || !Application.isPlaying) && state == newState)
                return;

            if (Target == null || OnProfile == null || OffProfile == null)
                return;

            state = newState;

            // Get the profile field of the target component and set it to the on profile
            // Store all icons in iconLookup via reflection
#if USE_WINRT
            FieldInfo fieldInfo = Target.GetType().GetTypeInfo().GetField("Profile");
#else
            FieldInfo fieldInfo = Target.GetType().GetField("Profile");
#endif
            if (fieldInfo == null) {
                Debug.LogError("Target component had no field type profile in CompoundButtonToggle");
                return;
            }

            fieldInfo.SetValue(Target, state ? OnProfile : OffProfile);

            if (Application.isPlaying) {
                // Disable, then re-enable the target
                // This will force the component to update itself
                Target.enabled = false;
                Target.enabled = true;
            }
        }        
    }
}
