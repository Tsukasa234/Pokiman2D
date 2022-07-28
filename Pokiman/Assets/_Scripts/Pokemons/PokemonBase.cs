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

    [SerializeField] private int xpBase;
    public int XpBase => xpBase;
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

    [SerializeField] private GrowRate growRate;
    public GrowRate GrowRate => growRate;

    [SerializeField] private int catchRate;
    public int CatchRate => catchRate;

    //Instanciacion de una lista con una clase que verificara los movimientos aprendidos y los guardara
    [SerializeField] private List<LearnableMove> learnableMove;
    public List<LearnableMove> LearnableMoves => learnableMove;

    public int ExpNecessaryForLevelUp(int level)
    {
        switch (growRate)
        {
            case GrowRate.Fast:
                return Mathf.FloorToInt(4 * Mathf.Pow(level, 4) / 5);
                break;
            case GrowRate.MediumFast:
                return Mathf.FloorToInt(Mathf.Pow(level, 3));
                break;
            case GrowRate.MediumSlow:
                return Mathf.FloorToInt(6 * Mathf.Pow(level, 3) / 5 - 15 * Mathf.Pow(level, 2) + 100 * level - 140);
                break;
            case GrowRate.Slow:
                return Mathf.FloorToInt(5 * Mathf.Pow(level, 3) / 4);
                break;
            case GrowRate.Erratic:
                if (level < 50)
                {
                    return Mathf.FloorToInt(Mathf.Pow(level, 3) * (100 - level) / 50);
                }
                else if (level < 68)
                {
                    return Mathf.FloorToInt(Mathf.Pow(level, 3) * (150 - level) / 100);
                }
                else if (level < 98)
                {
                    return Mathf.FloorToInt(Mathf.Pow(level, 3) * Mathf.FloorToInt((1911 - 10 * level) / 3) / 500);
                }
                else
                {
                    return Mathf.FloorToInt(Mathf.Pow(level, 3) * (160 - level) / 100);
                }
                break;
            case GrowRate.Fluctuating:
                if (level < 15)
                {
                    return Mathf.FloorToInt(Mathf.Pow(level, 3) * (Mathf.FloorToInt((level + 1) / 3) + 24) / 50);
                }
                else if (level < 36)
                {
                    return Mathf.FloorToInt(Mathf.Pow(level, 3) * (level + 14) / 50);
                }
                else
                {
                    return Mathf.FloorToInt(Mathf.Pow(level, 3) * (Mathf.FloorToInt(level / 2) + 32) / 50);
                }
                break;
        }

        return -1;

    }

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

public enum GrowRate
{
    Erratic,
    Fast,
    MediumFast,
    MediumSlow,
    Slow,
    Fluctuating
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

        return matrix[row - 1][col - 1];
    }
}

public class TypeColor
{
    private static Color[] typeColor =
    {
            Color.white, //None
            
            new Color(0.6588235f, 0.6588235f, 0.4705883f, 1),//Normal 

            new Color(0.9411765f, 0.5019608f, 0.1882353f, 1),//Fire

            new Color(0.4078432f, 0.5647059f, 0.9411765f, 1),//Water

            new Color(0.9686275f, 0.8117648f, 0.1882353f, 1),//Electric

            new Color(0.4705883f, 0.7843138f, 0.3137255f, 1), //Grass

            new Color(0.5960785f, 0.8470589f, 0.8470589f, 1), //Ice

            new Color(0.7529413f, 0.1882353f, 0.1568628f, 1), //Fight

            new Color(0.627451f, 0.2509804f, 0.627451f, 1),   //Poison

            new Color(0.8784314f, 0.7529413f, 0.4078432f, 1), //Ground

            new Color(0.6588235f, 0.5647059f, 0.9411765f, 1), //Fly

            new Color(0.8117648f, 0.3058824f, 0.4588236f, 1), //Physic

            new Color(0.6588235f, 0.7215686f, 0.1254902f, 1), //Bug

            new Color(0.7215686f, 0.627451f, 0.2196079f, 1),  //Rock

            new Color(0.4392157f, 0.345098f, 0.5960785f, 1),   //Ghost

            new Color(0.4392157f, 0.2196079f, 0.9725491f, 1), //Dragon

            new Color(0.4392157f, 0.345098f, 0.282353f, 1),   //Dark

            new Color(0.7215686f, 0.7215686f, 0.8156863f, 1), //Steel

            new Color(0.9333334f, 0.6f, 0.6745098f, 1)        //Fairy
        };

    public static Color GetColorOftype(PokemonType type)
    {
        return typeColor[(int)type];
    }
}