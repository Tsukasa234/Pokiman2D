using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleStates
{
    StartBattle,
    PlayerSelectAction,
    PlayerMove,
    EnemyMove,
    Busy,
    PartySelectScreen,

}

public class BattleManager : MonoBehaviour
{
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleHUD playerHUD;

    [SerializeField] private BattleUnit enemyUnit;
    [SerializeField] private BattleHUD enemyHUD;

    [SerializeField] private BattleDialogBox dialog;
    [SerializeField] private PartyHUD partyHud;

    public BattleStates battleStates;

    public event Action<bool> OnPokemonBattleFinish;

    PokemonParty playerParty;
    Pokemon wildPokemon;




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

        if (battleStates == BattleStates.PlayerSelectAction)
        {
            HandlerPlayerSelection();
        }
        else if (battleStates == BattleStates.PlayerMove)
        {
            HandlePlayerMovementSelection();
        }
        else if (battleStates == BattleStates.PartySelectScreen)
        {
            HandlePartySelection();
        }
    }

    public IEnumerator SetupBattle()
    {
        battleStates = BattleStates.StartBattle;

        playerUnit.SetupPokemon(playerParty.GetFirstHealthyPokemon());
        playerHUD.SetPokemonData(playerUnit.pokemon);
        dialog.PlayerMovements(playerUnit.pokemon.Moves);

        enemyUnit.SetupPokemon(wildPokemon);
        enemyHUD.SetPokemonData(enemyUnit.pokemon);

        partyHud.InitPartyHUD();

        yield return dialog.SetDialog($"Un {enemyUnit.pokemon.BasePokemon.Namae} salvaje ha aparecido");
        if (enemyUnit.pokemon.Speed > playerUnit.pokemon.Speed)
        {
            StartCoroutine(dialog.SetDialog("El enemigo ataca primero"));
            StartCoroutine(EnemyAction());
        }
        else
        {
            PlayerAction();
        }

    }

    private void PlayerAction()
    {
        battleStates = BattleStates.PlayerSelectAction;
        StartCoroutine(dialog.SetDialog("Seleciona una accion"));
        dialog.ToggleMovesText(false);
        dialog.ToggleDialogText(true);
        dialog.ToggleActions(true);
        currentSelectionAction = 0;
        dialog.SelectAction(currentSelectionAction);
    }

    private void PlayerMovement()
    {
        battleStates = BattleStates.PlayerMove;
        dialog.ToggleDialogText(false);
        dialog.ToggleActions(false);
        dialog.ToggleMovesText(true);
        playerMovementSelection = 0;
        dialog.SelectionMovement(currentSelectionAction, playerUnit.pokemon.Moves[playerMovementSelection]);
    }

    IEnumerator EnemyAction()
    {
        battleStates = BattleStates.EnemyMove;
        Move move = enemyUnit.pokemon.randomMove();
        move.PP--;

        var oldHPValue = playerUnit.pokemon.HP;

        yield return dialog.SetDialog($"{enemyUnit.pokemon.BasePokemon.Namae} salvaje ha usado {move.Base.Namae}");
        enemyUnit.PlayAttackAnimation();
        playerUnit.PlayReceiveDamageAnimation();
        yield return new WaitForSeconds(1.0f);
        var damageDesc = playerUnit.pokemon.ReceiveDamage(move, enemyUnit.pokemon);

        playerHUD.UpdatePokemonData(oldHPValue);
        yield return ShowDamageDescription(damageDesc);

        if (damageDesc.Fainted)
        {
            yield return dialog.SetDialog($"{playerUnit.pokemon.BasePokemon.Namae} se ha debilitado");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(1.5f);

            var nextPokemon = playerParty.GetFirstHealthyPokemon();
            if (nextPokemon == null) // Si es null no nos queda ningun pokemon con vida
            {
                OnPokemonBattleFinish(false);
            }
            else //Tengo que sacar otro pokemon con vida
            {
                playerUnit.SetupPokemon(nextPokemon);
                playerHUD.SetPokemonData(nextPokemon);

                dialog.PlayerMovements(nextPokemon.Moves);

                yield return dialog.SetDialog($"Adelante {nextPokemon.BasePokemon.Namae}!");
                PlayerAction();
            }

        }
        else
        {
            PlayerAction();
        }
    }



    private int currentSelectionAction;
    private float timeSinceLastClick;
    private float timeBeetweenClicks = 0.3f;

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
                PlayerMovement();
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

    private int playerMovementSelection;
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
            PlayerAction();
        }
    }

    private int currentSelectedPokemon;
    public void HandlePartySelection()
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
            PlayerAction();
        }
    }

    IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        yield return dialog.SetDialog($"Vuelve {playerUnit.pokemon.BasePokemon.Namae}");
        playerUnit.PlayFaintAnimation();
        yield return new WaitForSeconds(1.5f);
        playerUnit.SetupPokemon(newPokemon);
        playerHUD.SetPokemonData(newPokemon);
        dialog.PlayerMovements(newPokemon.Moves);
        yield return dialog.SetDialog($"{newPokemon.BasePokemon.Namae} yo te eligo");
        StartCoroutine(EnemyAction());
    }

    IEnumerator PerformPlayerMovement()
    {
        Move move = playerUnit.pokemon.Moves[playerMovementSelection];
        var oldHPValue = enemyUnit.pokemon.HP;
        move.PP--;
        yield return dialog.SetDialog($"{playerUnit.pokemon.BasePokemon.Namae} ha usado {move.Base.name}");
        playerUnit.PlayAttackAnimation();
        enemyUnit.PlayReceiveDamageAnimation();
        yield return new WaitForSeconds(1.0f);
        var damageDesc = enemyUnit.pokemon.ReceiveDamage(move, playerUnit.pokemon);
        enemyHUD.UpdatePokemonData(oldHPValue);
        yield return ShowDamageDescription(damageDesc);

        if (damageDesc.Fainted)
        {
            yield return dialog.SetDialog($"{enemyUnit.pokemon.BasePokemon.Namae} salvaje se ha debilitado");
            enemyUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(1.5f);
            OnPokemonBattleFinish(true);
        }
        else
        {
            StartCoroutine(EnemyAction());
        }
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

    public void OpenPartySelectionScreen()
    {
        battleStates = BattleStates.PartySelectScreen;
        partyHud.SetPartyPokemon(playerParty.Pokemons);
        partyHud.gameObject.SetActive(true);
        currentSelectedPokemon = 0;
        for (var i = 0; i < playerParty.Pokemons.Count; i++)
        {
            if (playerUnit.pokemon == playerParty.Pokemons[i])
            {
                currentSelectedPokemon = i;
            }
        }
        partyHud.UpdateSelectedPokemon(currentSelectedPokemon);
    }

    public void OpenInventoryScreen()
    {
        print("Abrir la pantalla de inventario");
    }
}
