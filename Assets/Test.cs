
// #define INT



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR

using UnityEditor;
#endif

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var (enumCount, dicCount, type) = (0, 0, "NONE");

        (enumCount, dicCount, type) = new Hoge().Count();

        GetComponent<Text>().text += $"{type}\ndic:{dicCount}\nenum:{enumCount}";
    }



}
