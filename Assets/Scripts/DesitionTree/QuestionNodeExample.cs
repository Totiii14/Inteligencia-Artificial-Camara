using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionNodeExample : IDesitionNodeExample
{
    IDesitionNodeExample trueNode;
    IDesitionNodeExample falseNode;
    private Func<bool> question;

    public QuestionNodeExample(IDesitionNodeExample trueNode, IDesitionNodeExample falseNode, Func<bool> question)
    {
        this.trueNode = trueNode;
        this.falseNode = falseNode;
        this.question = question;
    }

    public void Execute()
    {
        if (question())
            trueNode.Execute();
        else
            falseNode.Execute();
    }
}
