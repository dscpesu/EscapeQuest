using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscHandler : MonoBehaviour
{
    [SerializeField]
    GameObject diskAttach;

    [SerializeField]
    Transform diskTransform;


    private bool _readDisk = false;
    private bool _diskReadOnce = false;
    private bool _outDisk = false;

    private float bound = 0.3f;
    private float speed = 0.5f;


    public void IntakeDisk()
    {
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
        if(_readDisk &&  !_diskReadOnce)
        {
            diskTransform.Translate(new Vector3(0.3f * speed * Time.deltaTime, 0, 0),Space.Self);

            if(diskTransform.localPosition.x>=bound)
            {
                _readDisk = false;
                _diskReadOnce = true;
                InitiateDiskRead();

            }
        }

        if(_outDisk)
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
