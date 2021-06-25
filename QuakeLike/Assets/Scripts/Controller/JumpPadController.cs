using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPadController : MonoBehaviour
{

    private void OnTriggerEnter(Collider collider)
    {
        //Debug.Log("Passiert trigger");
        //Debug.Log(collider.tag);
        if (collider.CompareTag("Player"))
        {
            Debug.Log("Passiert auch trigger");
            PlayerController playerController = collider.gameObject.GetComponent<PlayerController>();
            playerController.JumpPad();
            //FindObjectOfType<AudioManager>().Play("JumpPad");
            AudioManager audioManager = FindObjectOfType<AudioManager>();
            audioManager.transform.position = playerController.transform.position;
            audioManager.Play("JumpPad");
        }
    }
}
