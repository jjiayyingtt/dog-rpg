using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector2 movement;
    private Animator animator;
    public float speed = 1f;
    private bool isMoving;

    public LayerMask solidObjectsLayer;
    public LayerMask GrassLayer;

    public event Action OnEncountered;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void HandleUpdate()
    {
        if (!isMoving)
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

            if (movement.x != 0) movement.y = 0;

            if (movement != Vector2.zero)
            {
                animator.SetFloat("moveX", movement.x);
                animator.SetFloat("moveY", movement.y);
                
                var targetPos = transform.position;
                targetPos.x += movement.x;
                targetPos.y += movement.y;

                if (IsWalkable(targetPos))
                {
                    StartCoroutine(Move(targetPos));
                }
            }
        }
        animator.SetBool("isMoving", isMoving);

    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.fixedDeltaTime);
            yield return null;
        }
        transform.position = targetPos;
        isMoving = false;

        CheckForEncounter();
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectsLayer) != null)
        {
            return false;
        }

        return true;
    }

    private void CheckForEncounter()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, GrassLayer) != null)
        {
            if (UnityEngine.Random.Range(1, 101) <= 10)
            {
                animator.SetBool("isMoving", false);
                OnEncountered();
            }
        }
    }


}
