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
    public string Namae => namae;


    [TextArea][SerializeField] private string description;
    public string Description => description; 

    [SerializeField] private Sprite frontsprite;
    public Sprite FrontSprite => frontsprite;
    [SerializeField] private Sprite backsprite;
    public Sprite BackSprite => backsprite;

    [SerializeField] private PokemonType type1;
    [SerializeField] private PokemonType type2;
    public PokemonType Type1 => type1;
    public PokemonType Type2 => type2;

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
    Electric,
    Grass,
    Ice,
    Fight,
    Poison,
    Ground,
    Fly,
    Physic,
    Bug,
    Rock,
    Ghost,
    Dragon,
    Dark,
    Steel,
    Fairy,
}

//Clase serializable(se necesita la libreria using system;) que guarda el movimiento y el nivel que se necesita
//para acceder a ese movimiento
[Serializable]
public class LearnableMove
{
    //Referencia al scriptable object de los movimientos
    [SerializeField] private MovementBase move;
    public MovementBase Move => move;
    //variable para decir que nivel se necesita para acceder a ese movimiento
    [SerializeField] private int level;
    public int Level => level;
}

public class TypeMatrix
{
    private static float[][] matrix =
    {
        /*	                NOR FIR WAT ELE GRA ICE FIG POI GRO FLY PSY BUG ROC GHO DRA DAR STE FAI*/
        /*	*/
        /*NOR*/ new float[] {1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 0.5f, 0f, 1f, 1f, 0.5f, 1f},  

        /*FIR*/ new float[] {1f, 0.5f, 0.5f, 2f, 2f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 0.5f, 1f, 0.5f, 1f, 2f, 1f},

        /*WAT*/ new float[] {1f, 2f, 0.5f, 1f, 0.5f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 2f, 1f, 0.5f, 1f, 2f, 1f},

        /*ELE*/ new float[] {1f, 1f, 2f, 0.5f, 0.5f, 1f, 1f, 1f, 0f, 2f, 1f, 1f, 1f, 1f, 0.5f, 1f, 1f, 1f},

        /*GRA*/ new float[] {1f, 0.5f, 2f, 1f, 0.5f, 1f, 1f, 0.5f, 2f, 0.5f, 1f, 0.5f, 2f, 1f, 0.5f, 1f, 0.5f, 1f},

        /*ICE*/ new float[] {1f, 0.5f, 0.5f, 1f, 2f, 0.5f, 1f, 1f, 2f, 2f, 1f, 1f, 1f, 1f, 2f, 1f, 0.5f, 1f},

        /*FIG*/ new float[] {2f, 1f, 1f, 1f, 1f, 2f, 1f, 0.5f, 1f, 0.5f, 0.5f, 0.5f, 2f, 0f, 1f, 2f, 2f, 0.5f},

        /*POI*/ new float[] {1f, 1f, 1f, 1f, 2f, 1f, 1f, 0.5f, 0.5f, 1f, 1f, 1f, 0.5f, 0.5f, 1f, 2f, 2f, 0.5f},

        /*GRO*/ new float[] {1f, 2f, 1f, 2f, 0.5f, 1f, 1f, 2f, 1f, 0f, 1f, 0.5f, 2f, 1f, 1f, 1f, 2f, 1f},

        /*FLY*/ new float[] {1f, 1f, 1f, 0.5f, 2f, 1f, 2f, 1f, 1f, 1f, 1f, 2f, 0.5f, 1f, 1f, 1f, 0.5f, 1f},

        /*PSY*/ new float[] {1f, 1f, 1f, 1f, 1f, 1f, 2f, 2f, 1f, 1f, 0.5f, 1f, 1f, 1f, 1f, 0f, 0.5f, 1f},

        /*BUG*/ new float[] {1f, 0.5f, 1f, 1f, 2f, 1f, 0.5f, 0.5f, 1f, 0.5f, 2f, 1f, 1f, 0.5f, 1f, 2f, 0.5f, 0.5f},

        /*ROC*/ new float[] {1f, 2f, 1f, 1f, 1f, 2f, 0.5f, 1f, 0.5f, 2f, 1f, 2f, 1f, 1f, 1f, 1f, 0.5f, 1f},

        /*GHO*/ new float[] {0f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 2f, 1f, 0.5f, 1f, 1f},

        /*DRA*/ new float[] {1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 0.5f, 0f},

        /*DAR*/ new float[] {1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 2f, 1f, 0.5f, 1f, 0.5f},

        /*STE*/ new float[] {1f, 0.5f, 0.5f, 0.5f, 1f, 2f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 0.5f, 2f},

        /*FAI*/ new float[] {1f, 0.5f, 1f, 1f, 1f, 1f, 2f, 0.5f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 2f, 0.5f, 1f}
    };

    public static float GetMupltiplierEfectiveness(PokemonType attackType, PokemonType pokemonDefenderType)
    {
        if (attackType == PokemonType.None || pokemonDefenderType == PokemonType.None)
        {
            return 1.0f;
        }

        int row = (int)attackType;
        int col = (int)pokemonDefenderType;

        return matrix[row - 1][col -1];
    }
}