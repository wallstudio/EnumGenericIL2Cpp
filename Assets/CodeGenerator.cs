
// #define INT
#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;


public static class CodeGenerator
{
    const string template =
@"
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Hoge
{
#if INT
    public string Name = ""_INT"";
%D_FOR%    public Dictionary<int, string> m_table%D_NUMBER% = new Dictionary<int, string>(){ [(int)Example%D_NUMBER%.VALUE_1] = ""Piyo"" };
#elif ENUM
    public string Name = ""_ENUM"";
%D_FOR%    public Dictionary<Example%D_NUMBER%, string> m_table%D_NUMBER% = new Dictionary<Example%D_NUMBER%, string>(){ [Example%D_NUMBER%.VALUE_1] = ""Piyo"" };
#endif
    
    public (int dicCount, int enumCount, string define) Count()
    {
        #region enum count
        var enums = new List<int>();
%E_FOR%        enums.Add((int)Example%E_NUMBER%.VALUE_1);
        #endregion

        var dics = new List<string>();
        
#if INT
        #region int dic count
%D_FOR%        dics.Add(m_table%D_NUMBER%[(int)(Example%D_NUMBER%.VALUE_1)]);
        #endregion
#elif ENUM
        #region int dic count
%D_FOR%        dics.Add(m_table%D_NUMBER%[Example%D_NUMBER%.VALUE_1]);
        #endregion
#endif

        return (dics.Count, enums.Count, dics.FirstOrDefault() ?? ""_NONE"");
    }
}

%E_FOR%public enum Example%E_NUMBER% { VALUE_1, VALUE_2, VALUE_3, VALUE_4, VALUE_5 }

    ";



    [MenuItem("CodeGen/1_1")]
    static void CodeGen_1_1() => CodeGen(1, 1);
    [MenuItem("CodeGen/15_10")]
    static void CodeGen_15_10() => CodeGen(15, 10);

    public static void CodeGen(int enumCount, int genericCount)
    {
        if(enumCount < genericCount)
        {
            throw new Exception($"enum:{enumCount} generic:{genericCount}");
        }
        

        var lines = new List<string>();
        foreach (var line in template.Replace("\r", "").Split('\n'))
        {
            if(line.StartsWith("%E_FOR%"))
            {
                var trimedLine = line.Replace("%E_FOR%", "");
                for (int i = 0; i < enumCount; i++)
                {
                    lines.Add(trimedLine.Replace("%E_NUMBER%", i.ToString()));
                }
            }
            else if(line.StartsWith("%D_FOR%"))
            {
                var trimedLine = line.Replace("%D_FOR%", "");
                for (int i = 0; i < genericCount; i++)
                {
                    lines.Add(trimedLine.Replace("%D_NUMBER%", i.ToString()));
                }
            }
            else
            {
                lines.Add(line);
            }
        }

        File.WriteAllLines("Assets/Generated.cs", lines);
        AssetImporter.GetAtPath("Assets/Generated.cs").SaveAndReimport();
        AssetDatabase.Refresh();
    }
}

#endif