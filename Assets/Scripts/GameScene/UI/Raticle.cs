using System.Collections;
using System.Collections.Generic;
using GameScene.Item;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun; // Photon 사용 시

public class Raticle : MonoBehaviour
{
    public static Raticle Instance { get; private set; }
    
    [Header("Crosshair Settings")]
    [SerializeField] private Image crosshairImage; // 조준점 UI 이미지
    [SerializeField] private Sprite farmingSprite; // 파밍 상태 원형 스프라이트
    [SerializeField] private Sprite aimingSprite;  // 총 장착 상태 원형 스프라이트

    [Header("Crosshair Colors")]
    [SerializeField] private Color farmingColor = new Color(1f, 1f, 1f, 0.6f); // 파밍 상태 색상 (연함)
    [SerializeField] private Color aimingColor = new Color(1f, 1f, 1f, 1f);    // 총 장착 상태 색상 (진함)

    [Header("Crosshair Sizes")]
    [SerializeField] private float farmingSize = 20f; // 파밍 상태 크기
    [SerializeField] private float aimingSize = 20f; // 총 장착 상태 크기
    [SerializeField] private float movingSize = 70f; // 이동 중 크기

    [Header("Transition Settings")]
    [SerializeField] private float transitionSpeed = 5f; // 크기 및 색상 전환 
    
    // 플레이어 참조
    private Transform playerTransform;
    private HeldItem heldItem;
    
    private float targetSize;
    private Color targetColor;
    private IState currentState;

    private bool isGun;
    
    private void Awake()
    {
        heldItem = FindObjectOfType<HeldItem>();
        if (heldItem == null)
        {
            Debug.LogWarning("HeldItem 컴포넌트를 찾을 수 없습니다.");
        }
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //처음 조준점 파밍으로 고정
        targetSize = farmingSize;
        targetColor = farmingColor;
        SetCrosshairState(farmingSprite, farmingColor, farmingSize);
        if (playerTransform == null)
        {
            // "Player" 태그 오브젝트를 찾음
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (var p in players)
            {
                PhotonView pv = p.GetComponent<PhotonView>();
                if (pv != null && pv.IsMine)
                {
                    playerTransform = p.transform;
                    heldItem = p.GetComponent<HeldItem>();
                    break;
                }
            }
        }
    }
    private void Update()
    {
        UpdateCrosshair(targetSize, targetColor);
        if (PlayerController.Instance != null)
        {
            currentState = PlayerController.Instance.GetCurrentState();
        }

        if (isGun)
        {
            if (currentState is MoveState || currentState is SprintState || currentState is JumpState)
            {
                targetSize = movingSize;
            }
            else
            {
                targetSize = aimingSize;
            }
        }
        
    }
    
    public void UpdateCrosshairByItemDelayed(Item currentItem)
    {
        StartCoroutine(UpdateCrosshairByItem(currentItem));
    }


    private IEnumerator UpdateCrosshairByItem(Item currentItem)
    {
        yield return new WaitForSeconds(0.1f);
        
        if (currentItem != null && currentItem.itemName == "Gun")
        {
            // 총인 경우 총 장착 상태로 업데이트
            crosshairImage.sprite = aimingSprite;
            targetSize = aimingSize;
            targetColor = aimingColor;
            isGun = true;
        }
        else
        {
            // 그 외에는 파밍 상태로 업데이트
            crosshairImage.sprite = farmingSprite;
            targetSize = farmingSize;
            targetColor = farmingColor;
            isGun = false;
        }
    }
    
    private void UpdateCrosshair(float targetSize, Color targetColor)
    {
        // 크기와 색상을 부드럽게 전환
        crosshairImage.rectTransform.sizeDelta = Vector2.Lerp(
            crosshairImage.rectTransform.sizeDelta,
            new Vector2(targetSize, targetSize),
            Time.deltaTime * transitionSpeed
        );
        crosshairImage.color = Color.Lerp(
            crosshairImage.color,
            targetColor,
            Time.deltaTime * transitionSpeed
        );
    }

    private void SetCrosshairState(Sprite sprite, Color color, float size)
    {
        crosshairImage.sprite = sprite;
        crosshairImage.color = color;
        crosshairImage.rectTransform.sizeDelta = new Vector2(size, size);
    }
}
