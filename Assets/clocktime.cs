using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public Transform hourHand;
    public Transform minuteHand;
    public Transform secondHand;

    void Update()
    {
        // Get the current system time
        System.DateTime currentTime = System.DateTime.Now;

        // Calculate rotation angles for each clock hand
        float hoursAngle = (float)(currentTime.Hour % 12 + currentTime.Minute / 60.0) * 360 / 12;
        float minutesAngle = (float)(currentTime.Minute + currentTime.Second / 60.0) * 360 / 60;
        float secondsAngle = (float)currentTime.Second * 360 / 60;

        // Apply rotations to the clock hands
        // hourHand.rotation = Quaternion.Euler(0, 0, -hoursAngle);
        // minuteHand.rotation = Quaternion.Euler(0, 0, -minutesAngle);
        // secondHand.rotation = Quaternion.Euler(0, 0, -secondsAngle);
        hourHand.localRotation = Quaternion.Euler(hoursAngle, 0f,0f );
        minuteHand.localRotation = Quaternion.Euler(minutesAngle, 0f, 0f);
        secondHand.localRotation = Quaternion.Euler(secondsAngle, 0f,0f );
    }
    
}

