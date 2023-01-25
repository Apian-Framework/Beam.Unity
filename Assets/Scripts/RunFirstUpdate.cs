using System.Collections;
using System.Collections.Generic;
using BeamGameCode;
using UnityEngine;

public class RunFirstUpdate : MonoBehaviour
{
    // Deletes itself after 1st update.
    // All of the other "permanent" classes are in place by the time ANY GameObject's
    // Update() is called, so this is a good place to do "do this once on the first frame"
    // stuff without having to check a "did I do the thing?" boolean on every frame after, forever.
    void Start()
    {

    }


    void Update()
    {
        BeamMain mainObj = BeamMain.GetInstance();

        mainObj.beamApp.Start(BeamModeFactory.kSplash); // Start the game code

        Object.Destroy(gameObject);
    }

}
