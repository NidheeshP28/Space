using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using TMPro;

public class FlappyBirdController : MonoBehaviour
{

    public GameObject Bird;
    public GameObject PipePrefab;
    public GameObject WingsLeft;
    public GameObject WingsRight;
    public TMP_Text ScoreText;
    public float Gravity = 30f;
    public float Jump = 10f;
    public float PipeSpawnInterval = 2f;
    public float PipesSpeed = 5f;

    private float VerticalSpeed;
    private float PipeSpawnCountdown;
    private GameObject PipesHolder;
    private int PipeCount;
    private int Score;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Reset Score
        Score = 0;
        ScoreText.text = Score.ToString();

        // Reset Pipes
        PipeCount = 0;
        Destroy(PipesHolder);
        PipesHolder = new GameObject("PipesHolder");
        PipesHolder.transform.parent = this.transform;

        // Reset Bird
        VerticalSpeed = 0;
        Bird.transform.position = Vector3.up * 5;

        // Reset Time
        PipeSpawnCountdown = 0;
    }

    // Update is called once per frame
    void Update()
    {

        // STEP 1 - Movement
        VerticalSpeed += -Gravity * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            VerticalSpeed = 0;
            VerticalSpeed += Jump;
        }

        Bird.transform.position += Vector3.up * VerticalSpeed * Time.deltaTime;

        // STEP 2 - Pipes
        PipeSpawnCountdown -= Time.deltaTime;

        if (PipeSpawnCountdown <= 0)
        {
            PipeSpawnCountdown = PipeSpawnInterval;

            // Create Pipe
            GameObject pipe = Instantiate(PipePrefab);
            pipe.transform.parent = PipesHolder.transform;
            pipe.transform.name = (++PipeCount).ToString();

            // Pipe Position
            pipe.transform.position += Vector3.right * 30;
            pipe.transform.position += Vector3.up * Mathf.Lerp(5, 10, Random.value);
        }

        // Move Pipes Left
        PipesHolder.transform.position += Vector3.left * PipesSpeed * Time.deltaTime;

        // STEP 4 - Bird Animation

        // Nose dive
        float speedTo01Range = Mathf.InverseLerp(-10, 10, VerticalSpeed);
        float noseAngle = Mathf.Lerp(-30, 30, speedTo01Range);
        Bird.transform.rotation = Quaternion.Euler(Vector3.forward * noseAngle) * Quaternion.Euler(Vector3.up * 20);

        // Wings
        float flatSpeed = (VerticalSpeed > 0) ? 30 : 5;
        float angle = Mathf.Sin(Time.time * flatSpeed) * 45;
        WingsLeft.transform.localRotation = Quaternion.Euler(Vector3.left * angle);
        WingsRight.transform.localRotation = Quaternion.Euler(Vector3.right * angle);

        // STEP 5 - Score
        foreach (Transform pipe in PipesHolder.transform)
        {
            // When pipe has passed the bird
            if (pipe.position.x < 0)
            {
                int pipeId = int.Parse(pipe.name);
                if (pipeId > Score)
                {
                    Score = pipeId;
                    ScoreText.text = Score.ToString();
                }
            }

            // When pipe is offscreen
            if (pipe.position.x < -30)
            {
                Destroy(pipe.gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        // Reset Game
        Start();
    }
}
