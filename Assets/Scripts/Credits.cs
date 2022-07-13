using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    //========== Variable 
    private bool _loaded; //Tells us if the fade animation has been activated 

    //==================================================================================================================
    // Base Functions 
    //==================================================================================================================

    //Starts the count down till we go to another scene 
    private void Awake()
    {
        StartCoroutine(PreLoad());
    }

    //Listen for player to hit space and allowing the fade to only occur once 
    public void Update()
    {
        if (!Input.GetButtonDown("Jump") || _loaded) return;
        StartCoroutine(Load());
    }

    //==================================================================================================================
    // Waiting Functions 
    //==================================================================================================================

    
    //Starts when the scene starts, waits for 15 sec to go to Main Menu 
    private IEnumerator PreLoad()
    {
        yield return new WaitForSeconds(15.1f);
        if (_loaded) yield break;
        StartCoroutine(Load());
    }
    
    //User activated fade, starts the fade animation early and then goes to Main Menu 
    private IEnumerator Load()
    {   
        _loaded = true;
        GameObject.Find($"FadeCanvas").GetComponent<Animator>().Play("FadeCanvasIn");
        yield return new WaitForSeconds(2.1f);
        SceneManager.LoadScene("MainMenu");
    }
}
