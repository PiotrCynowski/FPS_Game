using System;

public class PlayerState
{
    public enum PlayerDefinedStates
    {
        Idle,
        Walking,
        Jumping
    }

    public static event Action<PlayerDefinedStates> OnPlayerStateChanged;
    private PlayerDefinedStates currentState = PlayerDefinedStates.Idle;

    public PlayerDefinedStates CurrentState
    {
        get { return currentState; }
        set
        {
            if (currentState != value)
            {
                currentState = value;
                OnPlayerStateChanged?.Invoke(currentState);
            }
        }
    }
}