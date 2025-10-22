using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Numerics;

public class DialogueManager : MonoBehaviour
{

    [Header("GUI")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Image textBox;

    [Header("Dialogue Settings")]
    public Dialogue startingDialogue;

    [Header("Camera Reference")]
    public Camera cam;
    public int FOV;

    private Queue<string> sentences;
    private bool isDialogueActive = false; // Track if dialogue is active
    private GameObject trackingObject;
    private float dialogueInputDelay = 0.2f;
    private float dialogueTimer = 0f;
    private bool isTyping = false;



    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();

        if (startingDialogue != null)
        {
            StartDialogue(startingDialogue);
        }

        cam = Camera.main;
    }

    void Update()
    {
        if (isDialogueActive)
        {
            dialogueTimer += Time.deltaTime;

            if (dialogueTimer >= dialogueInputDelay && Input.GetKeyDown(KeyCode.E))
            {
                DisplayNextSentence();
            }
        }

    }

    public void StartDialogue(Dialogue dialogue, GameObject senderObject = null)
    {

        trackingObject = senderObject;
        textBox.gameObject.SetActive(true);
        isDialogueActive = true;

        dialogueTimer = 0f;

        if (trackingObject != null)
        {
            ItemID itemIdentification = trackingObject.GetComponent<ItemID>();
            if (itemIdentification != null && itemIdentification.itemID == "Person")
            {
                cam.fieldOfView = FOV;
                cam.transform.LookAt(trackingObject.transform.position);
            }
        }

        nameText.text = dialogue.name;

        sentences.Clear();
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();

    }

    public bool IsDialougeFinished()
    {
        return !isDialogueActive && !isTyping;
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        dialogueText.text = sentence;
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence, 0.05f));
    }

    IEnumerator TypeSentence(String sentence, float delayTime)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(delayTime);
        }

        isTyping = false;
    }

    void EndDialogue()
    {
        textBox.gameObject.SetActive(false);
        isDialogueActive = false; // Disable dialogue tracking

        if (trackingObject != null)
        {
            ItemID itemIdentification = trackingObject.GetComponent<ItemID>();
            if (itemIdentification != null && itemIdentification.itemID == "Person") // Replace with the ID you want
            {
                cam.fieldOfView = 90;
            }
        }

        trackingObject = null;

        Debug.Log("End of conversation");
    }

    public bool IsDialogueActive => isDialogueActive;
}
