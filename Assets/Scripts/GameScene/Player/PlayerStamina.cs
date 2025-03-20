using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaRecoveryRate = 5f;
    [SerializeField] private float staminaDrainRate = 8f;
    [SerializeField] private float currentStamina;
    [SerializeField] private Slider staminaBar;
    [SerializeField] private PlayerController playerController;

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
        if (isSprint && currentStamina > 0)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
        }
        if (!isSprint && currentStamina < maxStamina)
        {
            currentStamina += staminaRecoveryRate * Time.deltaTime;
        }
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        // 1번 방법
        // if (currentStamina < 2) PlayerController에서 SprintState로 변경 불가하게
        // else PlayerController에서 SprintState로 변경 가능하게

        // 2번 방법
        // 1번 방법보다 더 좋을 것 같은 방법은 위 조건에서 PlayerController로 보내기
        // 즉 InputManager -> PlayerController, InputManager -> PlayerStamina가 아니라
        // InputManager -> PlayerStamina -> PlayerController로 절차형 이벤트
        if (staminaBar) staminaBar.value = currentStamina;
    }

}
