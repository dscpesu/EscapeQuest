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


    string code = "";

    private void Start()
    {
        if(audioSource==null)
        audioSource = GetComponent<AudioSource>();
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
            
            //audioSource.pitch = Mathf.Abs(value / 10);
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
            Debug.Log("True");
            if(bulbObject!=null)
            {
                bulbObject.GetComponent<ChangeMaterial>().SetOtherMaterial();
            }
            //What to do
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
