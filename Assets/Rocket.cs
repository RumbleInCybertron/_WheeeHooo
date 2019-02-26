﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    Rigidbody rigidBody;
    AudioSource rocketAudio;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        rocketAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
    }

    private void ProcessInput()
    {
        if (Input.GetKey(KeyCode.Space))    // Can thrust while rotating
        {
            rigidBody.AddRelativeForce(Vector3.up);
            if(!rocketAudio.isPlaying)      // So it doesn't layer
            {
                rocketAudio.Play();
            }

        }
        else
        {
            rocketAudio.Stop();
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward);
            if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.Space))
            {
                print("Thrusting and rotating left");
            }
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.back);
            if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.Space))
            {
                print("Thrusting and rotating right");
            }
        }
    }
}
