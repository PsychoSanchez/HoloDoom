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
    public long HeadUserID { get; private set; }

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
        UIManager.Instance.LogMessage("Waiting for connection...");
        SetCurrentAppState(AppState.WaitingForConection);
        InitSharingManager();
    }

    private void InitSharingManager()
    {
        SharingStage.Instance.SharingManagerConnected += (e, i) =>
        {
            UIManager.Instance.LogMessage("Successfully connected!");
            SetCurrentAppState(AppState.WaitingForAnchor);
        };

        UIManager.Instance.LogMessage("Conencting...");
        SharingStage.Instance.ConnectToServer();
    }

    public void SetHeadUser(long userId)
    {
        HeadUserID = userId;
        // CustomMessages.Instance.
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
                UIManager.Instance.ToggleMode(UIMode.Menu);
                break;
            case AppState.WaitingForAnchor:
                UIManager.Instance.LogMessage("Waiting for anchor to place");
                if (ImportExportAnchorManager.Instance.AnchorEstablished)
                {
                    UIManager.Instance.LogMessage("AnchorEstablished");
                    UIManager.Instance.ToggleMode(UIMode.None);
                    EnableMapping();
                }
                break;
            case AppState.WaitingForStageTransform:
                UIManager.Instance.LogMessage("Waiting for stage transform...");
                UIManager.Instance.ToggleMode(UIMode.None);
                EnableMapping();

                // Hide anchor
                AnchorPlacement.Instance.gameObject.SetActive(false);
                break;
            case AppState.Ready:

                // Show anchor
                AnchorPlacement.Instance.gameObject.SetActive(true);
                UIManager.Instance.LogMessage("Game start");
                UIManager.Instance.ToggleMode(UIMode.Game);
                break;
        }
        needUpdate = false;
    }

    private static void EnableMapping()
    {
        SpatialMappingManager.Instance.gameObject.SetActive(true);
        SpatialMappingManager.Instance.DrawVisualMeshes = true;
        SpatialMappingManager.Instance.StartObserver();
    }
}
