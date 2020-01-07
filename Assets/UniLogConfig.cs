using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniLog;

public class UniLogConfig : MonoBehaviour
{
    [Serializable]
    public struct LevelEntry
    {
        public string LoggerName;
        public UniLogger.Level level;
    }

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
                UniLogger.GetLogger(le.LoggerName).LogLevel = le.level; 
        }
    }



}
