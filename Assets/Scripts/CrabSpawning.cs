using System;
using UnityEngine;

public class CrabSpawning : MonoBehaviour
{
    //================== Components 
    private Transform _respawn;         //Current location where the player will respawn if they die
    private GameFlow _gameFlowScript;   //Reference to the gameFlow script that will take away player control during respawn 
    private Animator _animator;         //Controls the player's animations 
    
    //================ Audio 
    private AudioSource _death;
    private AudioSource _checkpoint;

    //Tells us the player is dead, stops from the death triggering again while touching the hit box 
    private bool _isDead;
    
    //==================================================================================================================
    // Base Functions 
    //==================================================================================================================

    //Connects components to necessary game objects 
    private void Start()
    {
        _respawn = GameObject.Find($"Respawn").transform;
        _animator = transform.GetComponent<Animator>();
        _gameFlowScript = GameObject.Find("GameFlow").GetComponent<GameFlow>();

        _death = transform.Find($"Audio").transform.Find($"Death").GetComponent<AudioSource>();
        _checkpoint = transform.Find($"Audio").transform.Find($"Checkpoint").GetComponent<AudioSource>();
    }

    //==================================================================================================================
    // Respawn
    //==================================================================================================================

    //Checks if player hit a death hit box or walked onto a new save point 
    private void OnTriggerEnter(Collider hitBox)
    {
        //Checks if the player died, if so tell Game Flow to stop player from acting 
        if (hitBox.CompareTag($"Death") && !_isDead)
        {
            _isDead = true;
            _gameFlowScript.PlayerColliderUpdate(false);
            _death.Play();
            _animator.Play("MonsterArmature|Death");
            StartCoroutine(_gameFlowScript.Death());
        }
       
        //If player touched a new save spot save that as new location 
        if (!hitBox.CompareTag($"Checkpoint")) return;
        var distance = Vector3.Distance(hitBox.transform.position, transform.position);
        if (!(distance < 0.8f)) return;
        _checkpoint.Play();
        _respawn = hitBox.transform;
    }

    private void OnCollisionEnter(Collision hitBox)
    {
        //Checks if the player died, if so tell Game Flow to stop player from acting 
        if (!hitBox.collider.CompareTag($"Saw") || _isDead) return;
        _isDead = true;
        _gameFlowScript.PlayerColliderUpdate(false);
        _death.Play();
        _animator.Play("MonsterArmature|Death");
        StartCoroutine(_gameFlowScript.Death());
    }
    

    //Moves the player to the respawn point 
    public void MoveToRespawn()
    {
        var transform1 = transform;
        var position = _respawn.position;
        transform1.position = position;
        _isDead = false;
    }
    
}
