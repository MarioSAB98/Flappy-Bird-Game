using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TapController : MonoBehaviour
{

    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerScored;
    public static event PlayerDelegate OnPlayerDied;

    public float tapForce = 10;
    public float tiltSmooth = 5;
    public Vector3 startPosition;

    Rigidbody2D RigidBody;
    Quaternion downRotation;
    Quaternion forwardRotation;

    GameManager game;

    void Start()
    {
        RigidBody = GetComponent<Rigidbody2D>();
        downRotation = Quaternion.Euler(0, 0, -90);
        forwardRotation = Quaternion.Euler(0, 0, 35);
        game = GameManager.instance;
    }

    void OnEnable()
    {
        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    void OnDisable()
    {
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    void OnGameStarted()
    {
        RigidBody.velocity = Vector3.zero;
        RigidBody.simulated = true;
    }

    void OnGameOverConfirmed()
    {
        transform.position = startPosition;
        transform.rotation = Quaternion.identity;
    }


    void Update()
    {
        if(game.gameOver)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            transform.rotation = forwardRotation;
            RigidBody.velocity = Vector3.zero;
            RigidBody.AddForce(Vector2.up * tapForce, ForceMode2D.Force);
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, downRotation, tiltSmooth * Time.deltaTime);

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "ScoreZone")
        {
            //add a point
            OnPlayerScored();
            //play a sound

        }

        if (collision.gameObject.tag == "DeadZone")
        {
            RigidBody.simulated = false;
            //stop game
            OnPlayerDied();
            //play a sound
        }
    }

}
