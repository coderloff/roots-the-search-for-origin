using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Treasure : MonoBehaviour
{
    [SerializeField] Text passwordText;
    [SerializeField] Animator animator;
    [SerializeField] GameObject passwordUI;
    bool canUI = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckPassword(string password)
    {
        if(password == "1554")
        {
            passwordUI.SetActive(false);
            animator.SetTrigger("Open");
            canUI = false;
        }
        else
        {
            passwordText.text = "";
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!canUI) { return; }
        passwordUI.SetActive(true);
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!canUI) { return; }
        passwordUI.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Win");
        }
    }
}
