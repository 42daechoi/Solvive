using Photon.Pun;
using UnityEngine;


    public enum RayType
    {
        RandomShot,
        KnifeStab
    }
    
    [CreateAssetMenu(fileName = "NewRayModule", menuName = "ScriptableObjects/RayModule")]
public class RayModule : ScriptableObject
{
    [Header("레이 동작 타입")]
    public RayType rayType;

    [Header("사격 랜덤 범위"), Tooltip("Speed가 0이 아닐 때 사격 각도 무작위 범위를 조절")]
    public float randomRange = 2f;

    public RaycastHit? ExecuteRayAction(Transform shooterTransform, float speed)
    {
        switch (rayType)
        {
            case RayType.RandomShot:
                return PerformRandomShot(shooterTransform, speed);
            case RayType.KnifeStab:
                return PerformShortStab(shooterTransform);
        }

        return null;
    }

    private RaycastHit? PerformRandomShot(Transform shooterTransform, float speed)
    {
        // 1) 카메라 정중앙에서 레이를 얻는다.
        //    (화면 중앙 픽셀(Screen.width / 2, Screen.height / 2) 기준)
        Ray centerRay = Camera.main.ScreenPointToRay(
            new Vector3(Screen.width / 2, Screen.height / 2, 0)
        );

        // 레이 시작점(origin)과 기본 방향(direction)
        Vector3 origin = centerRay.origin;
        Vector3 direction = centerRay.direction;

        // 2) 이동 속도가 0이 아니라면, 랜덤 각도 적용
        if (!Mathf.Approximately(speed, 0f))
        {
            float randX = Random.Range(-randomRange, randomRange);
            float randY = Random.Range(-randomRange, randomRange);
    
            // direction에 랜덤 회전을 곱해줌
            direction = Quaternion.Euler(randX, randY, 0f) * direction;
        }

        // 3) 최종 레이 생성
        Ray finalRay = new Ray(origin, direction);

        // 4) 레이캐스트
        int playerLayer = LayerMask.NameToLayer("Player");
        int layerMask = ~(1 << playerLayer);

        if (Physics.Raycast(finalRay, out RaycastHit hit, 100f, layerMask))
        {
            PhotonView myView = shooterTransform.GetComponentInParent<PhotonView>();
            PhotonView hitView = hit.collider.GetComponentInParent<PhotonView>();
            
            if (hitView == null || myView == null || hitView.ViewID != myView.ViewID)
            {
                Debug.DrawRay(origin, direction * 100f, Color.red, 1f);
                return hit;
            }
        }
        return null;
    }


    private RaycastHit? PerformShortStab(Transform shooterTransform)
    {
        float knifeRange = 2f;

        // 1) 화면 중앙 기준 레이
        Ray centerRay = Camera.main.ScreenPointToRay(
            new Vector3(Screen.width / 2, Screen.height / 2, 0)
        );

        // 2) 레이 생성 (origin/direction)
        Vector3 origin = centerRay.origin;
        Vector3 direction = centerRay.direction;
        Ray finalRay = new Ray(origin, direction);

        // 3) 레이캐스트 (knifeRange 까지만)
        if (Physics.Raycast(finalRay, out RaycastHit hit, knifeRange))
        {
            PhotonView myView = shooterTransform.GetComponentInParent<PhotonView>();
            PhotonView hitView = hit.collider.GetComponentInParent<PhotonView>();
            
            if (hitView == null || myView == null || hitView.ViewID != myView.ViewID)
            {
                Debug.DrawRay(origin, direction * knifeRange, Color.blue, 1f);
                return hit;
            }
        }
        return null;
    }
}
