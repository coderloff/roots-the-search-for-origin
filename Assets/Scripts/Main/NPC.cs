using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    DialogueTrigger trigger;

    void Start()
    {
        trigger = GetComponent<DialogueTrigger>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            trigger.StartDialogue();
        }
    }
}
