using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<AudioClip> clips;
    [SerializeField] private Slider audioVolumeSlider;

    public void PlayAudio(int num){
        if(audioSource.isPlaying){
            audioSource.Stop();
        }
        audioSource.PlayOneShot(clips[num]);
    }
    
    public void ChangeVolume(){
        audioSource.volume = audioVolumeSlider.value;
    }
}
