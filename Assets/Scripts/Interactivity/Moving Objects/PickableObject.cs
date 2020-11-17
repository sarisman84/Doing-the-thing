using System;
using System.Collections;
using System.Collections.Generic;
using Interactivity;
using Player;
using UnityEngine;

public class PickableObject : MonoBehaviour, IInteractable
{
    private Rigidbody _rigidbody;
    private bool _isBeingGrabbed;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _rigidbody.useGravity = !_isBeingGrabbed;
        _isBeingGrabbed = false;
    }

    public void OnInteract(PlayerController controller)
    {
        _isBeingGrabbed = true;
        var pickupPosition = controller.transform.position + controller.playerCamera.transform.forward.normalized * 2f;
        _rigidbody.MovePosition(pickupPosition);
    }

    public void OnProximity()
    {
        
    }
}