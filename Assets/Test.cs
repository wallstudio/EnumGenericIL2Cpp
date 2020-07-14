
// #define INT



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Linq;
using UnityEngine.UI;

#if UNITY_EDITOR

using UnityEditor.Build.Reporting;
using UnityEditor;

static class Builder
{
    
    [MenuItem("Build/INT")]
    static void BuildInt() => Build("INT");
    [MenuItem("Build/ENUM")]
    static void BuildEnum() => Build("ENUM");
    [MenuItem("Build/NONE")]
    static void BuildNone() => Build("NONE");
    [MenuItem("Build/ALL")]
    static void BuildAll()
    {
        BuildInt();
        BuildEnum();
        BuildNone();
    }
    
    
    static void Build(string def)
    {
        foreach (var g in new[]{BuildTargetGroup.Android, BuildTargetGroup.iOS})
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(g, def);
        }

        foreach (var tar in new[]{BuildTarget.Android, BuildTarget.iOS})
        {
            for (int i = 0; i < 5; i++)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                string path = $"Build/Build_{def}_{tar}_{i}";
                var e = BuildPipeline.BuildPlayer(new []{new EditorBuildSettingsScene("Assets/SampleScene.unity", true)}, path, tar, BuildOptions.None);
                File.AppendAllLines(
                    "Build/log.txt",
                    new []{$"{def} {tar} [{i}] {sw.Elapsed.TotalMilliseconds:F0}ms {GetDirectorySize(path)/1000d/1000d:F6}MB"});
                if(e.summary.result != BuildResult.Succeeded)
                {
                    break;
                }
                Thread.Sleep(500);
            }
        }
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

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #if ENUM || INT
        GetComponent<Text>().text += new SPACE_0.Hoge().Name + "\n";
        GetComponent<Text>().text += new SPACE_1.Hoge().Name + "\n";
        GetComponent<Text>().text += new SPACE_2.Hoge().Name + "\n";
        GetComponent<Text>().text += new SPACE_3.Hoge().Name + "\n";
        GetComponent<Text>().text += new SPACE_4.Hoge().Name + "\n";
        GetComponent<Text>().text += new SPACE_5.Hoge().Name + "\n";
        GetComponent<Text>().text += new SPACE_6.Hoge().Name + "\n";
        GetComponent<Text>().text += new SPACE_7.Hoge().Name + "\n";
        GetComponent<Text>().text += new SPACE_8.Hoge().Name + "\n";
        GetComponent<Text>().text += new SPACE_9.Hoge().Name + "\n";
        GetComponent<Text>().text += new SPACE_10.Hoge().Name + "\n";
        GetComponent<Text>().text += new SPACE_11.Hoge().Name + "\n";
        GetComponent<Text>().text += new SPACE_12.Hoge().Name + "\n";
        GetComponent<Text>().text += new SPACE_13.Hoge().Name + "\n";
        GetComponent<Text>().text += new SPACE_14.Hoge().Name + "\n";
        GetComponent<Text>().text += new SPACE_15.Hoge().Name + "\n";
        GetComponent<Text>().text += new SPACE_16.Hoge().Name + "\n";
        GetComponent<Text>().text += new SPACE_17.Hoge().Name + "\n";
        GetComponent<Text>().text += new SPACE_18.Hoge().Name + "\n";
        GetComponent<Text>().text += new SPACE_19.Hoge().Name + "\n";
        #endif
    }

}
