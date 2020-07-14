
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

    const string CMD_LIST = "commnad.csv";

    [MenuItem("Build/ALL")]
    static void BuildAll()
    {
        var commands = new List<string>();
        foreach (var (targ, tar) in new[]{(BuildTargetGroup.iOS, BuildTarget.iOS), (BuildTargetGroup.Android, BuildTarget.Android)})
        {
            foreach (var def in new[]{ "NONE", "INT", "ENUM"})
            {
                var max = 16384; // 2^14
                for (int i = 64; i <= max; i *= 2) // 8 times
                {
                    var command = string.Join(",", new object[]{true, targ, tar, def, max, i, 5}.Select(o => o.ToString()));
                    commands.Add(command);
                }
            }
        }
        File.WriteAllLines(CMD_LIST, commands);
        Build();
    }
    
    [InitializeOnLoadMethod]
    static void Build()
    {
        // args
        if(!File.Exists(CMD_LIST)) return;
        var commands = File.ReadAllLines(CMD_LIST);
        if(commands.Length == 0) return;
        var args = commands.FirstOrDefault().Split(',');
        if(args.Length < 7) return;
        if(!bool.TryParse(args[0], out var isPrepare)) return;
        if(!Enum.TryParse<BuildTargetGroup>(args[1], out var targ)) return;
        if(!Enum.TryParse<BuildTarget>(args[2], out var tar)) return;
        string def = args[3];
        if(!int.TryParse(args[4], out var enumCount)) return;
        if(!int.TryParse(args[5], out var dicCount)) return;
        if(!int.TryParse(args[6], out var iter)) return;

        var called = false;
        var current = DateTime.Now;
        UnityEngine.Debug.Log($"Delay 10s");
        EditorApplication.update += () => 
        {
            if(called) return;
            if((DateTime.Now - current).TotalSeconds < 10) return;
            called = true;
            

            if(isPrepare)
            {
                var newCommand = string.Join(",", new object[]{false, targ, tar, def, enumCount, dicCount, iter}.Select(o => o.ToString()));
                File.WriteAllLines(CMD_LIST, new []{newCommand}.Concat(commands.Skip(1)));

                foreach (var g in new[]{BuildTargetGroup.Android, BuildTargetGroup.iOS})
                {
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(g, def);
                }
                CodeGenerator.CodeGen(enumCount, dicCount);
                AssetDatabase.Refresh(); // Build();
                return;
            }
            else
            {
                var span = new List<double>();
                var size = new List<long>();
                for (int i = 0; i < iter; i++) // mean
                {
                    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);

                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    var hoge = new Hoge().Count();
                    UnityEngine.Debug.Log(hoge);
                    
                    string path = $"Build/Build_{i}_{def}_{tar}_{dicCount}_{enumCount}";
                    var scenes = new []{new EditorBuildSettingsScene("Assets/SampleScene.unity", true)};
                    var e = BuildPipeline.BuildPlayer(scenes, path, tar, BuildOptions.None);
                    span.Add(sw.Elapsed.TotalMilliseconds);
                    size.Add(GetDirectorySize(path));

                    string line = $"{i}, {hoge.define}, {tar}, {hoge.dicCount}, {hoge.enumCount}, {sw.Elapsed.TotalMilliseconds:F0}, {GetDirectorySize(path)}";
                    File.AppendAllLines("Build/log.csv", new []{line});

                    if(e.summary.result != BuildResult.Succeeded)
                    {
                        File.WriteAllText(CMD_LIST, "");
                        throw new Exception();
                    }
                    UnityEngine.Debug.Log((span.Average(), (long)size.Average()));

                    File.WriteAllLines(CMD_LIST, commands.Skip(1));
                }

                Thread.Sleep(500);
                Build();
                return;
            }
        };
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