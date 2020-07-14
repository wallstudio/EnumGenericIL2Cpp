
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Hoge
{
#if INT
    public string Name = "INT";
    public Dictionary<int, string> m_table0 = new Dictionary<int, string>(){ [(int)Example0.VALUE_1] = "Piyo" };
#elif ENUM
    public string Name = "ENUM";
    public Dictionary<Example0, string> m_table0 = new Dictionary<Example0, string>(){ [Example0.VALUE_1] = "Piyo" };
#endif
    
    public (int, int, string) Count()
    {
        #region enum count
        var enums = new List<int>();
        enums.Add((int)Example0.VALUE_1);
        #endregion

        var dics = new List<string>();
        
#if INT
        #region int dic count
        dics.Add(m_table0[(int)(Example0.VALUE_1)]);
        #endregion
#elif ENUM
        #region int dic count
        dics.Add(m_table0[Example0.VALUE_1]);
        #endregion
#endif

        return (dics.Count, enums.Count, dics.FirstOrDefault() ?? "NONE");
    }
}

public enum Example0 { VALUE_1, VALUE_2, VALUE_3, VALUE_4, VALUE_5 }

    
