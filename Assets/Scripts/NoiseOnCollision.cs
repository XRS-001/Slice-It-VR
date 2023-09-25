using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseOnCollision : MonoBehaviour
{
    private Rigidbody rb;
    private AudioSource audioSource;
    public AudioClip collisionSound;
    public float velocity;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        velocity = rb.velocity.magnitude;
    }
    private void OnCollisionEnter(Collision collision)
    {
        audioSource.PlayOneShot(collisionSound, audioSource.volume = velocity / 15);
    }
}
