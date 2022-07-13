using System;
using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlow : MonoBehaviour
{
    //======================= Game States
    //Defines if the game is performing automatic action or player is in control 
    private enum GameState
    {
        FadingIn,   //Player Has No Control 
        Player,     //Is Turing to a different side 
    }
    private GameState _currentGame = GameState.FadingIn;     //Current state of the game 

    //===================== Crab Controls 
    private CrabMovement _crabMovementScript;       //Script that controls player movement 
    private CrabSpawning _crabSpawningScript;       //Script that controls player respawning 
    private Rigidbody _crabRigidbody;               //Crabs rigidbody that controls it's physics movement 
    private BoxCollider _feet;
    private BoxCollider _ahead; 
    
    //==================== Transition Animations 
    private Animator _fadeCanvasAnimator;           //Animation used to fade in and out when player dies 
    private Animator _winAnimator;                  //Animation used at the end of the level 
    
    //=================== Camera Movements 
    private CinemachineVirtualCamera _camera;       //The virtual camera 
    private Transform _topCamera;                   //The game object that angles and positions the camera 
    private Transform _cameraLocation;              //Where the virtual camera goes at the end of level 

    //================== Collective Updates 
    private GameObject _fruit;                      //The apple that is used to show if player collected the object or not
    public SkinnedMeshRenderer crabInTransition;    //Render of the crab in the win animation
    private SkinnedMeshRenderer _crabInLevel;       //The render of the crab in the level 
    
    //================= Level To Go To
    public string nextLevelName;                    //Holds name of the next scene 
    public int levelPos;
    [HideInInspector] public GameObject[] levelFruit = new GameObject[5];

    //==================================================================================================================
    // Base Functions 
    //==================================================================================================================

    private void Awake()
    {
        //Connects all of the fruits in the screen 
        levelFruit = new GameObject[5];
        var parent = GameObject.Find($"Top Down Camera").transform.Find($"Camera").Find($"Screen Transition").transform;
        levelFruit[0] = parent.Find($"Fruit_Level_One").gameObject;
        levelFruit[1] = parent.Find($"Fruit_Level_Two").gameObject;
        levelFruit[2] = parent.Find($"Fruit_Level_Three").gameObject;
        levelFruit[3] = parent.Find($"Fruit_Level_Four").gameObject;
        levelFruit[4] = parent.Find($"Fruit_Level_Five").gameObject;

        //Gets the persistent data 
        var data = GameObject.Find($"Data").GetComponent<Data>().GetFruitCollected();
        //Makes sure the fruits are visible in the start of level 
        for(var i = 0; i < data.Length - 1; i++)
        {
            if (data[i]) { levelFruit[i].SetActive(true); }
        }
    }

    //Sets up the game 
    private void Start()
    {
        //Connects Crab components 
        _crabMovementScript = GameObject.Find($"PlayerCrab").GetComponent<CrabMovement>();
        _crabSpawningScript = GameObject.Find($"PlayerCrab").GetComponent<CrabSpawning>();
        _crabRigidbody = GameObject.Find($"PlayerCrab").GetComponent<Rigidbody>();
        _ahead = GameObject.Find($"PlayerCrab").transform.Find($"AheadCollider").GetComponent<BoxCollider>();
        _feet = GameObject.Find($"PlayerCrab").transform.Find($"FeetCollider").GetComponent<BoxCollider>();
        
        //Connects animation components 
        _fadeCanvasAnimator = GameObject.Find($"FadeCanvas").GetComponent<Animator>();
        _winAnimator = GameObject.Find($"Top Down Camera").transform.Find($"Camera").transform
            .Find($"Screen Transition").GetComponent<Animator>();

        //Sets up camera and moves the near clip is -100 so that we don't cut through nearby objects 
        _topCamera = GameObject.Find($"Top Down Camera").transform;
        _camera = GameObject.Find($"Top Down Camera").transform.Find($"CM vcam1")
            .GetComponent<CinemachineVirtualCamera>();
        _camera.m_Lens
            .NearClipPlane = -100;
        _cameraLocation = GameObject.Find($"Top Down Camera").transform.Find($"Camera_End").transform;

        //Connects to the collectible components 
        _fruit = GameObject.Find($"Top Down Camera").transform.Find($"Camera").transform
            .Find($"Fruit Collectible").gameObject;
        _crabInLevel = GameObject.Find($"Collectibles").transform.Find($"Crab").transform.Find($"Crab")
            .GetComponent<SkinnedMeshRenderer>();
        
        //Starts the process of loading in the level, player can't move while we fade in 
        _crabMovementScript.enabled = false;
        StartCoroutine(ToPlayer());
    }

    //Switched between the different states of the game 
    private void Update()
    {
        switch (_currentGame)
        {
            case GameState.FadingIn:
                StartCoroutine(ToPlayer());
                break;
            case GameState.Player:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    //==================================================================================================================
    // Change State 
    //==================================================================================================================


    public void PlayerColliderUpdate(bool state)
    {
        _feet.enabled = state;
        _ahead.enabled = state;
    }
    
    //Make player wait for 2 sec till fade ends and then allows the player to control the crab 
    private IEnumerator ToPlayer()
    {
        yield return new WaitForSeconds(2);
        _crabMovementScript.enabled = true;
        _currentGame = GameState.Player;
    }

    //Takes away control from the player as the screen fades and places the crab at the respawn points at which 
    //point player gets back their control. 
    public IEnumerator Death()
    {
        //Take away the players ability to move, input wise and physics wise. 
        _crabRigidbody.useGravity = false;
        _crabRigidbody.velocity = Vector3.zero;
        _crabMovementScript.enabled = false;
        _crabMovementScript.GoIdle();
        
        //Start the fade animation 
        _fadeCanvasAnimator.Play($"FadeCanvasInAndOut");
        
        //Once we hit peak of dark move the crab and rest their position to face forward 
        yield return new WaitForSeconds(1f);
        _crabMovementScript.ResetOnDeath();
        _crabSpawningScript.MoveToRespawn();
        
        //Return the control back to player once the screen fades back to clear 
        yield return new WaitForSeconds(1.1f);
        _crabRigidbody.useGravity = true;
        _crabMovementScript.enabled = true;
        PlayerColliderUpdate(true);
        _currentGame = GameState.Player;
    }

    
    //The Win Process, moves the camera to a screen of the crabs dancing and play the outro animation 
    public IEnumerator Win()
    {
        //Takes away player control 
        _crabMovementScript.GoIdle();
        _crabMovementScript.enabled = false;
        
        //Fades out to the new camera position 
        _fadeCanvasAnimator.Play("FadeCanvasInAndOut");
        yield return new WaitForSeconds(1f);
        
        GameObject.Find($"Song").GetComponent<AudioSource>().Stop();
        GameObject.Find($"Victory").GetComponent<AudioSource>().Play();
        
        //Updates the UI visual for fruit to disappear and update materials on the transition animation on crab 
        _fruit.GetComponent<MeshRenderer>().enabled = false;
        crabInTransition.materials = _crabInLevel.materials;
        
        //Moves the camera to the new position 
        _topCamera.position = new Vector3(30, 9.6f, 26f);
        _topCamera.eulerAngles = new Vector3(36, 90f, 0f);
        _camera.m_Follow = _cameraLocation;
        _camera.m_LookAt = _cameraLocation;
        
        //Starts the exit animation and moves on to the next scene 
        yield return new WaitForSeconds(5f);
        _winAnimator.Play($"LevelEnd");
        yield return new WaitForSeconds(2.1f);
        SceneManager.LoadScene(nextLevelName);
    }
}
