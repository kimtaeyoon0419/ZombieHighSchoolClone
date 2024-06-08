// # System
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

// # Unity
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerMovement movement;


    private bool isMove;
    private Vector2 input;
    [SerializeField] float movespeed;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        MoveInput();
    }

    #region Move
    private void MoveInput()
    {
        if (!isMove)
        {
            if (Input.GetAxisRaw("Vertical") == 0)
            {
                input.y = 0;
                input.x = Input.GetAxisRaw("Horizontal");
            }
            if (Input.GetAxisRaw("Horizontal") == 0)
            {
                input.x = 0;
                input.y = Input.GetAxisRaw("Vertical");
            }

            if(input != Vector2.zero)
            {
                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                StartCoroutine(Move(targetPos));
            }
        }
    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMove = true;
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, movespeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;

        isMove = false;
    }

    #endregion
}
