#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace MurderVilla.Editor
{
    public static class MacAppBuilder
    {
        private const string ScenePath = "Assets/_Project/Scenes/VillaHorrorPrototype.unity";
        private const string OutputPath = "Builds/macOS/MurderInTheOldVilla.app";

        [MenuItem("Murder in Old Villa/Build macOS Test App")]
        public static void Build()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(OutputPath) ?? "Builds/macOS");
            BuildPlayerOptions options = new()
            {
                scenes = new[] { ScenePath },
                locationPathName = OutputPath,
                target = BuildTarget.StandaloneOSX,
                options = BuildOptions.Development,
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
                throw new InvalidOperationException(
                    $"macOS build failed: {report.summary.result} ({report.summary.totalErrors} errors)");

            Debug.Log($"macOS test app built at {OutputPath} " +
                $"({report.summary.totalSize / (1024f * 1024f):0.0} MB).");
        }
    }
}
#endif
