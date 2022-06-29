using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed;
    private bool isMoving;

    private Vector2 input;

    private Animator anim;

    [SerializeField]
    private LayerMask solidObjectsLayer, pokemonLayer;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        //float horizontal = Input.GetAxisRaw("Horizontal");
        //float vertical = Input.GetAxisRaw("Vertical");

        //input = new Vector2(horizontal * speed * Time.deltaTime, vertical * speed * Time.deltaTime);

        //transform.Translate(input.normalized);

        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            //if (input.x != 0)
            //{
            //    input.y = 0;
            //}

            if (input != Vector2.zero)
            {
                var targetPosition = transform.position;
                targetPosition.x += input.x;
                targetPosition.y += input.y;
                if (isAvailable(targetPosition))
                {
                    StartCoroutine(MoveTowards(targetPosition));
                    anim.SetFloat("Horizontal", input.x);
                    anim.SetFloat("Vertical", input.y);
                }
            }
        }
    }

    IEnumerator MoveTowards(Vector3 Destination)
    {
        isMoving = true;
        anim.SetBool("isWalk", isMoving);
        while (Vector3.Distance(transform.position, Destination) > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, Destination, speed * Time.deltaTime);
            yield return null;
        }
        transform.position = Destination;
        isMoving = false;
        anim.SetBool("isWalk", isMoving);
    }

    /// <summary>
    /// Metodo para comprobar si el lugar es accesible o no
    /// </summary>
    /// <param name="target">Lugar hacia donde queremos acceder</param>
    /// <returns>si es true no se accedera al lugar, pero si es false se puede acceder al lugar</returns>
    private bool isAvailable(Vector3 target)
    {
        if (Physics2D.OverlapCircle(target, 0.2f, solidObjectsLayer) != null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
