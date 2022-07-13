using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonMapArea : MonoBehaviour
{
    [SerializeField] private List<Pokemon> wildPokemons;

    public List<Pokemon> WildPokemons { get => wildPokemons; set => wildPokemons = value; }

    public Pokemon GetRandomWildPokemon()
    {
        var pokemon = WildPokemons[Random.Range(0, WildPokemons.Count)];
        pokemon.InitPokemon();

        return pokemon;
    }
}
