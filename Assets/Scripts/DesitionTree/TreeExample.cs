using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeExample : MonoBehaviour
{
    enum DayTime
    {
        Day,
        Night
    }

    [SerializeField] Transform enemy;
    [SerializeField] DayTime time;
    [SerializeField] bool raining;
    [SerializeField] bool sleepy;
    [SerializeField] int woodAmount;
    [SerializeField] int stoneAmount;
    private IDesitionNodeExample rootNode;

    void Start()
    {
        CreateTree();
    }

    void Update()
    {
        Debug.Log(Vector3.Distance(enemy.position, transform.position) < 5);
        if (Input.GetKeyDown(KeyCode.I))
            DoTree();
    }

    private void CreateTree()
    {
        ActionNodeClass huir = new ActionNodeClass(Huir);
        ActionNodeClass dormir = new ActionNodeClass(Dormir);
        ActionNodeClass viciar = new ActionNodeClass(Viciar);
        ActionNodeClass talar = new ActionNodeClass(Talar);
        ActionNodeClass minar = new ActionNodeClass(Minar);
        ActionNodeClass contruir = new ActionNodeClass(Construir);

        QuestionNodeExample enoughStone = new QuestionNodeExample(contruir, minar, () => stoneAmount >= 10);
        QuestionNodeExample enoughWood = new QuestionNodeExample(enoughStone, talar, HaveEnoghWood);
        QuestionNodeExample amISleepy = new QuestionNodeExample(dormir, viciar, () => sleepy);
        QuestionNodeExample isRaining = new QuestionNodeExample(amISleepy, enoughWood, () => raining);
        QuestionNodeExample isDayTime = new QuestionNodeExample(isRaining, dormir, () => time == DayTime.Day);
        QuestionNodeExample enemiesNearby = new QuestionNodeExample(huir, isDayTime, IsEnemyNearby);

        rootNode = enemiesNearby;
    }

    private void DoTree()
    {
        rootNode.Execute();
        //if(Vector3.Distance(enemy.position, transform.position) < 5)
        //{
        //    Debug.Log("Huyo!! Ahhhh!");
        //}
        //else
        //{
        //    if(time == DayTime.Day)
        //    {
        //        if(raining)
        //        {
        //            if(sleepy)
        //            {
        //                Debug.Log("A mimir");
        //            }
        //            else
        //            {
        //                Debug.Log("A viciar");
        //            }
        //        }
        //        else
        //        {
        //            if(woodAmount >= 10)
        //            {
        //                if(stoneAmount >= 10)
        //                {
        //                    Debug.Log("A construir");
        //                }
        //                else
        //                {
        //                    Debug.Log("Minar");
        //                }
        //            }
        //            else
        //            {
        //                Debug.Log("A talar");
        //            }
        //        }
        //    }
        //    else
        //    {
        //        Debug.Log("A mimir");
        //    }
        //}
    }

    private void Huir() => Debug.Log("Huyo!! Ahhhh!");
    private void Dormir() => Debug.Log("A mimir");
    private void Viciar() => Debug.Log("A viciar");
    private void Talar() => Debug.Log("A talar");
    private void Minar() => Debug.Log("A minar");
    private void Construir() => Debug.Log("A construir");


    bool HaveEnoghWood()
    {
        return woodAmount >= 10;
    }

    bool IsEnemyNearby()
    {
        return Vector3.Distance(enemy.position, transform.position) < 5;
    }
}
