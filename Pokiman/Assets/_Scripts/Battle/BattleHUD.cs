using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    //creamos una variable para pedir el texto de la UI para el nombre
    [SerializeField] private Text pokemonName;
    //Creamos una variable para pedir el texto de la UI para el nivel
    [SerializeField] private Text pokemonLevel;
    //Creamos una referencia a la barra de vida de la UI
    [SerializeField] private HealthBar healthBar;
    //Creamos una referencia al texto de Vida paa mostrar la vida actual y la vida maxima
    [SerializeField] private Text HealthText;

    /// <summary>Metodo que actualizara los datos del pokemon en la UI</summary>
    /// <param name="pPokemon">Referencia para tomar los datos del pokemon en batlla</param>
    public void SetPokemonData(Pokemon pPokemon)
    {
        pokemonName.text = pPokemon.BasePokemon.Namae;
        pokemonLevel.text = $"Lv.{pPokemon.Level}";
        healthBar.SetHealthBar(pPokemon.HP/pPokemon.MaxHP);
        HealthText.text = $"{pPokemon.HP}/{pPokemon.MaxHP}";
    }
}
