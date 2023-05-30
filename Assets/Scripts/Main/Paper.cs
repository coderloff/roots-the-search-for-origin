using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Paper : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Text paperText;
    [SerializeField] string message;
    bool isReading;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            animator.SetTrigger("Enter");
            animator.SetTrigger("Read");
            StopAllCoroutines();
            StartCoroutine(TypeSentence(message));
            isReading= true;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isReading)
        {
            Put();
        }    
    }

    public void Put()
    {
        animator.SetTrigger("Exit");
        animator.SetTrigger("Put");
        isReading = false;
    }

    IEnumerator TypeSentence(string sentence)
    {
        paperText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            paperText.text += letter;
            yield return null;
        }
    }
}
