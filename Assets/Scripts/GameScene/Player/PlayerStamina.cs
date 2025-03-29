using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaRecoveryRate = 5f;
    [SerializeField] private float staminaDrainRate = 8f;
    [SerializeField] private float currentStamina;
    [SerializeField] private Slider staminaBar;
    private PlayerController playerController;
    private PlayerSound playerSound;

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
        if (currentStamina <= 0)
        {
            playerSound.PlayPantingSound();
        }
        if (isSprint && currentStamina > 0)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
        }
        if (!isSprint && currentStamina < maxStamina)
        {
            currentStamina += staminaRecoveryRate * Time.deltaTime;
        }
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        EventManager_Game.Instance.InvokeOnPlayerSprintWithStamina(IsSprintEnabled(isSprint));
        if (staminaBar) staminaBar.value = currentStamina;
    }

    private bool IsSprintEnabled(bool isSprint)
    {
        if (isSprint && currentStamina > 0)
        {
            return true;
        }
        return false;
    }

    public bool TryToUseStamina(float requiredStamina)
    {
        if (currentStamina < requiredStamina) return false;
        else
        {
            currentStamina -= requiredStamina;
            return true;
        }
    }
}
