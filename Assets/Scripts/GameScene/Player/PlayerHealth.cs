using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviourPun
    {
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
                    if (obj.name == "Slider" && obj.GetComponent<Slider>() != null)
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
            if (!photonView.IsMine) return;
            currentHealth -= amount;
            currentHealth = Mathf.Clamp(currentHealth, 0f, 100f);
            
            //photonView.RPC("UpdateHPbar", RpcTarget.Others, currentHealth);
            Debug.Log($"{gameObject.name}가(이) {amount}만큼 데미지를 받음. 현재 체력: {currentHealth} playerhealth.스크립트");
            
            if (currentHealth <= 0f)
            {
                Debug.Log($"{photonView.Owner.NickName} 사망");
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
            // 사망 처리 부분 넣어야 함
        }
    }
