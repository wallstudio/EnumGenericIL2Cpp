
// #define INT
#if UNITY_EDITOR

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Linq;
using UnityEditor.Build.Reporting;
using UnityEditor;
using System.Collections.Generic;

public static class Builder
{
    
    // [MenuItem("Build/INT")]
    // static void BuildInt() => Build("INT", 100, 100);
    // [MenuItem("Build/ENUM")]
    // static void BuildEnum() => Build("ENUM", 100, 100);
    // [MenuItem("Build/NONE")]
    // static void BuildNone() => Build("NONE", 100, 100);

    [MenuItem("Build/ALL")]
    static void BuildAll()
    {
        foreach (var (targ, tar) in new[]{(BuildTargetGroup.iOS, BuildTarget.iOS), (BuildTargetGroup.Android, BuildTarget.Android)})
        {
            foreach (var def in new[]{"NONW", "INT", "ENUM"})
            {
                const string AVERAGE_LOG = "Build/mean_log.txt";
                var max = 262144; // 2^18
                for (int i = 64; i <= max; i *= i) // 12 times
                {
                    var (time, size) = Build(targ, tar, def, max, i, 5);
                    string line = $"{i}, {def}, {tar}, {i}, {max}, {time:F0}, {size}";
                    File.AppendAllLines(AVERAGE_LOG, new []{ line });
                }
                File.AppendAllLines(AVERAGE_LOG, new []{""});
            }
        }
    }
    
    
    static (double, long) Build(BuildTargetGroup targ, BuildTarget tar, string def, int enumCount = 100, int dicCount = 100, int iter = 3)
    {
        foreach (var g in new[]{BuildTargetGroup.Android, BuildTargetGroup.iOS})
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(g, def);
        }

        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);

        var span = new List<double>();
        var size = new List<long>();
        for (int i = 0; i < iter; i++) // mean
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            CodeGenerator.CodeGen(enumCount, dicCount);
            AssetDatabase.Refresh();
            UnityEngine.Debug.Log(new Hoge().Count());
            
            string path = $"Build/Build_{i}_{def}_{tar}_{dicCount}_{enumCount}";
            var e = BuildPipeline.BuildPlayer(new []{new EditorBuildSettingsScene("Assets/SampleScene.unity", true)}, path, tar, BuildOptions.None);
            span.Add(sw.Elapsed.TotalMilliseconds);
            size.Add(GetDirectorySize(path));

            string line = $"{i}, {def}, {tar}, {dicCount}, {enumCount}, {sw.Elapsed.TotalMilliseconds:F0}, {GetDirectorySize(path)}";
            File.AppendAllLines("Build/log.txt", new []{line});

            if(e.summary.result != BuildResult.Succeeded)
            {
                throw new Exception();
            }
            Thread.Sleep(500);
        }

        return (span.Average(), (long)size.Average());
    }

    static long GetDirectorySize(string path)
    {
        if(File.Exists(path))
        {
            return new FileInfo(path).Length;
        }
        else if(Directory.Exists(path))
        {
            return Directory.GetFiles(path).Concat(Directory.GetDirectories(path)).Sum(GetDirectorySize);
        }
        else
        {
            throw new Exception(path);
        }
    }
}

#endif