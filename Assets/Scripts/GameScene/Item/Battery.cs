using Photon.Pun;
using UnityEngine;

[CreateAssetMenu(fileName = "Battery", menuName = "ScriptableObjects/Battery")]
public class Battery : Item
{
    [SerializeField] private RayModule rayModule;
    public override void UseItem()
    {
        Transform shooterTransform = GetShooterTransform();
        if (shooterTransform == null)
        {
            Debug.Log("Battery : 플레이어 트랜스폼을 찾지 못했습니다.");
            return;
        }

        if (rayModule != null && shooterTransform != null)
        {
            RaycastHit? raycastHit = rayModule.ExecuteRayAction(shooterTransform, 0f);
            if (raycastHit.HasValue)
            {
                Generator generator = raycastHit.Value.collider.GetComponent<Generator>();
                if (generator != null)
                {
                    Debug.Log("Battery : 발전기 레이 히트 성공");
                    int playerID = shooterTransform.gameObject.GetPhotonView().ViewID;
                    generator.TryInstallBattery(playerID);
                }
            }
        }
        else
        {
            Debug.LogWarning($"RayModule 또는 ShooterTransform이 유효하지 않습니다.");
        }
    }

    private Transform GetShooterTransform()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            return playerObj.transform;
        return null;
    }
}
