using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] Image actorImage;
    [SerializeField] Text actorName;
    [SerializeField] Text messageText;
    [SerializeField] RectTransform backgroundBox;
    [SerializeField] Button continueButton;
    [SerializeField] Animator cutsceneAnimator;

    Message[] currenMessages;
    Actor[] currentActors;
    int activeMessage = 0;
    public static bool isActive;
    public static bool isEnd;

    public void OpenDialogue(Message[] messages, Actor[] actors)
    {
        isEnd = false;
        currenMessages = messages;
        currentActors = actors;
        activeMessage = 0;
        isActive = true;
        Debug.Log("Started Conversation! Loaded messages: " + messages.Length);
        DisplayMessage();
        backgroundBox.LeanScale(Vector3.one, 0.5f).setEaseInOutExpo();
        cutsceneAnimator.SetTrigger("Enter");
    }

    void DisplayMessage()
    {
        Message messageToDisplay = currenMessages[activeMessage];
        messageText.text = messageToDisplay.message;

        Actor actorToDisplay = currentActors[messageToDisplay.actorId];
        actorName.text = actorToDisplay.name;
        actorImage.sprite = actorToDisplay.sprite;

        StopAllCoroutines();
        StartCoroutine(TypeSentence(messageToDisplay.message));
        AnimateTextColor();
    }

    public void NextMessage()
    {
        activeMessage++;
        if(activeMessage < currenMessages.Length)
        {
            DisplayMessage();
        }
        else
        {
            Debug.Log("Conversation ended!");
            backgroundBox.LeanScale(Vector3.zero, 0.5f).setEaseInOutExpo();
            isActive = false;
            isEnd = true;
            cutsceneAnimator.SetTrigger("Exit");
        }
    }

    IEnumerator TypeSentence(string sentence)
    {
        messageText.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            messageText.text += letter;
            yield return null;
        }
    }

    void AnimateTextColor()
    {
        LeanTween.textAlpha(messageText.rectTransform, 0, 0);
        LeanTween.textAlpha(messageText.rectTransform, 1, 0.5f);
    }

    // Start is called before the first frame update
    void Start()
    {
        backgroundBox.transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isActive)
        {
            NextMessage();
        }
    }
}
