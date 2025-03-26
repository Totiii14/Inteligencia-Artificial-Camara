using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionNode : IDesitionNode
{
    private Action action;

    public ActionNode(Action action)
    {
        this.action = action;
    }

    public void Execute()
    {
        action();
    }
}
