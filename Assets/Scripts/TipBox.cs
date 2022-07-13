using TMPro;
using UnityEngine;

public class TipBox : MonoBehaviour
{
    //============= Components 
    private GameObject _textGameObject;     //Turns on and off the tip box
    private AudioSource _tipAudio;
    
    //============= Unique Text 
    public string tipText;                  //Tip information 
    
    //==================================================================================================================
    // Base Functions 
    //==================================================================================================================
    private void Start()
    {
        //Gets the component 
        _textGameObject = transform.Find($"Text Plane").gameObject;
        _textGameObject.SetActive(false);

        _tipAudio = GetComponent<AudioSource>();

        //Sets the text 
        transform.Find($"Text Plane").Find($"Text").GetComponent<TextMeshPro>().SetText(tipText);
    }

    //Checks if player has stepped on the block
    private void OnTriggerEnter(Collider hitBox)
    {
        if (!hitBox.CompareTag($"Player")) return;
        _tipAudio.Play();
        _textGameObject.SetActive(true);
    }

    //Checks if player left the block 
    private void OnTriggerExit(Collider hitBox)
    {
        if (!hitBox.CompareTag($"Player")) return;
        _textGameObject.SetActive(false);
    }
}
