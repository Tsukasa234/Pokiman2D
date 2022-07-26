using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokemonParty : MonoBehaviour
{
    [SerializeField] private List<Pokemon> pokemons;
    public const int NUM_MAX_OF_POKEMON_IN_PARTY = 6;

    // private List<List<Pokemon>> listBoxPC;

    public List<Pokemon> Pokemons => pokemons;

    private void Start()
    {
        foreach (var pokemon in Pokemons)
        {
            pokemon.InitPokemon();
        }

        // var box = new List<Pokemon>(15);
        // for (var i = 0; i < 6; i++)
        // {
        //     listBoxPC.Add(box);
        // }
    }

    public Pokemon GetFirstHealthyPokemon()
    {
        return Pokemons.Where(x => x.HP > 0).FirstOrDefault();
    }

    public int GetPositionFromPokemon(Pokemon pPokemon)
    {
        for (var i = 0; i < pokemons.Count; i++)
        {
            if (pPokemon == pokemons[i])
            {
                return i;
            }
        }

        return -1;
    }

    public bool AddPokemonToParty(Pokemon pPokemon)
    {
        if (pokemons.Count < NUM_MAX_OF_POKEMON_IN_PARTY)
        {
            pokemons.Add(pPokemon);
            return true;
        }
        else
        {
            //TODO crear la funcion de añadir al pc
            return false;
        }
    }
}
