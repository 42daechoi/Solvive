using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerStamina : MonoBehaviour
{
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaRecoveryRate = 5f;
    [SerializeField] private float staminaDrainRate = 8f;
    [SerializeField] private float staminaRecoveryDelay = 1f;
    [SerializeField] private float currentStamina;
    [SerializeField] private Slider staminaBar;

    private PlayerController playerController;
    private PlayerSound playerSound;
    private Coroutine recoveryCoroutine;

    private void Start()
    {
        if (EventManager_Game.Instance != null)
        {
            EventManager_Game.Instance.OnPlayerSprint += HandleStamina;
        }
        if (staminaBar == null)
        {
            GameObject obj = GameObject.Find("StaminaSlider");
            if (obj != null) staminaBar = obj.GetComponent<Slider>();
        }
        playerController = GetComponent<PlayerController>();
        playerSound = GetComponent<PlayerSound>();
        currentStamina = maxStamina;
        if (staminaBar) staminaBar.maxValue = maxStamina;
    }

    private void OnDisable()
    {
        if (EventManager_Game.Instance != null)
        {
            EventManager_Game.Instance.OnPlayerSprint -= HandleStamina;
        }
    }

    private void HandleStamina(bool isSprint)
    {
        if (isSprint && PlayerController.Instance.GetCurrentState() is not CrouchState)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            if (currentStamina <= 0) playerSound.PlayPantingSound();

            if (recoveryCoroutine != null)
            {
                StopCoroutine(recoveryCoroutine);
                recoveryCoroutine = null;
            }
        }
        else if (recoveryCoroutine == null)
        {
            recoveryCoroutine = StartCoroutine(WaitForRecoverStamina());
        }

        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        if (staminaBar) staminaBar.value = currentStamina;

        EventManager_Game.Instance.InvokeOnPlayerSprintWithStamina(isSprint && currentStamina > 0);
    }

    private IEnumerator WaitForRecoverStamina()
    {
        yield return new WaitForSeconds(staminaRecoveryDelay);

        while (currentStamina < maxStamina)
        {
            currentStamina += staminaRecoveryRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            if (staminaBar) staminaBar.value = currentStamina;
            yield return null;
        }

        recoveryCoroutine = null;
    }

    public bool TryToUseStamina(float requiredStamina)
    {
        if (currentStamina < requiredStamina) return false;

        if (recoveryCoroutine != null)
        {
            StopCoroutine(recoveryCoroutine);
            recoveryCoroutine = null;
        }
        currentStamina -= requiredStamina;
        return true;
    }
}
