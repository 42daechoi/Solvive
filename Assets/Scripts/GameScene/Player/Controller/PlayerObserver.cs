using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObserver : MonoBehaviour
{
    
    public GameObject itemSlot_Canvas;
    
    public GameObject CrosshairCanvas;
    public float moveSpeed = 10f;       // 이동 속도
    public float rotationSpeed = 100f;
    
    public void HideCowboy(GameObject cowboy)
    {
        // 모든 SkinnedMeshRenderer 비활성화
        SkinnedMeshRenderer[] skinnedRenderers = cowboy.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer smr in skinnedRenderers)
        {
            smr.enabled = false;
        }
    
        // 모든 Collider 비활성화
        Collider[] colliders = cowboy.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }
        
        if (itemSlot_Canvas != null)
        {
            itemSlot_Canvas.SetActive(false);
        }

        if (CrosshairCanvas != null)
        {
            CrosshairCanvas.SetActive(false);
        }
    }
    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        float moveUp = 0f;
        if (Input.GetKey(KeyCode.E))
            moveUp = 1f;
        else if (Input.GetKey(KeyCode.Q))
            moveUp = -1f;
        
        Vector3 movement = new Vector3(moveHorizontal, moveUp, moveVertical) * moveSpeed * Time.deltaTime;
        transform.Translate(movement, Space.Self);
        
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float mouseY = -Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
        transform.Rotate(mouseY, mouseX, 0);
    }

    void Start()
    {
        GameObject[] uiObjects = GameObject.FindGameObjectsWithTag("IngameUI");
        foreach (GameObject ui in uiObjects)
        {
            if (ui.name == "ItemSlotCanvas")
            {
                itemSlot_Canvas = ui;
            }
            else if (ui.name == "CrosshairCanvas")
            {
                CrosshairCanvas = ui;
            }
        }

        // 할당이 제대로 되었는지 확인
        if (itemSlot_Canvas == null || CrosshairCanvas == null)
        {
            Debug.LogWarning("UI 오브젝트 중 일부를 찾을 수 없습니다.");
        }
    }
}
