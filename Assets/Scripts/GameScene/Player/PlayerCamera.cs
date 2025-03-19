using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using Cinemachine;

public class PlayerCamera : MonoBehaviour
{
    public Cinemachine.AxisState xAxis, yAxis;
    private PhotonView _photonView;
    private bool _isCameraActive = true;

    [SerializeField] Transform camFollowPos;
    [SerializeField] CinemachineVirtualCamera virtualCamera;

    [SerializeField] Transform targetObject;
    
    public Transform TargetObject => targetObject;
    
    private void OnEnable()
    {
        if (EventManager_Game.Instance != null)
        {
            EventManager_Game.Instance.OnCameraActive += HandleCameraActive;
        }
    }
    
    private void OnDisable()
    {
        if (EventManager_Game.Instance != null)
        {
            EventManager_Game.Instance.OnCameraActive -= HandleCameraActive;
        }
    }
    
    void Start()
    {
        _photonView = GetComponent<PhotonView>();

        if (_photonView.IsMine)
        {
            if (virtualCamera != null && camFollowPos != null)
            {
                virtualCamera.Follow = camFollowPos;
                virtualCamera.LookAt = camFollowPos;
                virtualCamera.gameObject.SetActive(true);

                // Axis 초기화
                xAxis.Reset();
                yAxis.Reset();
            }
        }
        else
        {
            if (virtualCamera != null)
            {
                virtualCamera.gameObject.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (_photonView.IsMine && _isCameraActive)
        {
            xAxis.Update(Time.deltaTime);
            yAxis.Update(Time.deltaTime);
        }
    }

    private void LateUpdate()
    {
        if (!_photonView.IsMine || !_isCameraActive) return;
        
        if (virtualCamera.Follow == null || virtualCamera.LookAt == null)
        {
            virtualCamera.Follow = camFollowPos;
            virtualCamera.LookAt = camFollowPos;
        }

        Vector3 cameraRotation = virtualCamera.transform.localEulerAngles;
        cameraRotation.x = yAxis.Value;
        virtualCamera.transform.localEulerAngles = cameraRotation;
        transform.eulerAngles = new Vector3(0f, xAxis.Value, 0f);

        if (targetObject != null)
        {
            float xRotation = yAxis.Value;
            if (xRotation > 180f) xRotation -= 360f;
            
            Vector3 pos = targetObject.position;
            pos.y = -xRotation;
            targetObject.position = pos;
        }
    }
    
    private void HandleCameraActive(bool isActive)
    {
        _isCameraActive = isActive;
    }
    
    public void SetVirtualCamera(CinemachineVirtualCamera newCamera)
    {
        virtualCamera = newCamera;
    
        if (_photonView.IsMine && virtualCamera != null)
        {
            virtualCamera.Follow = camFollowPos;
            virtualCamera.LookAt = camFollowPos;
            virtualCamera.gameObject.SetActive(true);
        }
    }

    public void SetCamFollowPos(Transform newFollowPos)
    {
        camFollowPos = newFollowPos;

        if (_photonView.IsMine && virtualCamera != null)
        {
            virtualCamera.Follow = camFollowPos;
            virtualCamera.LookAt = camFollowPos;
        }
    }
}