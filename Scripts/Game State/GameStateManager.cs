using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    Spawn,
    ClockIn,
    TrashPickUp,
    TrashDropOff,
    Walkin,
    VeggiePickUp,
    VeggieDropOff,
    H_Walkin,
    FleshPickUp,
    FleshDropOff,
    FrontCounter
}

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    public State currentState { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        currentState = State.Spawn;
    }

    void Update()
    {
        if (Instance != null)
        {
            gameObject.name = $"[STATE] {Instance.currentState}";
        }
    }

    public void ChangeState(State newState)
    {
        Debug.Log($"State change: {currentState} → {newState}");
        currentState = newState;
    }

    public void SetState(State NewState)
    {
        ChangeState(NewState);
    }
}
