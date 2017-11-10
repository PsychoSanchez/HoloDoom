using System;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Sharing;
using HoloToolkit.Unity;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;

public enum AppState
{
    // Game started, not connected
    Starting = 0,
    // Connecting
    WaitingForConnection,
    // Picking user avatar
    PickingAvatar,
    // Wainting for user to place anchor
    WaitingForAnchor,
    // Scanning 
    Scanning,
    // Syncing game state and room transform
    Syncing,
    // Lobby
    Ready,
    // Match
    Playing
}

public class AppStateManager : Singleton<AppStateManager>
{
    public event EventHandler onAppStateChange;
    public bool WaitForPlayers = true;
    public long HeadUserID { get; private set; }
    private AppState currentAppState = AppState.Starting;
    Dictionary<long, AppState> connectedUsers = new Dictionary<long, AppState>();
    int nUsersJoined = 0;
    int usersReady = 0;

    // Use this for initialization
    void Start()
    {
        UIManager.Instance.LogMessage("Waiting for connection...");
        SetAppState(AppState.WaitingForConnection);
        InitSharingManager();
        SharingSessionTracker.Instance.SessionJoined += Instance_SessionJoined;
        SharingSessionTracker.Instance.SessionLeft += Instance_SessionLeft;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SetAppState(AppState.Playing);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            SetAppState(AppState.Ready);
        }
    }

    public AppState GetAppState()
    {
        return currentAppState;
    }

    private void SetUserState(long userId, AppState state)
    {
        if (!connectedUsers.ContainsKey(userId))
        {
            connectedUsers.Add(userId, AppState.Starting);
        }
        connectedUsers[userId] = state;
    }

    // Local user App State update
    public void SetAppState(AppState value)
    {
        if (value == currentAppState)
        {
            return;
        }
        if (CustomMessages.Instance.localUserID != 0)
        {
            SetUserState(CustomMessages.Instance.localUserID, value);
        }
        UpdateAppState(value);
        var e = onAppStateChange;
        if (e != null)
        {
            e.Invoke(this, null);
        }
    }

    // Remote user app state updated
    void PlayerAppStateUpdate(NetworkInMessage msg)
    {
        var userId = msg.ReadInt64();
        if (userId == CustomMessages.Instance.localUserID)
        {
            return;
        }
        var state = (AppState)msg.ReadInt16();
        SetUserState(userId, state);

        if (CustomMessages.Instance.localUserID == HeadUserID)
        {
            CheckIfUsersReady();
        }
    }

    public void StartGame()
    {
        SetAppState(AppState.Playing);
        CustomMessages.Instance.SetMatchPlaying(true);
    }

    public void PauseGame()
    {
        SetAppState(AppState.Ready);
        CustomMessages.Instance.SetMatchPlaying(false);
    }

    private void TrySetMatchState(NetworkInMessage msg)
    {
        var userId = msg.ReadInt64();
        if (userId == CustomMessages.Instance.localUserID)
        {
            return;
        }

        bool bPlay = (msg.ReadByte() == 1) ? true : false;
        var sourceAppState = (bPlay) ? AppState.Ready : AppState.Playing;
        var targetAppState = (bPlay) ? AppState.Playing : AppState.Ready;
        UpdateMatchState(targetAppState, sourceAppState);
    }

    private void UpdateMatchState(AppState target, AppState source)
    {
        if (currentAppState == source || currentAppState == target)
        {
            SetAppState(target);
            return;
        }

        CustomMessages.Instance.SendAppState(currentAppState);
    }

    private void UpdateAppState(AppState value)
    {
        // Set current app state
        currentAppState = value;

        // Do action
        switch (currentAppState)
        {
            case AppState.WaitingForConnection:
                // "Connecting" sign
                UIManager.Instance.SetMode(UIMode.Menu);
                break;
            case AppState.WaitingForAnchor:
                CustomMessages.Instance.MessageHandlers[CustomMessages.GameMessageID.UpdateAppState] += PlayerAppStateUpdate;
                CustomMessages.Instance.MessageHandlers[CustomMessages.GameMessageID.MatchPlaying] += TrySetMatchState;
                // Every user has "Anchor" in hands and searching place for place
                // After anchor placed go to scanning phase
                UIManager.Instance.LogMessage("Waiting for anchor to place");
                if (ImportExportAnchorManager.Instance.AnchorEstablished)
                {
                    UIManager.Instance.LogMessage("AnchorEstablished");
                    UIManager.Instance.SetMode(UIMode.None);
                    EnableMapping();
                }
                break;
            case AppState.Scanning:
                // Start scanning at all users
                // After upload start head user should go to ready state
                // Secondary users will see that server contains room and will go to syncing state
                UIManager.Instance.LogMessage("Waiting for stage transform...");
                UIManager.Instance.SetMode(UIMode.Scanning);
                EnableMapping();
                UIManager.Instance.LogMessage("Scanning space around you...");
                UIManager.Instance.LogMessage("Game will start, when anchor will scan enough space for game");

                // FIXME: Hide anchor (Questionable)
                // AnchorPlacement.Instance.gameObject.SetActive(false);
                break;
            case AppState.Syncing:
                UIManager.Instance.SetMode(UIMode.Syncing);
                // TODO: Stop All enemies, show "Syncing sign"
                // Download room, download enemies, download player states, sync space
                // After all this steps go to ready
                break;
            case AppState.Ready:
                // Anchor uploaded or downloaded, waiting for players to load 
                // Stop All enemies, wait for all players to get ready
                // Show "waiting for users to sync" sign
                // Send information to head user, that we are ready
                // After all users are ready, head user will start game (AppState.Playing)
                UIManager.Instance.LogMessage("Waiting for users to be ready...");
                UIManager.Instance.SetMode(UIMode.WaitingForPlayers);
                // DisableMapping();
                if (CustomMessages.Instance.localUserID == HeadUserID)
                {
                    CheckIfUsersReady();
                    return;
                }
                break;
            case AppState.Playing:
                // Start spawn enemies, start movement
                // Show anchor
                AnchorPlacement.Instance.gameObject.SetActive(true);
                UIManager.Instance.LogMessage("Game start");
                UIManager.Instance.SetMode(UIMode.Game);
                break;
            default:
                break;
        }

        if (currentAppState != AppState.WaitingForConnection)
        {
            CustomMessages.Instance.SendAppState(value);
        }
    }

    private void Instance_SessionJoined(object sender, SharingSessionTracker.SessionJoinedEventArgs e)
    {
        if (CustomMessages.Instance.localUserID == e.joiningUser.GetID())
        {
            return;
        }

        connectedUsers.Add(e.joiningUser.GetID(), AppState.Starting);
        CheckIfUsersReady();
    }

    private void Instance_SessionLeft(object sender, SharingSessionTracker.SessionLeftEventArgs e)
    {
        if (CustomMessages.Instance.localUserID == e.exitingUserId)
        {
            // TODO: If headuser left the game - reset to waiting for anchor
            // Clear all game data, remove all game objects, set game mode to placing anchor
            return;
        }

        var userIndex = connectedUsers.ContainsKey(e.exitingUserId);
        if (userIndex)
        {
            connectedUsers.Remove(e.exitingUserId);
        }
        CheckIfUsersReady();
    }

    private void CheckIfUsersReady()
    {
        nUsersJoined = SharingSessionTracker.Instance.UserIds.Count;
        bool bUsersReady = true;
        foreach (var id in connectedUsers.Keys)
        {
            if (connectedUsers[id] != AppState.Ready && connectedUsers[id] != AppState.Playing)
            {
                bUsersReady = false;
                break;
            }
        }
        // If all users ready
        if (bUsersReady)
        {
            // If we are  2 or more players
            if (WaitForPlayers && connectedUsers.Count > 1)
            {
                // If someone with syncing status left or updated his status to ready
                StartGame();
            }
            else if (!WaitForPlayers)
            {
                // If we are not waiting for players, start game
                StartGame();
            }
        }
        else if (currentAppState == AppState.Playing)
        {
            // Else user connected and started syncing
            // Pause game for all
            PauseGame();
        }
    }


    private void InitSharingManager()
    {
        SharingStage.Instance.SharingManagerConnected += (e, i) =>
        {
            UIManager.Instance.LogMessage("Successfully connected!");
            SetAppState(AppState.WaitingForAnchor);
        };

        UIManager.Instance.LogMessage("Conencting...");
        SharingStage.Instance.ConnectToServer();
    }

    /// <summary>
    /// Local user set anchor and becomes head user
    /// </summary>
    /// <param name="userId"></param>
    public void BecomeHeadUser(long userId)
    {
        // Set HeadUserID
        HeadUserID = userId;

        // Init users
        foreach (var id in SharingSessionTracker.Instance.UserIds)
        {
            // if it's our id return
            if (id == CustomMessages.Instance.localUserID)
            {
                return;
            }

            if (connectedUsers.ContainsKey(id))
            {
                continue;
            }
            // add connected users to list
            connectedUsers.Add(id, AppState.WaitingForAnchor);
        }

        // CustomMessages.Instance.
    }

    private static void EnableMapping()
    {
        SpatialMappingManager.Instance.gameObject.SetActive(true);
        SpatialMappingManager.Instance.DrawVisualMeshes = true;
        SpatialMappingManager.Instance.StartObserver();
    }

    private static void DisableMapping()
    {
        SpatialMappingManager.Instance.StopObserver();
        SpatialMappingManager.Instance.gameObject.SetActive(false);
    }
}
