using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleStates
{
    StartBattle,
    ActionSelection,
    MoveSelection,
    PerformMove,
    Busy,
    PartySelectScreen,
    ItemSelectScreen,
    FinishBattle,
}

public class BattleManager : MonoBehaviour
{
    [SerializeField] private BattleUnit playerUnit;

    [SerializeField] private BattleUnit enemyUnit;

    [SerializeField] private BattleDialogBox dialog;
    [SerializeField] private PartyHUD partyHud;

    public BattleStates battleStates;


    PokemonParty playerParty;
    Pokemon wildPokemon;

    private int currentSelectionAction;
    private float timeSinceLastClick;
    [SerializeField] private float timeBeetweenClicks = 0.3f;
    private int playerMovementSelection;
    private int currentSelectedPokemon;

    public event Action<bool> OnPokemonBattleFinish;


    public void HandleStartBattle(PokemonParty playerParty, Pokemon wildPokemon)
    {
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;
        StartCoroutine(SetupBattle());
    }

    public void HandleUpdate()
    {
        timeSinceLastClick += Time.deltaTime;
        if (dialog.isWriting)
        {
            return;
        }

        if (battleStates == BattleStates.ActionSelection)
        {
            HandlerPlayerSelection();
        }
        else if (battleStates == BattleStates.MoveSelection)
        {
            HandlePlayerMovementSelection();
        }
        else if (battleStates == BattleStates.PartySelectScreen)
        {
            HandlePlayerPartySelection();
        }
    }

    public IEnumerator SetupBattle()
    {
        battleStates = BattleStates.StartBattle;

        playerUnit.SetupPokemon(playerParty.GetFirstHealthyPokemon());
        dialog.PlayerMovements(playerUnit.pokemon.Moves);

        enemyUnit.SetupPokemon(wildPokemon);

        partyHud.InitPartyHUD();

        yield return dialog.SetDialog($"Un {enemyUnit.pokemon.BasePokemon.Namae} salvaje ha aparecido");
        if (enemyUnit.pokemon.Speed > playerUnit.pokemon.Speed)
        {
            yield return dialog.SetDialog("El enemigo ataca primero");
            StartCoroutine(PerformEnemyMove());
        }
        else
        {
            PlayerActionSelector();
        }
    }

    private void BattleFinish(bool playerHasWon)
    {
        battleStates = BattleStates.FinishBattle;
        OnPokemonBattleFinish(playerHasWon);
    }

    private void PlayerActionSelector()
    {
        battleStates = BattleStates.ActionSelection;
        StartCoroutine(dialog.SetDialog("Seleciona una accion"));
        dialog.ToggleMovesText(false);
        dialog.ToggleDialogText(true);
        dialog.ToggleActions(true);
        currentSelectionAction = 0;
        dialog.SelectAction(currentSelectionAction);
    }

    private void PlayerMovementSelection()
    {
        battleStates = BattleStates.MoveSelection;
        dialog.ToggleDialogText(false);
        dialog.ToggleActions(false);
        dialog.ToggleMovesText(true);
        playerMovementSelection = 0;
        dialog.SelectionMovement(currentSelectionAction, playerUnit.pokemon.Moves[playerMovementSelection]);
    }

    private void OpenPartySelectionScreen()
    {
        battleStates = BattleStates.PartySelectScreen;
        partyHud.SetPartyPokemon(playerParty.Pokemons);
        partyHud.gameObject.SetActive(true);
        currentSelectedPokemon = playerParty.GetPositionFromPokemon(playerUnit.pokemon);
        partyHud.UpdateSelectedPokemon(currentSelectedPokemon);
    }

    private void OpenInventoryScreen()
    {
        print("Abrir la pantalla de inventario");
    }

    private void HandlerPlayerSelection()
    {
        if (timeSinceLastClick < timeBeetweenClicks)
        {
            return;
        }
        if (Input.GetAxis("Vertical") != 0)
        {
            timeSinceLastClick = 0;
            currentSelectionAction = (currentSelectionAction + 2) % 4;
        }
        else if (Input.GetAxisRaw("Horizontal") != 0)
        {
            timeSinceLastClick = 0;
            currentSelectionAction = (currentSelectionAction + 1) % 2 + 2 * Mathf.FloorToInt(currentSelectionAction / 2);
        }
        //Luchar
        //Huir

        dialog.SelectAction(currentSelectionAction);

        if (Input.GetAxisRaw("Submit") != 0)
        {
            timeSinceLastClick = 0;
            if (currentSelectionAction == 0)
            {
                //TODO: Implementar la accion
                PlayerMovementSelection();
            }
            else if (currentSelectionAction == 1)
            {
                //Seleccionar Pokemon
                OpenPartySelectionScreen();
            }
            else if (currentSelectionAction == 2)
            {
                //Seleccionar Mochila
                OpenInventoryScreen();
            }
            else if (currentSelectionAction == 3)
            {
                //Seleccionar Huir
                OnPokemonBattleFinish(false);
            }
        }

        if (Input.GetAxisRaw("Return") != 0)
        {
            OnPokemonBattleFinish(false);
        }
    }


    public void HandlePlayerMovementSelection()
    {
        if (timeSinceLastClick < timeBeetweenClicks)
        {
            return;
        }

        if (Input.GetAxisRaw("Vertical") != 0)
        {
            timeSinceLastClick = 0;
            var oldSelectionMove = playerMovementSelection;
            playerMovementSelection = (playerMovementSelection + 2) % 4;
            if (playerMovementSelection >= playerUnit.pokemon.Moves.Count)
            {
                playerMovementSelection = oldSelectionMove;
            }
            dialog.SelectionMovement(playerMovementSelection, playerUnit.pokemon.Moves[playerMovementSelection]);
        }
        else if (Input.GetAxisRaw("Horizontal") != 0)
        {
            timeSinceLastClick = 0;
            var oldSelectionMove = playerMovementSelection;
            playerMovementSelection = (playerMovementSelection + 1) % 2 + 2 *
                Mathf.FloorToInt(playerMovementSelection / 2);
            if (playerMovementSelection >= playerUnit.pokemon.Moves.Count)
            {
                playerMovementSelection = oldSelectionMove;
            }
            dialog.SelectionMovement(playerMovementSelection, playerUnit.pokemon.Moves[playerMovementSelection]);
        }

        if (Input.GetAxisRaw("Submit") != 0)
        {
            timeSinceLastClick = 0;
            dialog.ToggleMovesText(false);
            dialog.ToggleDialogText(true);
            StartCoroutine(PerformPlayerMovement());
        }

        if (Input.GetAxisRaw("Return") != 0)
        {
            PlayerActionSelector();
        }
    }


    public void HandlePlayerPartySelection()
    {
        if (timeSinceLastClick < timeBeetweenClicks)
        {
            return;
        }

        if (Input.GetAxisRaw("Vertical") != 0)
        {
            timeSinceLastClick = 0;
            currentSelectedPokemon -= (int)Input.GetAxisRaw("Vertical") * 2;
        }
        else if (Input.GetAxisRaw("Horizontal") != 0)
        {
            timeSinceLastClick = 0;
            currentSelectedPokemon += (int)Input.GetAxisRaw("Horizontal");

        }
        currentSelectedPokemon = Mathf.Clamp(currentSelectedPokemon, 0, playerParty.Pokemons.Count - 1);
        partyHud.UpdateSelectedPokemon(currentSelectedPokemon);

        if (Input.GetAxisRaw("Submit") != 0)
        {
            timeSinceLastClick = 0;
            var selectedPokemon = playerParty.Pokemons[currentSelectedPokemon];

            if (selectedPokemon.HP <= 0)
            {
                partyHud.SetMessage($"{selectedPokemon.BasePokemon.Namae} esta debilitado");
                return;
            }
            else if (selectedPokemon == playerUnit.pokemon)
            {
                partyHud.SetMessage($"{selectedPokemon.BasePokemon.Namae} ya esta en batalla");
                return;
            }
            else
            {
                partyHud.gameObject.SetActive(false);
                StartCoroutine(SwitchPokemon(selectedPokemon));
            }
        }

        if (Input.GetAxisRaw("Return") != 0)
        {
            partyHud.gameObject.SetActive(false);
            battleStates = BattleStates.Busy;
            PlayerActionSelector();
        }
    }

    IEnumerator PerformPlayerMovement()
    {
        
        battleStates = BattleStates.PerformMove;
        Move move = playerUnit.pokemon.Moves[playerMovementSelection];
        if (move.PP <= 0)
        {
            PlayerMovementSelection();
            yield break;
        }
        yield return RunMovement(playerUnit, enemyUnit, move);

        if (battleStates == BattleStates.PerformMove)
        {
            StartCoroutine(PerformEnemyMove());
        }
    }

    IEnumerator PerformEnemyMove()
    {
        battleStates = BattleStates.PerformMove;
        Move move = enemyUnit.pokemon.randomMove();
        
        yield return RunMovement(enemyUnit, playerUnit, move);
        if (battleStates == BattleStates.PerformMove)
        {
            PlayerActionSelector();            
        }
    }

    IEnumerator RunMovement(BattleUnit attacker, BattleUnit target, Move move)
    {
        move.PP--;
        yield return dialog.SetDialog($"{attacker.pokemon.BasePokemon.Namae} ha usado {move.Base.Namae}");
        var oldHPValue = target.pokemon.HP;
        attacker.PlayAttackAnimation();
        target.PlayReceiveDamageAnimation();
        yield return new WaitForSeconds(1.0f);
        var damageDesc = target.pokemon.ReceiveDamage(move, attacker.pokemon);
        yield return target.HUD.UpdatePokemonData(oldHPValue);
        yield return ShowDamageDescription(damageDesc);

        if (damageDesc.Fainted)
        {
            yield return dialog.SetDialog($"{target.pokemon.BasePokemon.Namae} se ha debilitado");
            target.PlayFaintAnimation();
            yield return new WaitForSeconds(1.5f);
            CheckForBattleFinish(target);
        }
    }

    private void CheckForBattleFinish(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayer)
        {
            var nextPokemon = playerParty.GetFirstHealthyPokemon();
            if (nextPokemon != null)
            {
                OpenPartySelectionScreen();
            }
            else BattleFinish(false);
        }
        else BattleFinish(true);
    }

    IEnumerator ShowDamageDescription(DamageDescription desc)
    {
        if (desc.Critical > 1)
        {
            yield return dialog.SetDialog("Se ha realizado un critico");
        }
        if (desc.Type > 1)
        {
            yield return dialog.SetDialog("El ataque es super efectivo");
        }
        else if (desc.Type < 1 && desc.Type > 0.1)
        {
            yield return dialog.SetDialog("El ataque no fue muy efectivo...");
        }
        else if (desc.Type == 0)
        {
            yield return dialog.SetDialog("El ataque no fue efectivo...");
        }

    }

    IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        if (playerUnit.pokemon.HP > 0)
        {
            yield return dialog.SetDialog($"Vuelve {playerUnit.pokemon.BasePokemon.Namae}");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(1.5f);
        }
        playerUnit.SetupPokemon(newPokemon);
        dialog.PlayerMovements(newPokemon.Moves);
        yield return dialog.SetDialog($"{newPokemon.BasePokemon.Namae} yo te eligo");
        StartCoroutine(PerformEnemyMove());
    }
}
