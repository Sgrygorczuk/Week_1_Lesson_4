using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneEndFlow : MonoBehaviour
{
    //============= Components 
    private TextMeshPro _numberText;    //The Text Mesh Pro that will display collected number of fruit 
    private AudioSource _win;           //Audio source for when it's 5/5
    private AudioSource _lose;          //Audio source for when it's less than that
    private Animator _animator;         //Used to initiate the second animation if player got 5/5 
    
    //=============== Flags 
    public bool _collectedAll = true; //Keeps track if player got all of the fruits 

    //==================================================================================================================
    // Base Functions 
    //==================================================================================================================
    private void Awake()
    {
        _numberText = GameObject.Find($"Fruit Check").transform.Find($"NumberText").GetComponent<TextMeshPro>();
        var data = GameObject.Find($"Data").GetComponent<Data>().GetFruitCollected();
        GoThroughData(data);
        _win = transform.Find($"AudioWin").GetComponent<AudioSource>();
        _lose = transform.Find($"AudioLose").GetComponent<AudioSource>();
        _animator = GameObject.Find($"Scene").GetComponent<Animator>();
        StartCoroutine(CheckIfCollected());
    }

    //==================================================================================================================
    // Set Up
    //==================================================================================================================
    
    //Goes trough all of the collection data, counts how many the player collected and marks if it was all of them 
    private void GoThroughData(IEnumerable<bool> data)
    {
        var counter = 0;
        foreach (var collected in data)
        {
            if (!collected)
            {
                _collectedAll = false;
            }
            else
            {
                counter++;
            }
        }
        
        //Update the text in game to reflect player total count 
        _numberText.SetText(counter + "/5");
    }
    
    //==================================================================================================================
    // Waiting 
    //==================================================================================================================
    
    //Waits till the first part of the cutscene concluded and then based on if the player collected everything 
    //They either get to see the reward doodle or go straight to credits. 
    private IEnumerator CheckIfCollected()
    {
        yield return new WaitForSeconds(9.1f);
        if (_collectedAll)
        {
            _win.Play();
            StartCoroutine(WaitForWinThenCredits());
        }
        else
        {
            _lose.Play();
            StartCoroutine(GoToCredits());
        }
    }

    //Shows the player the reward image and then goes to credits  
    private IEnumerator WaitForWinThenCredits()
    {
        yield return new WaitForSeconds(3.1f);
        _animator.Play($"Scene_End_Win");
        yield return new WaitForSeconds(11.1f);
        SceneManager.LoadScene("Credits");
    }
    
    //Sends player straight to credits 
    private static IEnumerator GoToCredits()
    {
        yield return new WaitForSeconds(4.1f);
        SceneManager.LoadScene("Credits");
    }
}
