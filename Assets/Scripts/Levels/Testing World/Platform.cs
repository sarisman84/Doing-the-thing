using System;
using System.Collections;
using System.Collections.Generic;
using General_Scripts.Utility.Extensions;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Platform : MonoBehaviour
{
    public enum ActionMode
    {
        StopOnEnd,
        LoopOnEnd,
        ReverseOnEnd,
        TeleportOnEnd
    }

    private Coroutine m_PlatformCoroutine;
    private Rigidbody m_Rigidbody;


    private void Awake()
    {
        Move(10, ActionMode.LoopOnEnd, transform.position + Vector3.left * 10f, transform.position);
    }


    public void Move(float velocity, ActionMode movementMode, params Vector3[] waypoints)
    {
        if (m_PlatformCoroutine != null)
        {
            StopCoroutine(m_PlatformCoroutine);
        }

        m_Rigidbody ??= GetComponent<Rigidbody>();
        m_PlatformCoroutine = StartCoroutine(ExecuteMovement(velocity, movementMode, waypoints));
    }

    public IEnumerator ExecuteMovement(float velocity, ActionMode movementMode, Vector3[] waypoints)
    {
        int index = 0;
        bool isReversing = false;
        while (true)
        {
            Vector3 direction = (waypoints[index] - transform.position).normalized;
            m_Rigidbody.MovePosition(transform.position + (direction * (velocity * Time.fixedDeltaTime)));

            if (transform.position.IsInTheVicinityOf(waypoints[index], 0.1f))
            {
                index = isReversing ? index - 1 : index + 1;
                switch (movementMode)
                {
                    case ActionMode.StopOnEnd:
                        if (index >= waypoints.Length)
                            goto Exit;
                        break;
                    case ActionMode.LoopOnEnd:
                        if (index >= waypoints.Length)
                            index = 0;
                        break;
                    case ActionMode.ReverseOnEnd:
                        if (index >= waypoints.Length || index <= 0)
                            isReversing = !isReversing;
                        break;
                    case ActionMode.TeleportOnEnd:
                        if (index >= waypoints.Length)
                        {
                            index = 0;
                            m_Rigidbody.MovePosition(waypoints[index]);
                        }
                        break;
                }

              
            }
            yield return new WaitForFixedUpdate();
        }

        Exit:
        yield return null;
    }
}