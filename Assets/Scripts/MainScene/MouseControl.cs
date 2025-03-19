using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MouseControl : MonoBehaviour
{
    public float mouseSensitivity = 1f; // 기본 마우스 감도
    public Transform playerBody;

    // 슬라이더와 인풋 필드를 연결할 변수
    public Slider sensitivitySlider;
    public TMP_InputField sensitivityInput;

    private float xRotation = 0f;

    void Start()
    {
        // 슬라이더와 인풋 필드에 초기 감도 설정
        sensitivitySlider.value = mouseSensitivity;
        sensitivityInput.text = mouseSensitivity.ToString();

        // 이벤트 리스너 추가
        sensitivitySlider.onValueChanged.AddListener(UpdateSensitivityFromSlider);
        sensitivityInput.onEndEdit.AddListener(UpdateSensitivityFromInput);

        if (sensitivitySlider != null)
    {
        sensitivitySlider.value = mouseSensitivity;
        sensitivitySlider.onValueChanged.AddListener(UpdateSensitivityFromSlider);
    }
    else
    {
        Debug.LogWarning("sensitivitySlider가 할당되지 않았습니다!");
    }

    if (sensitivityInput != null)
    {
        sensitivityInput.text = mouseSensitivity.ToString();
        sensitivityInput.onEndEdit.AddListener(UpdateSensitivityFromInput);
    }
    else
    {
        Debug.LogWarning("sensitivityInput이 할당되지 않았습니다!");
    }
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    // 슬라이더 값이 변경될 때 호출되는 함수
    public void UpdateSensitivityFromSlider(float value)
    {
        mouseSensitivity = value;
        sensitivityInput.text = value.ToString("F2"); // 소수점 1자리까지 표시
    }

    // 입력 필드에 값이 입력되었을 때 호출되는 함수
    public void UpdateSensitivityFromInput(string value)
    {
        if (float.TryParse(value, out float newSensitivity))
        {
            mouseSensitivity = newSensitivity;
            sensitivitySlider.value = newSensitivity; // 슬라이더 값도 업데이트
        }
    }
}
