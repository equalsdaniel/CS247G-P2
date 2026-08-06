#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace MurderVilla.Editor
{
    public static class BatchBuilder
    {
        [MenuItem("Murder in Old Villa/Build macOS App")]
        public static void BuildMacOS()
        {
            string[] scenes = {
                "Assets/_Project/Scenes/VillaHorrorPrototype.unity"
            };

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = "Builds/macOS/MurderInTheOldVilla.app",
                target = BuildTarget.StandaloneOSX,
                options = BuildOptions.None,
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Build succeeded: " + summary.totalSize + " bytes, " + summary.totalTime);
            }
            else
            {
                Debug.LogError("Build failed: " + summary.result + ", errors: " + summary.totalErrors);
            }
        }
    }
}
#endif
