using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokemonParty : MonoBehaviour
{
    [SerializeField] private List<Pokemon> pokemons;

    public List<Pokemon> Pokemons => pokemons;

    private void Start()
    {
        foreach (var pokemon in Pokemons)
        {
            pokemon.InitPokemon();
        }
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
}
