using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniLog;

public class UniLogConfig : MonoBehaviour
{

#if UNITY_EDITOR
    // Only use this when playing in editor

    [Serializable]
    public struct LevelEntry
    {
        public string LoggerName;
        public UniLogger.Level level;
    }

    public bool throwOnError = false;
    public List<LevelEntry> levelEntries;

    // Start is called before the first frame update
    void Awake()
    {
        UpdateValues();
    }

    void Start()
    {
        InvokeRepeating("UpdateValues", .5f, .5f);
    }

    void UpdateValues()
    {
        foreach (LevelEntry le in levelEntries)
        {
            if ((le.LoggerName.Length > 0))
            {
                UniLogger.GetLogger(le.LoggerName).LogLevel = le.level;
                UniLogger.GetLogger(le.LoggerName).ThrowOnError = throwOnError;
            }
        }
    }


#endif

}

