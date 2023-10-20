using UnityEngine;
using UnityEngine.Video;

public class CDPlayer : MonoBehaviour
{
    public GameObject cdSlot; // The slot where the CD is inserted.
    public GameObject tvScreen; // The TV screen to display videos.
    private VideoPlayer videoPlayer;

    private void Start()
    {
        videoPlayer = tvScreen.GetComponent<VideoPlayer>();
    }

    public void InsertCD(GameObject cd)
    {
        CDInfo cdInfo = cd.GetComponent<CDInfo>();
        if (cdInfo != null)
        {
            int videoID = cdInfo.videoID;
            LoadVideo(videoID);
        }
    }

    private void LoadVideo(int videoID)
    {
        string videoPath = "Assets/Video/"; // Specify the folder path within Resources where your videos are located.

        switch (videoID)
        {
            case 1:
                videoPath += "vid-01"; // Video1 is the name of the video file without the file extension.
                break;
            case 2:
                videoPath += "vid-02"; // Video2 is the name of the second video file without the file extension.
                break;
            case 3:
                videoPath += "vid-03"; // Video3 is the name of the third video file without the file extension.
                break;
            default:
                Debug.LogError("Video not found for ID " + videoID);
                return;
        }

        // Load the video clip from the Resources folder.
        VideoClip videoClip = Resources.Load<VideoClip>(videoPath);

        if (videoClip != null)
        {
            videoPlayer.clip = videoClip;
            videoPlayer.Play();
        }
        else
        {
            Debug.LogError("Video not found for ID " + videoID);
        }
    }
}
