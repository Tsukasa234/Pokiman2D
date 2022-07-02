using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Move", menuName = "Pokemon/Nuevo Movimiento")]
public class MovementBase : ScriptableObject
{
    [SerializeField] private string namae;
    [SerializeField][TextArea] private string description;
    [SerializeField] private PokemonType tipoAtaque;
    [SerializeField] private int power;
    [SerializeField] private int accuracy;
    [SerializeField] private int pp;
}
