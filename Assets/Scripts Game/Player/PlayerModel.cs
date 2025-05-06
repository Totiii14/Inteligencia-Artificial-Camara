using UnityEngine;

public class PlayerModel 
{
    public PlayerFSM Data { get; private set; }
    private Rigidbody rb;
    private float startYScale;

    public PlayerModel(PlayerFSM data, Rigidbody rb)
    {
        this.Data = data;
        this.rb = rb;
        startYScale = data.transform.localScale.y;
    }

    public void Move(Vector3 dir, float speed)
    {
        Vector3 moveDir = Data.orientation.forward * dir.z + Data.orientation.right * dir.x;
        if (IsGrounded())
            rb.AddForce(moveDir.normalized * speed * 10f * Time.deltaTime, ForceMode.Force);
        else
            rb.AddForce(moveDir.normalized * speed * 10f * Data.airMultiplier * Time.deltaTime, ForceMode.Force);
    }

    public bool IsGrounded()
    {
        float checkDistance = (Data.transform.localScale.y / startYScale) * (Data.playerHeight * 0.5f + 0.2f);
        return Physics.Raycast(Data.transform.position, Vector3.down, checkDistance, Data.groundLayer);
    }

    public void ApplyDrag()
    {
        rb.drag = IsGrounded() ? Data.groundDrag : 0f;
    }

    public Vector2 GetInput()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }
}
