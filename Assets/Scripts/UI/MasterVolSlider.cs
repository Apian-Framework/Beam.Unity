using System.Collections;
using System.Collections.Generic;
using UniLog;
using UnityEngine;
using UnityEngine.UI;

public class MasterVolSlider : MonoBehaviour
{
    public Slider slider;
    UniLogger logger;


    void OnEnable()
    {
        logger = UniLogger.GetLogger("UI");

        slider.value = BeamMain.GetInstance().driverSettings.masterVolume;
        logger.Verbose($"MasterVolSlider.Start: Val: {slider.value}");
    }

    // Update is called once per frame
    void Update()
    {

    }

}
