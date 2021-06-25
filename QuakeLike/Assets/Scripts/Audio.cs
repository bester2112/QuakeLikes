using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] audioClips;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D)) && audioSource.isPlaying == false)
        {
            FindObjectOfType<AudioManager>().Play("Running"); // Name is not the file. Name is the set name in the AudioManager
            /*audioSource.pitch = Random.Range(0.7f, 1.3f);
            audioSource.volume = Random.Range(0.5f, 1.0f);
            audioSource.PlayOneShot(audioClips[0]);*/
        }

       // if (cc.velocity.magnitude > 2.0f && audioSource.isPlaying == false)
        {

        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            audioSource.pitch = Random.Range(0.7f, 1.3f);
            audioSource.volume = 1.0f;
            audioSource.PlayOneShot(audioClips[1]);
        } 
    }
}
