using System;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Sharing;
using HUX.Interaction;
using HUX.Receivers;
using UnityEngine;

public class MainMenuController : InteractionReceiver
{
    const string SINGLE_PLAYER_BUTTON_NAME = "StartSinglePlayerButton";
    const string BACK_TO_MAIN_MENU_BUTTON_NAME = "Anchor";
    const string CONNECT_SERVER_BUTTON_NAME = "ConnectServerButton";
    const string SETTINGS_BUTTON_NAME = "SettingsButton";
    const string DIFFICULTY_BUTTON_NAME = "DifficultyButton";
    const string BACK_BUTTON_NAME = "BackButton";
    const string EASY_BUTTON_NAME = "EasyButton";
    const string NORMAL_BUTTON_NAME = "NormalButton";
    const string AUDIO_BUTTON_NAME = "AudioButton";
    const string EXIT_BUTTON_NAME = "ExitButton";
    public enum MainMenuState
    {
        Closed = 0,
        FirstScreen,
        ServerBrowser,
        Setttings,
        DifficultyMenu
    }
    public int ReleaseMenuButtonAfter = 3;
    public GameObject MainMenuRef;
    DateTime startTime;
    MainMenuState currentState = MainMenuState.FirstScreen;
    [Serializable]
    public struct MenuButtons
    {
        public MainMenuState State;
        public GameObject Button;
    }
    public MenuButtons[] Buttons;
    void Start()
    {
        foreach (var menuButton in Buttons)
        {
            if (menuButton.Button != null)
            {
                if (Interactibles.Contains(menuButton.Button))
                {
                    continue;
                }
                Interactibles.Add(menuButton.Button);
            }
        }
    }

    private void SetMainMenuState(MainMenuState state)
    {
        currentState = state;
        HideButtons();
        for (int i = 0; i < Buttons.Length; i++)
        {
            if (Buttons[i].Button == null)
            {
                continue;
            }

            if (Buttons[i].State == currentState)
            {
                Buttons[i].Button.SetActive(true);
            }
        }
        switch (state)
        {
            case MainMenuState.Closed:
                // MainMenuRef.SetActive(false);
                break;
            case MainMenuState.FirstScreen:
                break;
            case MainMenuState.ServerBrowser:
                break;
            case MainMenuState.Setttings:
                break;
            default:
                break;
        }
    }

    private void HideButtons()
    {
        for (int i = 0; i < Buttons.Length; i++)
        {
            if (Buttons[i].Button == null)
            {
                continue;
            }
            Buttons[i].Button.SetActive(false);
        }
    }
    protected override void OnTapped(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
    {
        if (obj == null)
        {
            return;
        }
        switch (obj.name)
        {
            case SINGLE_PLAYER_BUTTON_NAME:
                GameManager.Instance.StartSinglePlayerGame();
                SetMainMenuState(MainMenuState.Closed);
                startTime = DateTime.Now;
                break;
            case CONNECT_SERVER_BUTTON_NAME:
                SharingStage.Instance.ConnectToServer(SharingStage.Instance.ServerAddress, SharingStage.Instance.ServerPort);
                break;
            case BACK_TO_MAIN_MENU_BUTTON_NAME:
                // Activate button only after 3 seconds of game
                if ((DateTime.Now - startTime).Seconds > ReleaseMenuButtonAfter)
                {
                    SetMainMenuState(MainMenuState.FirstScreen);
                    GameManager.Instance.StopGame();
                }
                break;
            case SETTINGS_BUTTON_NAME:
                SetMainMenuState(MainMenuState.Setttings);
                break;
            case DIFFICULTY_BUTTON_NAME:
                SetMainMenuState(MainMenuState.DifficultyMenu);
                break;
            case EASY_BUTTON_NAME:
                UIManager.Instance.LogMessage("Difficulty set to easy");
                SetMainMenuState(MainMenuState.Setttings);
                break;
            case NORMAL_BUTTON_NAME:
                UIManager.Instance.LogMessage("Difficulty set to normal");
                SetMainMenuState(MainMenuState.Setttings);
                break;
            case EXIT_BUTTON_NAME:
                Application.Quit();
                break;
            case BACK_BUTTON_NAME:
                if (currentState == MainMenuState.Setttings)
                {
                    SetMainMenuState(MainMenuState.FirstScreen);
                }
                else if (currentState == MainMenuState.DifficultyMenu)
                {
                    SetMainMenuState(MainMenuState.Setttings);
                }
                break;
            default:
                break;
        }
    }
}
