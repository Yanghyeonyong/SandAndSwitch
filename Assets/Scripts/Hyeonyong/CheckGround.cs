using UnityEngine;

public class CheckGround : MonoBehaviour
{
    public bool _isGround = true;
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            _isGround = false;
        }
    }
}
