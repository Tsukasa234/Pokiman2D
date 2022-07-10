using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Scriptable object para las estadisticas de cada pokemon
[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Nuevo Pokemon")]
public class PokemonBase : ScriptableObject
{
    [SerializeField] private int ID;
    [SerializeField] private string namae;
    public string Namae { get { return namae; } }


    [TextArea] [SerializeField] private string description;
    public string Description { get => description; }

    [SerializeField] private Sprite frontsprite;
    public Sprite FrontSprite => frontsprite;
    [SerializeField] private Sprite backsprite;
    public Sprite BackSprite => backsprite;

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

    //Instanciacion de una lista con una clase que verificara los movimientos aprendidos y los guardara
    [SerializeField] private List<LearnableMove> learnableMove;
    public List<LearnableMove> LearnableMoves => learnableMove;

}
//Enumerado que guarda los tipos de pokemon que hay
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

//Clase serializable(se necesita la libreria using system;) que guarda el movimiento y el nivel que se necesita
//para acceder a ese movimiento
[Serializable]public class LearnableMove
{
    //Referencia al scriptable object de los movimientos
    [SerializeField] private MovementBase move;
    public MovementBase Move => move;
    //variable para decir que nivel se necesita para acceder a ese movimiento
    [SerializeField] private int level;
    public int Level => level;
} 