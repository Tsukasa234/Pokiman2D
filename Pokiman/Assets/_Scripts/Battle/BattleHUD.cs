using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleHUD : MonoBehaviour
{
    //creamos una variable para pedir el texto de la UI para el nombre
    [SerializeField] private Text pokemonName;
    //Creamos una variable para pedir el texto de la UI para el nivel
    [SerializeField] private Text pokemonLevel;
    //Creamos una referencia a la barra de vida de la UI
    [SerializeField] private HealthBar healthBar;
    //Creamos una referencia al texto de Vida paa mostrar la vida actual y la vida maxima
    [SerializeField] private Text healtBarText;
    [SerializeField] private GameObject expBar;

    private Pokemon _pokemon;

    /// <summary>Metodo que actualizara los datos del pokemon en la UI</summary>
    /// <param name="pPokemon">Referencia para tomar los datos del pokemon en batlla</param>
    public void SetPokemonData(Pokemon pPokemon)
    {
        _pokemon = pPokemon;
        pokemonName.text = pPokemon.BasePokemon.Namae;
        SetLevelText();
        // healthBar.SetHealthBar(pPokemon.HP / pPokemon.MaxHP);
        // healtBarText.text = $"{pPokemon.HP}/{pPokemon.MaxHP}";
        healthBar.SetHealthBar((float)_pokemon.HP / _pokemon.MaxHP);
        SetExp();
        StartCoroutine(UpdatePokemonData(_pokemon.HP));
    }

    public IEnumerator UpdatePokemonData(int oldHPVal)
    {
        StartCoroutine(healthBar.SetSmoothHP((float)_pokemon.HP / _pokemon.MaxHP));
        StartCoroutine(DecreaseHealthPoints(oldHPVal));
        yield return null;
    }

    public IEnumerator DecreaseHealthPoints(int oldHPVal)
    {
        while (oldHPVal > _pokemon.HP)
        {
            oldHPVal--;
            healtBarText.text = $"{oldHPVal}/{_pokemon.MaxHP}";
            yield return new WaitForSeconds(0.3f);
        }
        healtBarText.text = $"{_pokemon.HP}/{_pokemon.MaxHP}";
    }

    public void SetExp()
    {
        if (expBar == null)
        {
            return;
        }

        expBar.transform.localScale = new Vector3(NormalizedExp(), 1, 1);
    }

    public IEnumerator SetSmoothExp(bool resetBarExp = false)
    {
        if (expBar == null)
        {
            yield break;
        }

        if (resetBarExp)
        {
            expBar.transform.localScale = new Vector3(0, 1, 1);
        }

        yield return expBar.transform.DOScaleX(NormalizedExp(), 1.5f).WaitForCompletion();
    }

    float NormalizedExp()
    {
        float currentLevelExp = _pokemon.BasePokemon.ExpNecessaryForLevelUp(_pokemon.Level);
        float nextLevelExp = _pokemon.BasePokemon.ExpNecessaryForLevelUp(_pokemon.Level + 1);
        float normalizedExp = (_pokemon.Experience - currentLevelExp) / (nextLevelExp - currentLevelExp);
        return Mathf.Clamp01(normalizedExp);
    }

    public void SetLevelText()
    {
        pokemonLevel.text = $"Lv. {_pokemon.Level}";
    }

    

}
