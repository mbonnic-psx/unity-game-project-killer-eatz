using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class LoganAI : MonoBehaviour
{
    [Header("Waypoints")]
    public List<Transform> wayPoints;
    public int currentWaypointIndex = 0;
    public int triggerWaypointIndex = 0; // The index of the waypoint where trigger logic should fire

    [Header("Logan")]
    public Animator animator;
    public Billboard billboard;
    public DialogueManager dialogueManager;

    [Header("Reference")]
    public PlayerMovement playerMovement;
    public GameObject frontCounterBorder;
    public StateChanger stateChanger;
    public FrontCounterSequence frontCounterSequence;

    private bool isInConversation = false;
    private bool hasTriggeredWaypoint = false;
    private bool isLoganDialogue = false;
    private bool flag = false;
    private String currentAnim = "";
    NavMeshAgent navMeshAgent;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        playerMovement.EnableMovement(true);
        ChangeAnimation("Idle");
        billboard.DisableLookAt();
        frontCounterBorder.SetActive(false);
    }

    void Update()
    {
        if (dialogueManager != null && dialogueManager.IsDialogueActive)
        {
            ChangeAnimation("Idle");
            playerMovement.EnableMovement(false);
            billboard.EnableLookAt();
            frontCounterBorder.SetActive(true);
            isInConversation = true;
        }

        if (!dialogueManager.IsDialogueActive && isInConversation == true)
        {
            billboard.DisableLookAt();
            ChangeAnimation("Walking");
            Walking();
        }

        if (hasTriggeredWaypoint == true && flag == false)
        {
            ChangeAnimation("Idle");
            stateChanger.desiredState = State.FrontCounter;
            stateChanger.ChangeGameState();
            frontCounterSequence.enabled = true;
            flag = true;
        }

    }

    void Walking()
    {
        if (wayPoints.Count == 0)
            return;

        float distanceToWayPoint = Vector3.Distance(wayPoints[currentWaypointIndex].position, transform.position);

        if (distanceToWayPoint <= 3f)
        {
            // Only trigger once when reaching the specific waypoint
            if (currentWaypointIndex == triggerWaypointIndex && !hasTriggeredWaypoint)
            {
                Debug.Log("Logan has reached the Front Counter waypoint.");
                hasTriggeredWaypoint = true;

                // Do the same logic that was in OnTriggerEnter
                frontCounterBorder.SetActive(true);
                playerMovement.EnableMovement(true);
            }

            // Move to next waypoint
            currentWaypointIndex = (currentWaypointIndex + 1) % wayPoints.Count;
        }

        navMeshAgent.SetDestination(wayPoints[currentWaypointIndex].position);
    }

    private void ChangeAnimation(string animation, float crossfade = 0.2f)
    {
        if (currentAnim != animation)
        {
            currentAnim = animation;
            animator.CrossFade(animation, crossfade); // Transitions the current animation to the next animation
        }
    }
    
    public void SetLoganDialogueActive(bool active)
    {
        isLoganDialogue = active;
    }
}
