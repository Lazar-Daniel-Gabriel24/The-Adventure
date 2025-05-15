using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;               // Jucătorul
    public float smoothSpeed = 5f;         // Viteza de urmărire
    public Vector2 offset = new Vector2(2f, 1f);  // Offset pe X și Y

    private bool facingRight = true;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + new Vector3(facingRight ? offset.x : -offset.x, offset.y, -10f);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }

    /// <summary>
    /// Apelează când jucătorul schimbă direcția
    /// </summary>
    public void CallTurn()
    {
        facingRight = !facingRight;
    }
}
