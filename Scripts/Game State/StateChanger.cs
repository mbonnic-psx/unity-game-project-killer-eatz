using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateChanger : MonoBehaviour
{
    public State desiredState;

    public void ChangeGameState()
    {
        GameStateManager.Instance.ChangeState(desiredState);
    }
}
