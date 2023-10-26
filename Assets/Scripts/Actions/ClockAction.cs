using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockAction : MonoBehaviour
{
    [SerializeField] Transform hourHand;
    [SerializeField] Transform minuteHand;
    [SerializeField] Transform secondHand;
    [SerializeField] bool startsWithBattery;

    [SerializeField] float clockDuration=1;
   
    

    public void StartClock()
    {
        InvokeRepeating("TickSecond", 0.0f, clockDuration);
        InvokeRepeating("TickMinute", 60.0f, clockDuration * 60);
        InvokeRepeating("TickHour", 0.0f, clockDuration * 3600);
    }

    public void StopClock()
    {
        CancelInvoke();
    }
    private void TickSecond()
    {
        int currentSecond = (int) secondHand.localRotation.x;
        
        currentSecond += 6;
        secondHand.Rotate(0,(float)(currentSecond),0,Space.Self);
      
    }
    private void TickMinute()
    {
        int currentMinute = (int)minuteHand.localRotation.x;

        currentMinute += 6;
        minuteHand.Rotate(0, (float)(currentMinute), 0, Space.Self);

    }
    private void TickHour()
    {
        int currentHour = (int)hourHand.localRotation.x;

        currentHour += 6;
        hourHand.Rotate(0, (float)(currentHour), 0, Space.Self);

    }



}



