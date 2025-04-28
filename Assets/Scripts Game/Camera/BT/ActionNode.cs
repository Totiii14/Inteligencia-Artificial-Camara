using System;

public class ActionNode : IDesitionNode
{
    private event Action action;

    public ActionNode(Action action)
    {
        this.action = action;
    }

    public void Execute()
    {
        action?.Invoke();
    }
}
