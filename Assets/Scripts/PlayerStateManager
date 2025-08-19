using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    private IPlayerState currentState;
    private Animator animator;
    private TPVPlayerMove playerMove;
    private CombatSystem combatSystem;

    void Awake()
    {
        animator = GetComponent<Animator>();
        playerMove = GetComponent<TPVPlayerMove>();
        combatSystem = GetComponent<CombatSystem>();
    }

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
        currentState?.Exit();
        currentState = newState;
        currentState.Enter(this);
    }

    public Animator GetAnimator() => animator;
    public TPVPlayerMove GetPlayerMove() => playerMove;
    public CombatSystem GetCombatSystem() => combatSystem;
}
