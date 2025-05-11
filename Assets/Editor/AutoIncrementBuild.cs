using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class AutoIncrementBuildOnBuild : IPreprocessBuildWithReport
{
    private const string BuildNumberKey = "BuildNumber";

    // Implement the IPreprocessBuildWithReport interface
    public int callbackOrder { get { return 0; } }

    public void OnPreprocessBuild(BuildReport report)
    {
        int buildNumber = EditorPrefs.GetInt(BuildNumberKey, 0);
        buildNumber++;
        EditorPrefs.SetInt(BuildNumberKey, buildNumber);
        string[] versionAry = Application.version.Split(".");
        string version = versionAry[0];
        string buildVersion = $"{version}.{buildNumber}";

        PlayerSettings.bundleVersion = buildVersion;
        Debug.Log($"Auto-incremented build version to: {buildVersion}");
    }

    [MenuItem("Build/Reset Auto Build Number")]
    public static void ResetAutoBuildNumber()
    {
        EditorPrefs.DeleteKey(BuildNumberKey);
        Debug.Log("Auto build number reset to 0.");
    }
}