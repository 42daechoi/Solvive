using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventManager_Custom : MonoBehaviour
{
    public static event Action OnCreateButtonClicked;

    public static event Action OnJoinButonClicked;

    public static event Action OnSearchButtonClicked;

    public static event Action OnPreviousButtonClicked;
    public static event Action OnIncreaseButtonClicked;
    public static event Action OnDecreaseButtonClicked;
    public static event Action OnCancleButtonClicked;
    public static event Action OnCreateComfirmButtonClicked;
    public static event Action OnJoinWithCodeButtonClicked;
    public static event Action OnJoinWithCodeEscButtonClicked;
    public static event Action OnCodeJoinButtonClicked;

    public void OnClickCreateButton() {
        OnCreateButtonClicked?.Invoke();   
    }

    public void OnClickJoinButton(){
        OnJoinButonClicked?.Invoke();
    }

    public void OnClickSearchButton(){
        OnSearchButtonClicked?.Invoke();
    }
    public void OnPreviousButton(){
        OnPreviousButtonClicked?.Invoke();
    }
    public void OnClickIncreaseButton(){
        OnIncreaseButtonClicked?.Invoke();
    }
    public void OnClickDecreaseButton(){
        OnDecreaseButtonClicked?.Invoke();
    }
    public void OnClickCancleButton(){
        OnCancleButtonClicked?.Invoke();
    }
    public void OnCreateComfirmButton(){
        OnCreateComfirmButtonClicked?.Invoke();
    }

    public void OnClickJoinWithCodeButton()
    {
        OnJoinWithCodeButtonClicked?.Invoke();
    }

    public void OnClickJoinWithCodeEscButton()
    {
        OnJoinWithCodeEscButtonClicked?.Invoke();
    }

    public void OnClickCodeJoinButton()
    {
        OnCodeJoinButtonClicked?.Invoke();
    }
}
