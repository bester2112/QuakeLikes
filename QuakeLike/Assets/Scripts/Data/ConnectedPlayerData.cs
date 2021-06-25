using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectedPlayerData : MonoBehaviour
{
    public int id;
    public string username;
    public int health;
    public int maxHealth = 100;
    public int armor;
    public int kills;
    public int deaths;

    public SkinnedMeshRenderer model;
    public MeshRenderer actual;

    private Camera[] cameras;

    public Vector3 oldPosition;
    public Vector3 newPosition;
    public float lerpTime = 0.0f;

    public GameObject laserBeamPrefab;

    public bool[] inputs = new bool[5];

    /// <Animations>
    public Animator animator;
    float velocityZ = 0.0f;
    float velocityX = 0.0f;
    public float acceleration = 2.0f;
    public float deceleration = 2.0f;
    public float maximumWalkVelocity = 0.5f;
    public float maximumRunVelocity = 2.0f;
    float transition = 0.0f;

    int VelocityZHash;
    int VelocityXHash;
    int JumpHash;
    /// </Animations>
    /// 

    private Timer jumpTimer;

    private void Start()
    {
        jumpTimer = new Timer();

        animator = GetComponentInChildren<Animator>();

        VelocityZHash = Animator.StringToHash("Velocity Z");
        VelocityXHash = Animator.StringToHash("Velocity X");
        JumpHash = Animator.StringToHash("isJumping");
    }

    private void Update()
    {
        jumpTimer.Update();

        if (id != ConnectionToServer.instance.myId)
        {
            float lerpValue = lerpTime / Time.fixedDeltaTime;
            lerpValue = Mathf.Clamp(lerpValue, 0.0f, 1.0f);

            transform.position = oldPosition + lerpValue * (newPosition - oldPosition);
            //Debug.Log(oldPosition.ToString() + " " + (lerpValue * newPosition).ToString());
            lerpTime += Time.deltaTime;
            actual.transform.localPosition = newPosition - transform.position;

            if (jumpTimer.isFinished() &&inputs[4])
            {
                jumpTimer.startTimer(0.75f);
                AudioManager audioManager = FindObjectOfType<AudioManager>();
                audioManager.transform.position = transform.position;
                audioManager.Play("Jumping");
            }

            updateAnimation();
        }
    }

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        health = maxHealth;
        cameras = Camera.allCameras;
    }

    public void SetHealth(int _health)
    {
        health = _health;

        if (health <= 0)
        {
            deaths++;
            Die();
        }
    }

    public void Die()
    {
        model.enabled = false;

        if (id == ConnectionToServer.instance.myId)
        {
            Debug.Log("Die");
            //FindObjectOfType<AudioManager>().Play("Dieing");
            LocalPlayerController.alive = false;
            foreach (Camera camera in cameras)
            {
                if (camera.name == "PlayerCamera")
                {
                    camera.enabled = false;
                }
                if (camera.name == "ServerCamera")
                {
                    camera.enabled = true;
                }
            }
        }
    }

    public void Respawn()
    {
        model.enabled = true;
        SetHealth(maxHealth);

        if (id == ConnectionToServer.instance.myId)
        {
            Debug.Log("Respawned");
            LocalPlayerController.alive = true;
            foreach (Camera camera in cameras)
            {
                if (camera.name == "PlayerCamera")
                {
                    camera.enabled = true;
                }
                if (camera.name == "ServerCamera")
                {
                    camera.enabled = false;
                }
            }
        }
    }

    public void Shoot(Vector3 position, Vector3 forward, float distance)
    {
        GameObject laserBeam = Instantiate(laserBeamPrefab);
        laserBeam.transform.position = position;
        laserBeam.transform.forward = forward;

        laserBeam.transform.localScale = new Vector3(1, 1, distance);
        laserBeam.SetActive(true);
        //FindObjectOfType<AudioManager>().Play("LaserShot");
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        audioManager.transform.position = position;
        audioManager.Play("LaserShot");
    }

    public IEnumerator ResetPlayer(float time)
    {
        yield return new WaitForSeconds(time);

        health = maxHealth;
        armor = 0;
        kills = 0;
        deaths = 0;

        //transform.position = Vector3.zero;
        //Respawn();
    }

    private void updateAnimation()
    {
        // input from player
        bool forwardPressed = inputs[0];
        bool backwardPressed = inputs[1];
        bool leftPressed = inputs[2];
        bool rightPressed = inputs[3];
        bool runPressed = true;
        bool crouchPressed = false;
        bool jumpPressed = inputs[4];

        // set the current velocity
        float currentMaxVelocity = runPressed ? maximumRunVelocity : maximumWalkVelocity;

        if (crouchPressed)
        {
            transition += Time.deltaTime * acceleration;
            if (transition > 1.0f)
            {
                transition = 1.0f;
            }
        }
        else
        {
            transition -= Time.deltaTime * deceleration;
            if (transition < 0.0f)
            {
                transition = 0.0f;
            }
        }

        animator.SetLayerWeight(animator.GetLayerIndex("Crouch Layer"), transition);

        // handle changes in velocity
        changeVelocity(forwardPressed, leftPressed, rightPressed, runPressed, currentMaxVelocity, backwardPressed);
        lockOrResetVelocity(forwardPressed, leftPressed, rightPressed, runPressed, currentMaxVelocity, backwardPressed);

        animator.SetFloat(VelocityZHash, velocityZ);
        animator.SetFloat(VelocityXHash, velocityX);
        animator.SetBool(JumpHash, jumpPressed);
    }

    void changeVelocity(bool forwardPressed, bool leftPressed, bool rightPressed, bool runPressed, float currentMaxVelocity, bool backwardPressed)
    {
        if (forwardPressed && velocityZ < currentMaxVelocity)
        {
            velocityZ += Time.deltaTime * acceleration;
        }

        if (backwardPressed && velocityZ > -currentMaxVelocity)
        {
            velocityZ -= Time.deltaTime * acceleration;
        }

        if (leftPressed && velocityX > -currentMaxVelocity)
        {
            velocityX -= Time.deltaTime * acceleration;
        }

        if (rightPressed && velocityX < currentMaxVelocity)
        {
            velocityX += Time.deltaTime * acceleration;
        }

        // decrease 
        if (!forwardPressed && velocityZ > 0.0f)
        {
            velocityZ -= Time.deltaTime * deceleration;
        }

        if (!backwardPressed && velocityZ < 0.0f)
        {
            velocityZ += Time.deltaTime * deceleration;
        }

        if (!leftPressed && velocityX < 0.0f)
        {
            velocityX += Time.deltaTime * deceleration;
        }

        if (!rightPressed && velocityX > 0.0f)
        {
            velocityX -= Time.deltaTime * deceleration;
        }
    }

    // handles reset and locking of velocity 
    void lockOrResetVelocity(bool forwardPressed, bool leftPressed, bool rightPressed, bool runPressed, float currentMaxVelocity, bool backwardPressed)
    {
        /*if (!forwardPressed && velocityZ < 0.0f)
        {
            velocityZ = 0.0f;
        }*/

        if (!forwardPressed && !backwardPressed && velocityZ != 0.0f && (velocityZ > -0.05f && velocityZ < 0.05f))
        {
            velocityZ = 0.0f;
        }

        /**/

        // reset velocityX
        if (!leftPressed && !rightPressed && velocityX != 0.0f && (velocityX > -0.05f && velocityX < 0.05f))
        {
            velocityX = 0.0f;
        }

        if (forwardPressed && runPressed && velocityZ > currentMaxVelocity)
        {
            velocityZ = currentMaxVelocity;
        }
        // decelerate to the maximum walk velocity
        else if (forwardPressed && velocityZ > currentMaxVelocity)
        {
            velocityZ -= Time.deltaTime * deceleration;
            // round to the currentMaxVelocity if within offset
            if (velocityZ > currentMaxVelocity && velocityZ < (currentMaxVelocity + 0.05f))
            {
                velocityZ = currentMaxVelocity;
            }
        }
        // round to the currentMacVelocity if within offset
        else if (forwardPressed && velocityZ < currentMaxVelocity && velocityZ > (currentMaxVelocity - 0.05f))
        {
            velocityZ = currentMaxVelocity;
        }

        /***/

        if (backwardPressed && runPressed && velocityZ < -currentMaxVelocity)
        {
            velocityZ = -currentMaxVelocity;
        }
        // decelerate to the maximum walk velocity
        else if (backwardPressed && velocityZ < -currentMaxVelocity)
        {
            velocityZ += Time.deltaTime * deceleration;
            // round to the currentMaxVelocity if within offset
            if (velocityZ < -currentMaxVelocity && velocityZ > (-currentMaxVelocity - 0.05f))
            {
                velocityZ = -currentMaxVelocity;
            }
        }
        // round to the currentMacVelocity if within offset
        else if (backwardPressed && velocityZ > -currentMaxVelocity && velocityZ < (-currentMaxVelocity + 0.05f))
        {
            velocityZ = -currentMaxVelocity;
        }

        /***/

        // locking left
        if (leftPressed && runPressed && velocityX < -currentMaxVelocity)
        {
            velocityX = -currentMaxVelocity;
        }
        // decelerate to the maximum walk velocity
        else if (leftPressed && velocityX < -currentMaxVelocity)
        {
            velocityX += Time.deltaTime * deceleration;
            // round to the currentMaxVelocity if withing offset
            if (velocityX < -currentMaxVelocity && velocityX > (-currentMaxVelocity - 0.05f))
            {
                velocityX = -currentMaxVelocity;
            }
        }
        // round to the currentMaxVelocity if withing offset
        else if (leftPressed && velocityX > -currentMaxVelocity && velocityX < (-currentMaxVelocity + 0.05f))
        {
            velocityX = -currentMaxVelocity;
        }

        // locking right
        if (rightPressed && runPressed && velocityX > currentMaxVelocity)
        {
            velocityX = currentMaxVelocity;
        }
        // decelerate to the maximum walk velocity
        else if (rightPressed && velocityX > currentMaxVelocity)
        {
            velocityX -= Time.deltaTime * deceleration;
            // round to the currentMaxVelocity if within offset
            if (velocityX > currentMaxVelocity && velocityX < (currentMaxVelocity + 0.05f))
            {
                velocityX = currentMaxVelocity;
            }
        }
        // round to the currentMaxVelocity if within offset
        else if (rightPressed && velocityX < currentMaxVelocity && velocityX > (currentMaxVelocity - 0.05f))
        {
            velocityX = currentMaxVelocity;
        }
    }
}
