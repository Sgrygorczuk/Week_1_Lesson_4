using System;
using UnityEngine;

public class CrabCollector : MonoBehaviour
{
    //============= Components 
    private MeshRenderer _appleMeshRenderer;      //The display apple that will have it's mesh rendered update 
    private Material[] _materials;                //The materials that the apple will receive 
    private GameFlow _gameFlowScript;             //Game Flow to update when player touched a crab 

    //========================= Audio 
    private AudioSource _appleAudio;
    private AudioSource _crabAudio;

    //==================================================================================================================
    // Base Functions 
    //==================================================================================================================

    //Connects the components to 
    private void Start()
    {
        _materials = GameObject.Find($"Collectibles").transform.Find($"Fruit").GetComponent<MeshRenderer>().materials;
        _gameFlowScript = GameObject.Find($"GameFlow").GetComponent<GameFlow>();
        _appleMeshRenderer = GameObject.Find($"Top Down Camera").transform.Find($"Camera").transform.Find($"Fruit Collectible").GetComponent<MeshRenderer>();
        
        _appleAudio = transform.Find($"Audio").transform.Find($"CollectApple").GetComponent<AudioSource>();
        _crabAudio = transform.Find($"Audio").transform.Find($"CrabCollector").GetComponent<AudioSource>();
    }

    //==================================================================================================================
    // Trigger
    //==================================================================================================================
    private void OnTriggerEnter(Collider hitBox)
    {
        //If we walk into a crab start the win process 
        if (hitBox.CompareTag($"Crab"))
        {
            var dist = Vector3.Distance(hitBox.transform.position, transform.position);
            if (!(dist < 1f)) return;
            _crabAudio.Play();
            StartCoroutine(_gameFlowScript.Win());
        }
        
        //If we walk into the fruit update the UI materials and destroy the level fruit 
        if (!hitBox.CompareTag($"Fruit")) return;
        var distance = Vector3.Distance(hitBox.transform.position, transform.position);
        if (!(distance < 1)) return;
        
        //
        var pos = GameObject.Find("GameFlow").GetComponent<GameFlow>().levelPos;
        GameObject.Find($"Data").GetComponent<Data>().CollectFruit(pos);
        GameObject.Find("GameFlow").GetComponent<GameFlow>().levelFruit[pos].SetActive(true);
        
        _appleAudio.Play();
        _appleMeshRenderer.materials = _materials;
        Destroy(hitBox.gameObject);
    }
    
}
