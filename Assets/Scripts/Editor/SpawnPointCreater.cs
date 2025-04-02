using UnityEditor;
using UnityEngine;

public class SpawnPointCreator : EditorWindow
{
    [MenuItem("Tools/Create Spawn Point %&-")] // Ctrl + alt + - 단축키
    private static void CreateSpawnPoint()
    {
        SceneView sceneView = SceneView.lastActiveSceneView;
        if (sceneView == null)
        {
            Debug.LogWarning("씬 뷰가 없습니다.");
            return;
        }

        Vector3 spawnPosition = sceneView.camera.transform.position;
        Quaternion spawnRotation = sceneView.camera.transform.rotation;

        GameObject spawnPoint = new GameObject("GeneratorSpawnPoint");
        spawnPoint.transform.position = spawnPosition;
        spawnPoint.transform.rotation = spawnRotation;

        Selection.activeGameObject = spawnPoint;

        Debug.Log($"스폰 포인트 생성됨: {spawnPosition}");
    }
}
