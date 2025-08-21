using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    public IPlayerState currentState;
    public Animator animator;
    public TPVPlayerMove playerMove;
    public CombatSystem combatSystem;

    void Start()
    {
        SetState(new IdleState());
    }

    void Update()
    {
        currentState.Update();
        currentState.HandleInput();
    }

    public void SetState(IPlayerState newState)
    {
        Debug.Log("ChangingState");
        currentState?.Exit();
        currentState = newState;
        currentState.Enter(this);
    }

    public Animator GetAnimator() => animator;
    public TPVPlayerMove GetPlayerMove() => playerMove;
    public CombatSystem GetCombatSystem() => combatSystem;
}
