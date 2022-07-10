using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField]private PokemonBase _base;
    public PokemonBase _Base {get => _base; set => _base = value;}
    [SerializeField]private int level;
    public int Level { get => level; set => level = value;}
    public bool isPlayer;

    public Pokemon pokemon {get; set;}

    /// <summary>
    ///Metodo para configurar al pokemon que estara en la batalla dependiendo de si es enemigo o el player
    ///</summary>
    public void SetupPokemon()
    {
        pokemon = new Pokemon(_base, level);
        
        GetComponent<Image>().sprite = (isPlayer? pokemon.BasePokemon.BackSprite 
            : pokemon.BasePokemon.FrontSprite);
    }
}
