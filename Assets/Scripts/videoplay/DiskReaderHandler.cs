using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Video;

public class DiskReaderHandler : MonoBehaviour
{
    public GameObject tvScreen;
    private VideoPlayer videoPlayer;
    private AudioSource audioPlayer;

    // Start is called before the first frame update
    void Start()
    {
        videoPlayer = tvScreen.GetComponent<VideoPlayer>();
        audioPlayer = tvScreen.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    

    public void Reader(SelectEnterEventArgs eventArgs)
    {
        GameObject cd = eventArgs.interactable.gameObject;
 
        bool written = cd.GetComponent<CDInfo>().written;
         
        if (written)
        {
            VideoClip cdVideo = cd.GetComponent<CDInfo>().videoClip;
            if(cdVideo)
            {
            videoPlayer.clip = cdVideo;
            videoPlayer.Play();
            }
            else
            {
                AudioClip cdAudio = cd.GetComponent<CDInfo>().audioClip;
                audioPlayer.clip = cdAudio;
                audioPlayer.Play();
            }
            
        }
        
    }

    public void Eject(SelectExitEventArgs eventArgs)
    {
        videoPlayer.Stop();
        audioPlayer.Stop();
    }

    
}
