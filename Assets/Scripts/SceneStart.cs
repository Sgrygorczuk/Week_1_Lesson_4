using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneStart : MonoBehaviour
{
    public string nextLevelName; 

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(WaitToEnd());
    }
    
    //Starts the jump and gives player velocity 
    private IEnumerator WaitToEnd()
    {
        yield return new WaitForSeconds(30.1f);
        SceneManager.LoadScene(nextLevelName);
    }
    
}
