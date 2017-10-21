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
    WaitingForConnection,
    WaitingForAnchor,
    WaitingForStageTransform,
    PickingAvatar,
    Ready,
    WaitingForGameStart
}

// TODO: Add users states
public struct UserState
{
    long id;
    AppState state;
}

public class AppStateManager : Singleton<AppStateManager>
{
    public event EventHandler onAppStateChange;
    public long HeadUserID { get; private set; }
    private bool needUpdate = false;
    private AppState currentAppState;
    int usersReady = 0;

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
        CustomMessages.Instance.MessageHandlers[CustomMessages.GameMessageID.UpdateAppState] += AppStageUpdate;
        SetCurrentAppState(AppState.WaitingForConnection);
        InitSharingManager();
        SharingSessionTracker.Instance.SessionJoined += Instance_SessionJoined;
    }

    private void Instance_SessionJoined(object sender, SharingSessionTracker.SessionJoinedEventArgs e)
    {
    }

    void AppStageUpdate(NetworkInMessage msg)
    {
        var userId = msg.ReadInt64();
        if (userId == CustomMessages.Instance.localUserID)
        {
            return;
        }
        var state = (AppState)msg.ReadInt16();

        if (state == AppState.WaitingForGameStart)
        {
            var usersCount = SharingSessionTracker.Instance.UserIds.Count;
            usersReady++;
        }

        // TODO:

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
            case AppState.WaitingForConnection:
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
            case AppState.WaitingForGameStart:
                UIManager.Instance.LogMessage("Waiting for match begin...");
                if (CustomMessages.Instance.localUserID != HeadUserID)
                {
                    CustomMessages.Instance.SendNewAppState(AppState.WaitingForGameStart);
                    break;
                }
                usersReady++;
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
