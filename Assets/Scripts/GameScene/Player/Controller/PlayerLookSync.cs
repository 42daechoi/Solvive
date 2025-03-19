using Photon.Pun;
using UnityEngine;

public class PlayerLookSync : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private Transform remoteTargetObject;
    private PlayerCamera _playerCamera;
    
    private float _networkYRotation = 0f;
    private float _lastNetworkYRotation = 0f;
    private float _currentYPosition = 0f;
    private float _lastReceiveTime = 0f;
    
    [SerializeField] private float _smoothSpeed = 20f;
    private const float Y_MOVE_SCALE = 0.3f;

    private void Start()
    {
        _playerCamera = GetComponent<PlayerCamera>();
        
        if (remoteTargetObject == null)
        {
            GameObject targetObj = new GameObject("RemoteLookTarget");
            targetObj.transform.parent = transform;
            targetObj.transform.localPosition = new Vector3(0, 0, 8.2f);
            remoteTargetObject = targetObj.transform;
        }
        
        if (!photonView.IsMine)
        {
            RootMotion.FinalIK.LookAtIK lookAtIK = GetComponent<RootMotion.FinalIK.LookAtIK>();
            if (lookAtIK != null)
            {
                lookAtIK.solver.target = remoteTargetObject;
            }
            else
            {
                Debug.LogWarning("LookAtIK not found on remote player!");
            }
        }
    }
    
    private void Update()
    {
        if (!photonView.IsMine && remoteTargetObject != null)
        {
            float timeSinceLastUpdate = Time.time - _lastReceiveTime;
            float lerpFactor = Mathf.Clamp01(timeSinceLastUpdate * _smoothSpeed);
            
            _currentYPosition = Mathf.Lerp(_currentYPosition, -_networkYRotation * Y_MOVE_SCALE, lerpFactor);
            
            Vector3 pos = remoteTargetObject.localPosition;
            pos.y = _currentYPosition;
            remoteTargetObject.localPosition = pos;
        }
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if (_playerCamera != null)
            {
                short compressedYRot = (short)(_playerCamera.yAxis.Value * 100);
                stream.SendNext(compressedYRot);
            }
            else
            {
                stream.SendNext((short)0);
            }
        }
        else
        {
            _lastNetworkYRotation = _networkYRotation;
            _networkYRotation = (short)stream.ReceiveNext() / 100f;
            _lastReceiveTime = Time.time;
        }
    }
}
