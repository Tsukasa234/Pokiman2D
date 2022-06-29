using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Nuevo Pokemon")]
public class PokemonBase : ScriptableObject
{
    [SerializeField] private int ID;
    [SerializeField] private string namae;
    [TextArea] [SerializeField] private string description;

    [SerializeField] private Sprite frontsprite;
    [SerializeField] private Sprite backsprite;

    [SerializeField] private PokemonType type1;
    [SerializeField] private PokemonType type2;

    //Stats
    [SerializeField] private int maxHP;
    [SerializeField] private int attack;
    [SerializeField] private int defense;
    [SerializeField] private int speed;
    [SerializeField] private int spAttack;
    [SerializeField] private int spDefense;
}

public enum PokemonType
{
    None,
    Normal,
    Fire,
    Water,
    Darkness,
    Electric,
    Grass,
    Fight,
    Ice,
    Poison,
    Ground,
    Fly,
    Bug,
    Ghost,
    Dragon,
    Fairy,
    Steel,
}