using System;

public class QuestionNode : IDesitionNode
{
    private IDesitionNode trueNode;
    private IDesitionNode falseNode;
    private System.Func<bool> condition;

    public QuestionNode(IDesitionNode trueNode, IDesitionNode falseNode, Func<bool> condition)
    {
        this.trueNode = trueNode;
        this.falseNode = falseNode;
        this.condition = condition;
    }

    public void Execute()
    {
        if (condition())
            trueNode.Execute();
        else
            falseNode.Execute();
    }
}
