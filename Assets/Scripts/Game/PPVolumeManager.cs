using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PPVolumeManager : MonoBehaviour
{
    private Volume volume;
    private DepthOfField depthOfField;

    private void Start() {
        volume = GameObject.Find("Global Volume").GetComponent<Volume>();
        volume.profile.TryGet(out depthOfField);
    }

    public void TurnOnDepthOfField(){
        depthOfField.mode.Override(DepthOfFieldMode.Gaussian);
        print(depthOfField.mode);

    }

    public void TurnOffDepthOfField(){
        depthOfField.mode.Override(DepthOfFieldMode.Off);
        print(depthOfField.mode);
    }
}
