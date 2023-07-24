using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

#if UNITY_WEBGL
using System.Runtime.InteropServices;
using AOT;
#endif

public class WebGLBrowserClip
{

#if UNITY_WEBGL && !UNITY_EDITOR

    [DllImport("__Internal")]
    private static extern bool JS_CopyToBrowserClip( string textToCopy);


    public static void Copy( string textToCopy)
    {
        JS_CopyToBrowserClip(textToCopy);
    }

#endif // UNITY_WEBGL

}

