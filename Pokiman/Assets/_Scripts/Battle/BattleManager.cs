using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleStates
{
    StartBattle,
    PlayerSelectAction,
    PlayerMove,
    EnemyMove,
    Busy

}

public class BattleManager : MonoBehaviour
{
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleHUD playerHUD;

    [SerializeField] private BattleUnit enemyUnit;
    [SerializeField] private BattleHUD enemyHUD;

    [SerializeField] private BattleDialogBox dialog;
    public BattleStates battleStates;

    private void Start()
    {
        StartCoroutine(SetupBattle());
    }
    private void Update()
    {
        if (battleStates == BattleStates.PlayerSelectAction)
        {
            HandlerPlayerSelection();
            timeSinceLastClick += Time.deltaTime;
        }
        else if (battleStates == BattleStates.PlayerMove)
        {
            HandlePlayerMovementSelection();
            timeSinceLastClick += Time.deltaTime;
        }
    }

    public IEnumerator SetupBattle()
    {
        battleStates = BattleStates.StartBattle;

        playerUnit.SetupPokemon();
        playerHUD.SetPokemonData(playerUnit.pokemon);
        dialog.PlayerMovements(playerUnit.pokemon.Moves);

        enemyUnit.SetupPokemon();
        enemyHUD.SetPokemonData(enemyUnit.pokemon);

        yield return dialog.SetDialog($"Un {enemyUnit.pokemon.BasePokemon.Namae} salvaje ha aparecido");
        if (enemyUnit.pokemon.Speed > playerUnit.pokemon.Speed)
        {
            StartCoroutine(dialog.SetDialog("El enemigo ataca primero"));
            EnemyAction();
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

        yield return dialog.SetDialog($"{enemyUnit.pokemon.BasePokemon.Namae} ha usado {move.Base.Namae}");

        bool pokemonFainted = playerUnit.pokemon.ReceiveDamage(move, enemyUnit.pokemon);

        if (pokemonFainted)
        {
            yield return dialog.SetDialog($"{playerUnit.pokemon.BasePokemon.Namae} se ha debilitado");
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
            currentSelectionAction = (currentSelectionAction + 1) % 2;
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
                //TODO: Implementar la batalla
            }
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
            if (playerMovementSelection <= 1)
            {
                playerMovementSelection = (playerMovementSelection + 1) % 2;
            }
            else
            {
                playerMovementSelection = (playerMovementSelection + 1) % 2 + 2;
            }
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
    }

    IEnumerator PerformPlayerMovement()
    {
        Move move = playerUnit.pokemon.Moves[playerMovementSelection];
        yield return dialog.SetDialog($"{playerUnit.pokemon.BasePokemon.Namae} ha usado {move.Base.name}");
        bool pokemonFainted = enemyUnit.pokemon.ReceiveDamage(move, playerUnit.pokemon);

        if (pokemonFainted)
        {
            yield return dialog.SetDialog($"{enemyUnit.pokemon.BasePokemon.Namae} se ha debilitado");
        }
        else
        {
            StartCoroutine(EnemyAction());
        }
    }
}
