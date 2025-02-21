using UnityEditor;
public class MultiplayerBuildAndRun
{
    [MenuItem("Tools/Multiplay Test")]
    private static void Win64()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);

        for (int i = 0; i < 1; i++)
        {
            BuildPlayerOptions options = new BuildPlayerOptions();
            options.scenes = GetScenesPath();
            options.locationPathName = string.Format("Build/Win64/{0}/Test.exe", i);
            options.target = BuildTarget.StandaloneWindows64;
            options.options = BuildOptions.AutoRunPlayer;
            BuildPipeline.BuildPlayer(options);
        }
    }

    private static string[] GetScenesPath()
    {
        EditorBuildSettingsScene[] scenes = UnityEditor.EditorBuildSettings.scenes;
        string[] scenes_path = new string[scenes.Length];

        for (int i = 0;i < scenes.Length;i++)
        {
            scenes_path[i] = scenes[i].path;
        }

        return scenes_path;
    }
}