using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniLog;

public class UniLogConfig : MonoBehaviour
{
    public UniLogger.Level P2pNetLevel = UniLogger.Level.Warn;
    public UniLogger.Level GameNetLevel = UniLogger.Level.Warn;

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
        UniLogger.GetLogger("P2pNet").LogLevel = P2pNetLevel;
        UniLogger.GetLogger("GameNet").LogLevel = GameNetLevel; 
    }



}
