using UnityEngine;

public class Data : MonoBehaviour
{
    //Variables 
    private static Data _instance; //Is the instance of the object that will show up in each scene 
    public bool[] fruitCollectd = new[] { false, false, false, false, false }; //Start data of the object 

    //==================================================================================================================
    // Base Functions 
    //==================================================================================================================
    
    //Creates the object, if one already has been created in another scene destroy this one and the make a new one 
    private void Awake()
    {
        //Checks if there already exits a copy of this, if does destroy it and let the new one be created 
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    //==================================================================================================================
    // Data Update Methods 
    //==================================================================================================================

    //If player collected a fruit this will update the value to true
    public void CollectFruit(int pos)
    {
        fruitCollectd[pos] = true;
    }

    //Returns the list of collected fruits, used in start of each level and check in the end scene 
    public bool[] GetFruitCollected()
    {
        return fruitCollectd;
    }
}
