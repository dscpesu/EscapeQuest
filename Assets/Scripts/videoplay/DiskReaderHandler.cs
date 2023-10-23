using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Video;

public class DiskReaderHandler : MonoBehaviour
{
    public GameObject tvScreen;
    private VideoPlayer videoPlayer;
    // Start is called before the first frame update
    void Start()
    {
        videoPlayer = tvScreen.GetComponent<VideoPlayer>();
    }

    // Update is called once per frame
    

    public void Reader(SelectEnterEventArgs eventArgs)
    {
        GameObject cd = eventArgs.interactable.gameObject;
 
        bool written = cd.GetComponent<CDInfo>().written;
         
        if (written)
        {
            VideoClip cdVideo = cd.GetComponent<CDInfo>().videoClip;
            videoPlayer.clip = cdVideo;
            videoPlayer.Play();
            
        }
        
    }

    public void Eject(SelectExitEventArgs eventArgs)
    {
        videoPlayer.Stop();
    }

    
}
