using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerDummy : MonoBehaviour
{
    public bool MyIsAlive { get; set; } = true;

    [SerializeField]
    private float velocity = 10f;

    private float myStamina = 100;

    private void Update()
    {
        // if isAlive
        Fly();
    }
    private void Fly() 
    {
        transform.Translate((transform.forward * velocity * Time.deltaTime) + (transform.right * velocity * Time.deltaTime * Input.GetAxis("Horizontal")) + (transform.up * velocity * Time.deltaTime * Input.GetAxis("Vertical")));

    }
    private void Run()
    {

    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Obstacle"))
        {
            MyIsAlive = false;
        }
        // if colliding with nut -> Add score
        // if colliding with enemy play animation, respawn
        // if entering running section -> start running instead of flying
        // if entering checkpoint send info to GameManager
    }

    private void OnCollisionEnter(Collision collision)
    {
        // if colliding with obstacle respawn
    }
}
