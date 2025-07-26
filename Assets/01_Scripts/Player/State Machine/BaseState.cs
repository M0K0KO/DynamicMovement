using UnityEngine;

public class BaseState
{
    protected StateManager manager;

    public BaseState(StateManager manager)
    {
        this.manager = manager;
    }
    
    public virtual void EnterState() {}
    public virtual void UpdateState() {}
    public virtual void FixedUpdateState() {}
    public virtual void TransitionCheck() {}
    public virtual void ExitState() {}
}
