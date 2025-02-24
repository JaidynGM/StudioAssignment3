using System;
using UnityEngine;
using UnityEngine.Events;

public class GroundCheck : MonoBehaviour
{
    public LayerMask groundLayer;
    public bool isOnGround = false;

    public UnityEvent<bool> OnGroundChange;

    private void OnTriggerEnter(Collider triggeredObject)
    {
        if (triggeredObject.CompareTag("ground"))
        {
            isOnGround = true;
            OnGroundChange?.Invoke(true);
        }
    }

    private void OnTriggerExit(Collider triggeredObject)
    {
        if (triggeredObject.CompareTag("ground"))
        {
            isOnGround = false;
            OnGroundChange?.Invoke(false);
        }
    }
}
