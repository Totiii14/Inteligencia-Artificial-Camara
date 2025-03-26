using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionNode : IDesitionNode
{
    IDesitionNode trueNode;
    IDesitionNode falseNode;
    private Func<bool> question;

    public QuestionNode(IDesitionNode trueNode, IDesitionNode falseNode, Func<bool> question)
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
