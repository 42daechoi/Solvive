using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    private float xRotation;
    private float yRotation;
    private PhotonView _photonView;
    private MouseControl mouseControl;
    private bool _isCameraActive = true;

    [SerializeField] Transform camFollowPos;
    [SerializeField] CinemachineVirtualCamera virtualCamera;

    [SerializeField] Transform targetObject;
    
    public Transform TargetObject => targetObject;
    public float YRotation => yRotation;
    
    [SerializeField] private float mouseSensitivity = 1.0f;
    [SerializeField] private float minY = -80f;
    [SerializeField] private float maxY = 80f;
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
        mouseControl = FindObjectOfType<MouseControl>();
        if (_photonView.IsMine)
        {
            if (virtualCamera != null && camFollowPos != null)
            {
                virtualCamera.Follow = camFollowPos;
                virtualCamera.LookAt = camFollowPos;
                virtualCamera.gameObject.SetActive(true);
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

    /*void Update()
    {
        
    }*/

    private void LateUpdate()
    {
        if (!_photonView.IsMine || !_isCameraActive) return;

        if (mouseControl == null) return;

        float sensitivity = mouseControl.mouseSensitivity;
        
        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivity;

        xRotation += mouseX;
        yRotation -= mouseY;
        yRotation = Mathf.Clamp(yRotation, minY, maxY);
        
        transform.rotation = Quaternion.Euler(0f, xRotation, 0f);
        virtualCamera.transform.localEulerAngles = new Vector3(yRotation, 0f, 0f);
        
        if (targetObject != null)
        {
            Vector3 pos = targetObject.position;
            pos.y = -yRotation;
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