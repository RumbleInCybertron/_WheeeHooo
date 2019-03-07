﻿using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 2000f;
    [SerializeField] float levelLoadDelay = 2.5f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip death;
    [SerializeField] AudioClip levelUp;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem levelUpParticles;
    [SerializeField] ParticleSystem deathParticles;

    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State { Alive, Dying, Transcending};
    State state = State.Alive;

    bool collisionsAreEnabled = true;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
        if(Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }
    }

    private void RespondToDebugKeys()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsAreEnabled = !collisionsAreEnabled; // toggle
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(state != State.Alive || !collisionsAreEnabled) { return; }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                // do nothing
                break;
            case "Finish":
                StartLevelUpSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }

    private void StartLevelUpSequence()
    {
        state = State.Transcending;
        audioSource.PlayOneShot(levelUp);
        levelUpParticles.Play();
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    private void StartDeathSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        deathParticles.Play();
        Invoke("LoadFirstLevel", levelLoadDelay);
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(1); //TODO allow for more than two levels
    }

    private void RespondToThrustInput()
    {
        float thrustThisFrame = mainThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.Space))    // Can thrust while rotating
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.forward * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying)      // So it doesn't layer
        {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }

    private void RespondToRotateInput()
    {
        rigidBody.freezeRotation = true; // take manual control of rotation

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.up * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.down * rotationThisFrame);
        }

        rigidBody.freezeRotation = false; // resume physics control of rotation
    }
}
