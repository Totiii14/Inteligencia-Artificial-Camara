using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    public bool IsDetectable { get; private set; } = true;

    public void SetDetectable(bool value)
    {
        IsDetectable = value;
    }
}
