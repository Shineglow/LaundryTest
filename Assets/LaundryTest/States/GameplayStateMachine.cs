using System;
using System.Collections;
using System.Collections.Generic;
using LaundryTest;
using UnityEngine;

public class GameplayStateMachine : MonoBehaviour
{
    [field: SerializeField] public EGameplayModes GameplayState { get; private set; }
    
    private GameplayState _actualState;

    private List<GameplayState> _states = new()
    {
        
    };

#if true
    private EGameplayModes gameplayState;
    
    private void OnValidate()
    {
        if (GameplayState != gameplayState)
        {
            _actualState?.Exit();
            _actualState = _states[(int)GameplayState];
        }
    }
#endif
}

public enum EGameplayModes
{
    Normal,
    Building,
}
