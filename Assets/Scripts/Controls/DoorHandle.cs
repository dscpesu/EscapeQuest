using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHandle : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    GameObject door;

    HingeJoint _hingeJoint;
    JointMotor _motor;
    XRKnob xrKnob;
    private void Start()
    {
        xrKnob = GetComponent<XRKnob>();
        _hingeJoint = door.GetComponent<HingeJoint>();
        _motor = _hingeJoint.motor;
    }
    public void HandleDoorHandle()
    {
          
        if(xrKnob && xrKnob.maximum == 90 && xrKnob.Value>0.3)
        {
            _motor.targetVelocity = 20;
            _motor.force = 100;
            _hingeJoint.motor = _motor;
        }
    }
}
