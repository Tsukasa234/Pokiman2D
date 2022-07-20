using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PartyHUD : MonoBehaviour
{
    private PartyMemberHUD[] memberHud;

    [SerializeField] private Text messageText;

    [SerializeField] List<Pokemon> pokemons;
    public void InitPartyHUD()
    {
        memberHud = GetComponentsInChildren<PartyMemberHUD>();
    }

    public void SetPartyPokemon(List<Pokemon> pokemons)
    {
        messageText.text = "Selecciona un pokemon";
        this.pokemons = pokemons;
        for (var i = 0; i < memberHud.Length; i++)
        {
            if (i < pokemons.Count)
            {
                memberHud[i].gameObject.SetActive(true);
                memberHud[i].SetPokemonData(pokemons[i]);
            }
            else
            {
                memberHud[i].gameObject.SetActive(false);
            }
        }
    }

    public void UpdateSelectedPokemon(int selectedPokemon)
    {
        for (var i = 0; i < pokemons.Count; i++)
        {
            memberHud[i].SetSelectedPokemon(i == selectedPokemon);
        }
    }

    public void SetMessage(string message)
    {
        messageText.text = message;
    }
}
