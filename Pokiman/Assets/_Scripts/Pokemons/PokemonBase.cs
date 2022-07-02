using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Nuevo Pokemon")]
public class PokemonBase : ScriptableObject
{
    [SerializeField] private int ID;
    [SerializeField] private string namae;
    public string Namae { get { return namae; } }


    [TextArea] [SerializeField] private string description;
    public string Description { get => description; }

    [SerializeField] private Sprite frontsprite;
    [SerializeField] private Sprite backsprite;

    [SerializeField] private PokemonType type1;
    [SerializeField] private PokemonType type2;
    public PokemonType Type1 {get => type1;}
    public PokemonType Type2 {get => type2;}

    //Stats
    [SerializeField] private int maxHP;
    public int MaxHP => maxHP;
    [SerializeField] private int attack;
    public int Attack => attack;
    [SerializeField] private int defense;
    public int Defense => defense;
    [SerializeField] private int speed;
    public int Speed => speed;
    [SerializeField] private int spAttack;
    public int SpAttack => spAttack;
    [SerializeField] private int spDefense;
    public int SpDefense => spDefense;
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