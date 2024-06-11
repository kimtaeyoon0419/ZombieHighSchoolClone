// # System
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;

// # Unity
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Move")]
    private bool isMove;
    private Vector2 input;
    private Vector2 previousInput;
    [SerializeField] float movespeed;

    [Header("LayerMask")]
    [SerializeField] private LayerMask objectLayer;
    [SerializeField] private LayerMask groundLayer;

    [Header("Animation")]
    Animator anim;
    private readonly int hashUp = Animator.StringToHash("Up");
    private readonly int hashDown = Animator.StringToHash("Down");
    private readonly int hashLeft = Animator.StringToHash("Left");
    private readonly int hashRight = Animator.StringToHash("Right");


    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        MoveInput();
        if (input != previousInput)
        {
            previousInput = input;
            SetAnim();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        //Gizmos.DrawLine(transform.position, input, 1f);
    }

    #region Animation
    private void SetAnim()
    {
        anim.SetBool(hashUp, input.y > 0 && !anim.GetBool(hashUp));
        anim.SetBool(hashDown, input.y < 0 && !anim.GetBool(hashDown));
        anim.SetBool(hashRight, input.x > 0 && !anim.GetBool(hashRight));
        anim.SetBool(hashLeft, input.x < 0 && !anim.GetBool(hashLeft));
    }

    #endregion

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

            if (input != Vector2.zero)
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
        if (Physics2D.Raycast(transform.position, input, 1f, objectLayer) == false)
        {
            while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, movespeed * Time.deltaTime);
                yield return null;
            }
        }
        isMove = false;
    }
    #endregion
}
