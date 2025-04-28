using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    [field: SerializeField] public bool IsDetectable { get; private set; } = true;

    public void SetDetectable(bool value)
    {
        IsDetectable = value;
    }
}
