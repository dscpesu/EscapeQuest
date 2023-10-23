using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NumpadHandler : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    string passcode = "";

    [SerializeField]
    TextMeshProUGUI displayContent;

    [SerializeField]
    GameObject bulbObject;

    [SerializeField]
    AudioSource audioSource;

    [SerializeField]
    GameObject doorObject;

    [SerializeField]
    GameObject doorHandle;

    string code = "";

    HingeJoint _hingeJoint;
    XRKnob xrKnob;

    private void Start()
    {
        if(audioSource==null)
        audioSource = GetComponent<AudioSource>();


        xrKnob = doorHandle.GetComponent<XRKnob>();
        xrKnob.maximum = 15;

        _hingeJoint = doorObject.GetComponent<HingeJoint>();
        JointLimits jointLimit = _hingeJoint.limits;
        jointLimit.max = 0;
        _hingeJoint.limits = jointLimit;


    }

    public void HandleButtonPress(int buttonValue)
    {
        if(buttonValue==-1)
        {
            CheckCode();
        }
        else if(buttonValue==-2)
        {
            if (code.Length > 0)
                code = code.Remove(code.Length - 1, 1);
        }
        else
        {
            if (code.Length < passcode.Length)
                code += buttonValue.ToString();
        }
        PlayAudio();
        UpdateText();

    }

     void PlayAudio()
    {
        
        if(audioSource!=null)
        {
            audioSource.Play();
        }
    }

    private void  UpdateText()
    {
        displayContent.text = code;
    }

    private void CheckCode()
    {
        if (code == passcode)
        {
      
            if(bulbObject!=null)
            {
                bulbObject.GetComponent<ChangeMaterial>().SetOtherMaterial();
            }
                xrKnob.maximum = 90;
            _hingeJoint.useMotor = true;
            JointLimits jointLimit = _hingeJoint.limits;

            jointLimit.max = 130;
            _hingeJoint.limits = jointLimit;

        }
        else
        {
            Debug.Log("False");
            bulbObject.GetComponent<ChangeMaterial>().SetOriginalMaterial();
        }
        code = "";
        UpdateText();

    }
}
