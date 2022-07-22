using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberHUD : MonoBehaviour
{
    [SerializeField] private Text nameText, levelText, typeText, hpText;

    [SerializeField] private HealthBar _hpBar;
    [SerializeField] private Image pokemonImage, bgImage;
    [SerializeField] private Color selectedColor = Color.blue;

    private Pokemon _pokemon;

    public void SetPokemonData(Pokemon pokemon)
    {
        _pokemon = pokemon;
        nameText.text = _pokemon.BasePokemon.Namae;
        levelText.text = $"Lv. {_pokemon.Level}";
        typeText.text = _pokemon.BasePokemon.Type1.ToString().ToUpper();
        hpText.text = $"{_pokemon.HP}/{_pokemon.MaxHP}";
        _hpBar.SetHealthBar((float)_pokemon.HP / _pokemon.MaxHP);
        pokemonImage.sprite = _pokemon.BasePokemon.FrontSprite;
    }

    public void SetSelectedPokemon(bool selected)
    {
        bgImage.color = (selected ? selectedColor : Color.white);
    }

}
