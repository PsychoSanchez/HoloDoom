using System;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Sharing;
using HoloToolkit.Unity;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;

public enum AppState
{
    Starting = 0,
    WaitingForConection,
    WaitingForAnchor,
    WaitingForStageTransform,
    PickingAvatar,
    Ready
}

public class AppStateManager : Singleton<AppStateManager>
{
    private bool needUpdate = false;
    private AppState currentAppState;
    public event EventHandler onAppStateChange;

    public AppState GetCurrentAppState()
    {
        return currentAppState;
    }

    public void SetCurrentAppState(AppState value)
    {
        if (value == currentAppState)
        {
            return;
        }
        currentAppState = value;
        needUpdate = true;

        var e = onAppStateChange;
        if (e != null)
        {
            e.Invoke(this, null);
        }
    }

    // Use this for initialization
    void Start()
    {
        UIManger.Instance.LogMessage("Waiting for connection...");
        SetCurrentAppState(AppState.WaitingForConection);

        // SpatialMappingManager.Instance.StopObserver();
        // SpatialMappingManager.Instance.gameObject.SetActive(false);
        InitSharingManager();
    }

    private void InitSharingManager()
    {
        SharingStage.Instance.SharingManagerConnected += (e, i) =>
        {
            UIManger.Instance.LogMessage("Successfully connected!");
            SetCurrentAppState(AppState.WaitingForAnchor);
        };

        UIManger.Instance.LogMessage("Conencting...");
        SharingStage.Instance.ConnectToServer();
    }

    // Update is called once per frame
    void Update()
    {
        if (!needUpdate)
        {
            return;
        }

        switch (currentAppState)
        {
            case AppState.WaitingForConection:
                UIManger.Instance.ToggleMode(UIMode.Menu);
                break;
            case AppState.WaitingForAnchor:
                UIManger.Instance.LogMessage("Waiting for anchor to place");
                // SpatialMappingManager.Instance.gameObject.SetActive(true);
                // SpatialMappingManager.Instance.StartObserver();
                if (ImportExportAnchorManager.Instance.AnchorEstablished)
                {
                    UIManger.Instance.LogMessage("AnchorEstablished");
                    // SetCurrentAppState()
                    UIManger.Instance.ToggleMode(UIMode.None);
                    SpatialMappingManager.Instance.gameObject.SetActive(true);
                    SpatialMappingManager.Instance.DrawVisualMeshes = true;
                    SpatialMappingManager.Instance.StartObserver();
                    // currentAppState = AppState.WaitingForStageTransform;
                    // GestureManager.Instance.OverrideFocusedObject = ImportExportAnchorManager.Instance.gameObject;
                }
                break;
            case AppState.WaitingForStageTransform:
                UIManger.Instance.LogMessage("Waiting for stage transform...");
                // Now if we have the stage transform we are ready to go.
                // if (HologramPlacement.Instance.GotTransform)
                // {
                //     CurrentAppState = AppState.Ready;
                //     GestureManager.Instance.OverrideFocusedObject = null;
                // }
                break;
            case AppState.Ready:
                UIManger.Instance.LogMessage("AnchorEstablished");

                UIManger.Instance.ToggleMode(UIMode.Game);
                break;
        }
        needUpdate = false;
    }
}
