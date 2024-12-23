using UnityEngine;

public abstract class BaseState
{
    protected BaseCharacter _controller;

    public virtual void Enter(BaseCharacter controller)
    {
        _controller = controller;
    }

    public virtual void Exit() { }

    public virtual void Update() { }

    public virtual void FixedUpdate() { }
}
