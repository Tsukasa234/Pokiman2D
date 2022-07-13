using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { Travel, Battle }

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BattleManager battleManager;

    [SerializeField] private Camera worldMainCamera;

    private GameState state;

    private void Awake()
    {
        state = GameState.Travel;
    }

    private void Start()
    {
        playerController.OnPokemonEncounter += StartPokemonBattle;
        battleManager.OnPokemonBattleFinish += FinishPokemonBattle;
    }

    private void Update()
    {
        if (state == GameState.Travel)
        {
            playerController.HandleUpdate();

        }
        else if (state == GameState.Battle)
        {
            battleManager.HandleUpdate();
        }
    }

    private void StartPokemonBattle()
    {
        state = GameState.Battle;
        battleManager.gameObject.SetActive(true);
        worldMainCamera.gameObject.SetActive(false);
        
        var playerParty = playerController.GetComponent<PokemonParty>();
        var wildPokemon = FindObjectOfType<PokemonMapArea>().GetComponent<PokemonMapArea>().GetRandomWildPokemon();

        battleManager.HandleStartBattle(playerParty, wildPokemon);
    }

    private void FinishPokemonBattle(bool playerHasWon)
    {
        state = GameState.Travel;
        worldMainCamera.gameObject.SetActive(true);
        battleManager.gameObject.SetActive(false);
        if (!playerHasWon)
        {
            //TODO: Implementar derrota o cambio de pokemon
        }
    }
}
