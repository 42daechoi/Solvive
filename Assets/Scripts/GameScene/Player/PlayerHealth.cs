using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviourPun
{
	private HitEffect hitEffect;
	public float maxHealth = 100f;
	public Slider healthSlider;
	public float currentHealth;
		
	private void Start()
	{
		currentHealth = maxHealth;
			
		if (healthSlider == null)
		{ 
			GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
			foreach (GameObject obj in allObjects)
			{
				if (obj.name == "HealthSlider" && obj.GetComponent<Slider>() != null)
				{
					healthSlider = obj.GetComponent<Slider>();
					Debug.Log("이름으로 슬라이더를 찾았습니다.");
					break;
				}
			}
		}
		if (healthSlider != null)
		{
			healthSlider.maxValue = maxHealth;
			healthSlider.value = currentHealth;
		}
	}

	[PunRPC]
	public void TakeDamage(float amount)
	{
		if (!photonView.IsMine)
		{
			hitEffect.OnHit();
			return;
		}
		currentHealth -= amount;
		currentHealth = Mathf.Clamp(currentHealth, 0f, 100f);
		
		
		if (currentHealth <= 0f)
		{
			Die();
		}
		else
		{
            EventManager_Game.Instance.InvokeTakeDamage();
        }
		UpdateHPbar();
	}
				
	public void UpdateHPbar() {
		if (healthSlider != null)
		{
			healthSlider.value = currentHealth;
		}
	}

	private void Die()
	{
		Debug.Log($"{gameObject.name} 사망");
		GameManager.Instance.EliminateOrEscapeCitizen(photonView.ViewID, "Eliminate");
		EventManager_Game.Instance.InvokeDeathState(true);
	}

    public void SetHitEffect(HitEffect effect)
    {
        hitEffect = effect;
    }
}
