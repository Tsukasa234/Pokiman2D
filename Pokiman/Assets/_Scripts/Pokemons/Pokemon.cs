using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pokemon
{
    private PokemonBase pokemon;
    private int level;

    public Pokemon(PokemonBase pPokemon, int pLevel)
    {
        pokemon = pPokemon;
        level = pLevel;
    }

    public int Attack => Mathf.FloorToInt((pokemon.Attack*level)/100f)+1;
    public int MaxHP => Mathf.FloorToInt((pokemon.MaxHP*level)/100f)+10;
    public int Defense => Mathf.FloorToInt((pokemon.Defense*level)/100f)+1;
    public int Speed => Mathf.FloorToInt((pokemon.Speed*level)/100f)+1;
    public int SpAttack => Mathf.FloorToInt((pokemon.SpAttack*level)/100f)+1;
    public int SpDefense => Mathf.FloorToInt((pokemon.SpDefense*level)/100f)+1;
}
