using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayerController : PlayerController
{
    public Transform cameraTransform;

    public float mouseSensitivity = 100.0f;

    public float timeBetweenShoot = 1.0f;

    public static bool alive = true;
    private bool diedNow = true;

    private float xRotation = 0.0f;
    private float yRotation = 0.0f;

    public Timer timer;

    public GameObject laserBeamPrefab;
    public Transform rayOrigin;

    void Start()
    {
        base.Start();
        timer = new Timer();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        timer.Update();

        if (!alive)
        {
            velocity = Vector3.zero;
            if (diedNow)
            {
                diedNow = false;
                //FindObjectOfType<AudioManager>().Play("Dieing");
                AudioManager audioManager = FindObjectOfType<AudioManager>();
                audioManager.transform.position = cameraTransform.position;
                audioManager.Play("Dieing");
            }
            return;
        }
        else
        {
            diedNow = true;
        }

        timeStep = Time.deltaTime;

        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");
        jumpQueued = Input.GetButton("Jump");
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime * 5.0f;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime * 5.0f;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90.0f, 90.0f);
        yRotation += mouseX;

        transform.localRotation = Quaternion.Euler(0.0f, yRotation, 0.0f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0.0f, 0.0f);

        base.Update();

        ShootLocal();
    }

    private void FixedUpdate()
    {
        if (!alive)
        {
            return;
        }

        bool[] keyInputs = new bool[]
        {
            (z>0), (z<0), (x<0), (x>0), jumpQueued
        };

        ClientPacketSender.PlayerMovement(keyInputs);
    }

    private void ShootLocal()
    {
        if (timer.isFinished() && Input.GetKeyDown(KeyCode.Mouse0))
        {
            ClientPacketSender.PlayerShoot(cameraTransform.forward);
            timer.startTimer(timeBetweenShoot);

            float distance = 100.0f;

            if (Physics.Raycast(rayOrigin.position, cameraTransform.forward, out RaycastHit hit))
            {
                distance = hit.distance;
            }

            GameObject laserBeam = Instantiate(laserBeamPrefab);
            laserBeam.transform.position = rayOrigin.position;
            laserBeam.transform.rotation = cameraTransform.rotation;

            laserBeam.transform.localScale = new Vector3(1, 1, distance);
            laserBeam.SetActive(true);
            //AudioManager audioManager = FindObjectOfType<AudioManager>().Play("LaserShot");
            AudioManager audioManager = FindObjectOfType<AudioManager>();
            audioManager.transform.position = cameraTransform.position;
            audioManager.Play("LaserShot");
        }
    }
}
