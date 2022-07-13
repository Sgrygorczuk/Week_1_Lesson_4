using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuFlow : MonoBehaviour
{
    private bool _loaded;
    
    // Update is called once per frame
    public void Update()
    {
        if (!Input.GetButtonDown("Jump") || _loaded) return;
        StartCoroutine(Load());
    }

    private IEnumerator Load()
    {   
        _loaded = true;
        GameObject.Find($"FadeCanvas").GetComponent<Animator>().Play("FadeCanvasIn");
        yield return new WaitForSeconds(2.1f);
        
        SceneManager.LoadScene("CutScene_Start");
    }
}
