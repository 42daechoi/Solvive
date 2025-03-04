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
    
    [SerializeField] Transform lookAtTarget;
    [SerializeField] float lookAtDistance = 10f;
    
    // 캐싱을 위한 변수들
    private Vector3 _rotatedForward;
    private Vector3 _lookAtPosition;
    private Vector3 _forwardDir;
    private Vector3 _rightAxis;
    private Quaternion _rotation;
    
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
        // PlayerController에서 PhotonView 가져오기
        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            _photonView = playerController.GetPhotonView();
        }
        else
        {
            // 백업 옵션: PlayerController가 없는 경우에만 직접 가져오기
            _photonView = GetComponent<PhotonView>();
        }

        if (_photonView.IsMine)
        {
            if (virtualCamera != null && camFollowPos != null)
            {
                virtualCamera.Follow = camFollowPos;
                
                if (lookAtTarget == null)
                {
                    GameObject lookAtObj = new GameObject("LookAtTarget");
                    lookAtTarget = lookAtObj.transform;
                    lookAtTarget.parent = transform;
                }
                
                virtualCamera.LookAt = lookAtTarget;
                virtualCamera.gameObject.SetActive(true);
                
                _forwardDir = Vector3.forward;
                _rightAxis = Vector3.right;
                _rotatedForward = Vector3.forward;
                _lookAtPosition = Vector3.zero;
                
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
        
        transform.eulerAngles = new Vector3(0f, xAxis.Value, 0f);
        
        float pitch = yAxis.Value;
        
        _forwardDir = transform.forward;
        _rightAxis = transform.right;
        
        _rotation = Quaternion.AngleAxis(-pitch, _rightAxis);
        _rotatedForward = _rotation * _forwardDir;
        
        _lookAtPosition = camFollowPos.position + _rotatedForward * lookAtDistance;
        
        lookAtTarget.position = _lookAtPosition;
    }
    
    private void HandleCameraActive(bool isActive)
    {
        _isCameraActive = isActive;
    }
}