using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pokemon
{
    //referencia a la clase scriptableObject PokemonBase para las estadisticas de cada pokemon
    private PokemonBase _base;
    public PokemonBase BasePokemon => _base;
    //Variable para medir el nivel en el que se encuentre el pokemon actual
    private int level;
    public int Level { get => level; set=> level = value; }
    //Una lista de los movimientos que el pokemon actual puede aprender con su propiedad accesible
    [SerializeField]private List<Move> moves;
    public List<Move> Moves {get => moves; set => moves = value;}
    //Variable para medir los puntos de vida del pokemon actual con su propiedad accesible
    private int _hp;
    public int HP {get => _hp; set => _hp = value;}
    //Constructor de la clase con los parametos de las estadisticas del pokemon y el nivel
    public Pokemon(PokemonBase pPokemon, int pLevel)
    {
        //Inicializacion de las variables
        _base = pPokemon;
        level = pLevel;
        _hp = MaxHP;
        //incializacion de la lista de movimientos
        moves = new List<Move>();
        //Ciclo para recorrer los movimientos que el pokemon puede aprender
        foreach(var lMove in _base.LearnableMoves)
        {
            //Medimos el nivel del pokemon para saber que movimiento agregar
            if (lMove.Level <= level)
            {
                //Agregamos ese movimiento aprendido a la lista de movimientos del pokemon instanciando el constructor
                //de la clase que administra los movimientos y pasamos el paramentro que pide.
                moves.Add(new Move(lMove.Move));
            }
            //Verificamos que la cantidad de movimientos aprendidos no sea mayor a 4
            if (moves.Count >= 4)
            {
                break;
            }
        }
    }
    //aqui formulamos para subir las estadisticas del pokemon cada que suba de nivel
    public int Attack => Mathf.FloorToInt((_base.Attack*level)/100f)+1;
    public int MaxHP => Mathf.FloorToInt((_base.MaxHP*level)/20.0f)+10;
    public int Defense => Mathf.FloorToInt((_base.Defense*level)/100f)+1;
    public int Speed => Mathf.FloorToInt((_base.Speed*level)/100f)+1;
    public int SpAttack => Mathf.FloorToInt((_base.SpAttack*level)/100f)+1;
    public int SpDefense => Mathf.FloorToInt((_base.SpDefense*level)/100f)+1;
}
