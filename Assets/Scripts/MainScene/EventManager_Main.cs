using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventManager_Main : MonoBehaviour
{
    public static event Action OnFindGameClicked;
    public static event Action OnCustomGameClicked;
    public static event Action OnCreditsClicked;
    public static event Action OnOptionClicked;
    public static event Action OnQuitClicked;
    public static event Action OnSoloModeClicked;
    public static event Action OnMultiModeClicked;
    public static event Action OnControlPanelButtonClicked;
    public static event Action OnResolutionPanelButtonClicked;
    public static event Action OnAudioPanelButtonClicked;
    public static event Action OnOptionConfirmButtonClicked;
    public static event Action OnGMS_backgroundClicked;
    public static event Action OnHowToPlayClicked;
    public static event Action OnBackClicked;
    
    public Button FindGameButton;
    public Button CustomGameButton;
    public Button selectButton;
    public Button OptionButton;
    public Button ControlPanelButton;
    public Button ResolutionPanelButton;
    public Button AudioPanelButton;
    public Button OptionConfirmButton;
    public Button HowToPlayButton;
    public Button GMS_background;
    public Button Back;

    IEnumerator WaitSecond(Button button)
        {
        Debug.Log("WaitSecond 시작됨");
        yield return new WaitForSeconds(3);
        Debug.Log("3 초 후");
        button.interactable = true;
        }

    public void OnClickFindGame() 
    {
        FindGameButton.interactable = false;   
        StartCoroutine(WaitSecond(FindGameButton));
        OnFindGameClicked?.Invoke();
    }
    public void OnClickCustomGame(){
        CustomGameButton.interactable = false;   
        StartCoroutine(WaitSecond(CustomGameButton));
        OnCustomGameClicked?.Invoke();
    }
    public void OnClickCredits(){
        selectButton.interactable=false;
        StartCoroutine(WaitSecond(selectButton));
        OnCreditsClicked?.Invoke();
                
    }
    public void OnClickOption(){
        OptionButton.interactable=false;
        StartCoroutine(WaitSecond(OptionButton));
        OnOptionClicked?.Invoke();
    }
    public void OnClickControlPanelButton(){
        ControlPanelButton.interactable=false;
        StartCoroutine(WaitSecond(ControlPanelButton));
        OnControlPanelButtonClicked?.Invoke();
    }
    public void OnClickResolutionPanelButton(){
        ResolutionPanelButton.interactable=false;
        StartCoroutine(WaitSecond(ResolutionPanelButton));
        OnResolutionPanelButtonClicked?.Invoke();
    }
    public void OnClickAudioPanelButton(){
        AudioPanelButton.interactable=false;
        StartCoroutine(WaitSecond(AudioPanelButton));
        OnAudioPanelButtonClicked?.Invoke();
    }
    public void OnOptionConfirmButton(){
        OptionConfirmButton.interactable=false;
        StartCoroutine(WaitSecond(OptionConfirmButton));
        OnOptionConfirmButtonClicked?.Invoke();
        Debug.Log("설정 적용");
    }

    public void OnClickQuit(){
        OnQuitClicked?.Invoke();
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    public void OnClickSoloMode() {
        OnSoloModeClicked?.Invoke();
        
    }
    
    public void OnClickMultiMode() {
        OnMultiModeClicked?.Invoke();
    }
    public void OnGMS_background(){
        OnGMS_backgroundClicked?.Invoke();
    }

    public void OnClickHowToPlayButton()
    {
        OnHowToPlayClicked?.Invoke();
    }

    public void OnClickBackButton()
    {
        OnBackClicked?.Invoke();
    }
}
