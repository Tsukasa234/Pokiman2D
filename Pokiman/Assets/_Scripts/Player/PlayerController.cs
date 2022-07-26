using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed;
    private bool isMoving;

    private Vector2 input;

    private Animator anim;
    

    public event Action OnPokemonEncounter;
    
    [SerializeField] private float verticalDes = 0.2f;

    [SerializeField]
    private LayerMask solidObjectsLayer, pokemonLayer;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void HandleUpdate()
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

    private void LateUpdate()
    {
        anim.SetBool("isWalk", isMoving);
    }

    IEnumerator MoveTowards(Vector3 Destination)
    {
        isMoving = true;
        while (Vector3.Distance(transform.position, Destination) > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, Destination, speed * Time.deltaTime);
            yield return null;
        }
        transform.position = Destination;
        isMoving = false;
        CheckforPokemon();
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

    private void CheckforPokemon()
    {
        if (Physics2D.OverlapCircle(transform.position - new Vector3(0,verticalDes,0), 0.1f, pokemonLayer) != null)
        {
            if (Random.Range(0, 100) < 15)
            {
                OnPokemonEncounter();
                anim.SetBool("isWalk", false);
            }
        }
    }
}
