using GameScene.Item;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun; // Photon 사용 시

public class Raticle : MonoBehaviour
{
    [Header("Crosshair Settings")]
    [SerializeField] private Image crosshairImage; // 조준점 UI 이미지
    [SerializeField] private Sprite farmingSprite; // 파밍 상태 원형 스프라이트
    [SerializeField] private Sprite aimingSprite;  // 총 장착 상태 원형 스프라이트

    [Header("Crosshair Colors")]
    [SerializeField] private Color farmingColor = new Color(1f, 1f, 1f, 0.3f); // 파밍 상태 색상 (연함)
    [SerializeField] private Color aimingColor = new Color(1f, 1f, 1f, 1f);    // 총 장착 상태 색상 (진함)

    [Header("Crosshair Sizes")]
    [SerializeField] private float farmingSize = 25f; // 파밍 상태 크기
    [SerializeField] private float aimingSize = 100f; // 총 장착 상태 크기
    [SerializeField] private float movingSize = 150f; // 이동 중 크기

    [Header("Transition Settings")]
    [SerializeField] private float transitionSpeed = 5f; // 크기 및 색상 전환 속도
    
    // 플레이어 참조
    private Transform playerTransform;
    private HeldItem heldItem;

    // 이동 판별
    private bool isMoving = false;
    private Vector3 lastPosition;

    private void Start()
    {
        // 처음 조준점
        SetCrosshairState(farmingSprite, farmingColor, farmingSize);
        
        // 플레이어 찾고
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

        if (playerTransform != null)
        {
            lastPosition = playerTransform.position;
        }
    }

    private void Update()
    {
        if (playerTransform == null)
        {
            // 로컬 플레이어를 못찾으면 반환
            return;
        }
        
        // (1) 이동 감지
        Vector3 currentPlayerPos = playerTransform.position;
        isMoving = (currentPlayerPos - lastPosition).sqrMagnitude > 0.01f;
        lastPosition = currentPlayerPos;

// (2) 총 장착 여부
        bool isAiming = heldItem.GetItem().itemName == "Gun";

// (3) 조준점 업데이트
        if (isAiming)
        {
            crosshairImage.sprite = aimingSprite;
            if (isMoving)
                UpdateCrosshair(movingSize, aimingColor);
            else
                UpdateCrosshair(aimingSize, aimingColor);
        }
        else
        {
            crosshairImage.sprite = farmingSprite;
            UpdateCrosshair(farmingSize, farmingColor);
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
