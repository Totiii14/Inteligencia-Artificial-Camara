using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionNodeClass : IDesitionNodeExample
{
    private Action action;

    public ActionNodeClass(Action action)
    {
        this.action = action;
    }

    public void Execute()
    {
        action();
    }
}
