using UnityEngine;

public class BaseState
{
    public virtual void OnEnterState() {}
    public virtual void OnUpdateState() {}
    public virtual void OnFixedUpdateState() {}
    public virtual void OnExitState() {}
}
