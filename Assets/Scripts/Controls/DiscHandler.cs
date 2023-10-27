using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class DiscHandler : MonoBehaviour
{
    

    [SerializeField]
    Transform diskTransform;
    [SerializeField]
    ChangeMaterial upperRadio;
    [SerializeField]
    UpdateChangeMaterial lowerRadio;

    [SerializeField]
    AudioSource radio;

    private bool active = false;

    private bool _readDisk = false;
    private bool _diskReadOnce = false;
    private bool _outDisk = false;

    private float bound = 0.3f;
    private float speed = 0.5f;

    private int battreyPower = 0;



    public void SliderOnChange(float val)
    {

        if(val>=0.9 && battreyPower==2 )
        {
            active = true;
            upperRadio.SetOtherMaterial();
            lowerRadio.SetOtherMaterial();
            if(radio)
            {
                radio.Play();
            }
        }
        else
        {
            active = false;
            upperRadio.SetOriginalMaterial();
            lowerRadio.SetOriginalMaterial();
            if(radio)
            {
                radio.Stop();
            }
        }
    }

    public void IncreaseBatteryPower()
    {
        if(battreyPower<2)
        {
            battreyPower++;
        }
    }

    public void DecreaseBatteryPower()
    {
        if (battreyPower > 0 )
        {
            battreyPower--;
      
        }
    }

    public void IntakeDisk(SelectEnterEventArgs eventArgs)
    {
        eventArgs.interactable.GetComponent<CDInfo>().written = true;
        _readDisk = true;
    }


    private void InitiateDiskRead()
    {
        //Play Some Audio and other functions
        
        Invoke("OutTakeDisk", 3);


    }

    private void OutTakeDisk()
    {

        _outDisk = true;
    }

    private void Update()
    {
        if(active && _readDisk &&  !_diskReadOnce)
        {
            diskTransform.Translate(new Vector3(0.3f * speed * Time.deltaTime, 0, 0),Space.Self);

            if(diskTransform.localPosition.x>=bound)
            {
                _readDisk = false;
                _diskReadOnce = true;
                InitiateDiskRead();

            }
        }

        if(_outDisk && active)
        {
            diskTransform.Translate(new Vector3( -speed * Time.deltaTime, 0, 0), Space.Self);
            if(diskTransform.localPosition.x<=0)
            {
                _outDisk = false;
                _diskReadOnce = false;
            }
        }
    }
}
