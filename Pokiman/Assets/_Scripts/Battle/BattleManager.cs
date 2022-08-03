using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public enum BattleStates
{
    StartBattle,
    ActionSelection,
    MoveSelection,
    PerformMove,
    Busy,
    PartySelectScreen,
    ItemSelectScreen,
    MoveReplace,
    LoseTurn,
    FinishBattle,
}

public enum BattleType
{
    WildPokemon,
    Trainer,
    Leader
}

public class BattleManager : MonoBehaviour
{
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleUnit enemyUnit;

    [SerializeField] private BattleDialogBox dialog;
    [SerializeField] private PartyHUD partyHud;
    [SerializeField] private SelectionMovementUI moveSelection;
    [SerializeField] private GameObject pokeball;

    [SerializeField] private BattleStates battleStates;
    [SerializeField] private BattleType battleType;

    PokemonParty playerParty;
    Pokemon wildPokemon;

    private int currentSelectionAction;
    private float timeSinceLastClick;
    [SerializeField] private float timeBeetweenClicks = 0.3f;
    private int playerMovementSelection;
    private int currentSelectedPokemon;
    private int escapeAttempts;

    private MovementBase moveToLearn;

    public event Action<bool> OnPokemonBattleFinish;


    public void HandleStartBattle(PokemonParty playerParty, Pokemon wildPokemon)
    {

        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;
        escapeAttempts = 0;
        StartCoroutine(SetupBattle());
    }

    public void HandleUpdate()
    {
        timeSinceLastClick += Time.deltaTime;

        if (timeSinceLastClick < timeBeetweenClicks || dialog.isWriting)
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
        else if (battleStates == BattleStates.LoseTurn)
        {
            StartCoroutine(PerformEnemyMove());
        }
        else if (battleStates == BattleStates.MoveReplace)
        {
            moveSelection.HandleMoveReplaceSelection(
                (moveIndex) =>
                {
                    if (moveIndex < 0)
                    {
                        timeSinceLastClick = 0;
                        return;
                    }
                    StartCoroutine(ForgetOldMove(moveIndex));
                });
        }
    }

    IEnumerator ForgetOldMove(int moveIndex)
    {
        moveSelection.gameObject.SetActive(false);
                    if (moveIndex == PokemonBase.CANT_OF_LEARNABLE_MOVES)
                    {
                        yield return dialog.SetDialog($"{playerUnit.pokemon.BasePokemon.Namae} no ha podido aprender {moveToLearn.Namae}");
                    }
                    else
                    {
                        var movementSelected = playerUnit.pokemon.Moves[moveIndex].Base;
                        yield return dialog.SetDialog($"{playerUnit.pokemon.BasePokemon.Namae} ha olvidado {movementSelected}");
                        yield return dialog.SetDialog($"y aprendio {moveToLearn.Namae}");
                        playerUnit.pokemon.Moves[moveIndex] = new Move(moveToLearn);
                    }

                    moveToLearn = null;
                    battleStates = BattleStates.FinishBattle;
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
            dialog.ToggleDialogText(true);
            dialog.ToggleActions(false);
            dialog.ToggleMovesText(false);
            yield return dialog.SetDialog("El enemigo ataca primero");
            yield return PerformEnemyMove();
        }
        else
        {
            PlayerActionSelector();
        }
    }

    public void HandleStartTrainerBattle(PokemonParty playerParty, PokemonParty trainerParty, bool isLeader)
    {
        battleType = (isLeader ? BattleType.Trainer : BattleType.Leader);
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
        StartCoroutine(ThrowPokeball());
        dialog.ToggleActions(false);
        print("Abrir la pantalla de inventario");
    }

    private void HandlerPlayerSelection()
    {
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
                StartCoroutine(TryToEscape());
            }
        }

        if (Input.GetAxisRaw("Return") != 0)
        {
            OnPokemonBattleFinish(false);
        }
    }


    public void HandlePlayerMovementSelection()
    {
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
            yield return HandlePokemonFainted(target);
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

    IEnumerator ThrowPokeball()
    {
        battleStates = BattleStates.Busy;

        if (battleType != BattleType.WildPokemon)
        {
            yield return dialog.SetDialog("El pokemon no puede ser capturado");
            battleStates = BattleStates.LoseTurn;
            yield break;
        }

        yield return dialog.SetDialog($"Has Lanzado {pokeball.name}");
        var pokeballIns = Instantiate(pokeball, playerUnit.transform.position - new Vector3(2f, 0f), Quaternion.identity);
        var pokeballSpt = pokeballIns.GetComponent<SpriteRenderer>();

        yield return pokeballSpt.transform.DOLocalJump(enemyUnit.transform.position + new Vector3(0, 1.5f), 2f, 1, 1.0f)
                                    .WaitForCompletion();

        yield return enemyUnit.PlayCatchPokemonAnimation();
        yield return pokeballSpt.transform.DOLocalMoveY(enemyUnit.transform.position.y - 1.5f, 1).WaitForCompletion();

        var numberOfShakes = TryToCatchPokemon(enemyUnit.pokemon);

        for (int i = 0; i < MathF.Min(numberOfShakes, 3); i++)
        {
            yield return new WaitForSeconds(0.5f);
            yield return pokeballSpt.transform.DOPunchRotation(new Vector3(0, 0, 25.0f), 1f).WaitForCompletion();
        }
        if (numberOfShakes == 4)
        {
            yield return dialog.SetDialog($"{enemyUnit.pokemon.BasePokemon.Namae} ha sido capturado");
            yield return pokeballSpt.DOFade(0, 1f).WaitForCompletion();
            if (playerParty.AddPokemonToParty(enemyUnit.pokemon))
            {
                yield return dialog.SetDialog($"{enemyUnit.pokemon.BasePokemon.Namae} se ha agregado a la party");
            }
            else
            {
                yield return dialog.SetDialog($"{enemyUnit.pokemon.BasePokemon.Namae} se ha agregado al PC");
            }
            Destroy(pokeballIns);
            BattleFinish(true);
        }
        else
        {
            yield return new WaitForSeconds(1.0f);
            pokeballSpt.DOFade(0, 0.2f);
            yield return enemyUnit.PlayFailCatchPokemonAnimation();

            if (numberOfShakes < 2)
            {
                yield return dialog.SetDialog($"{enemyUnit.pokemon.BasePokemon.Namae} ha escapado");
            }
            else
            {
                yield return dialog.SetDialog($"Casi lo has atrapado");
            }
            battleStates = BattleStates.LoseTurn;
            Destroy(pokeballIns);
        }
    }

    int TryToCatchPokemon(Pokemon pPokemon)
    {
        float bonusPokeball = 1;//TODO Class pokeball con su modificador
        float bonusStat = 1; //TODO Implementar los estados para los pokemons
        float a = (3 * pPokemon.MaxHP - 2 * pPokemon.HP) * pPokemon.BasePokemon.CatchRate *
                    bonusPokeball * bonusStat / (3 * pPokemon.MaxHP);

        if (a > 255)
        {
            return 4;
        }

        float b = 1048560 / MathF.Sqrt(MathF.Sqrt(16711680 / a));

        int shakesCount = 0;
        while (shakesCount < 4)
        {
            if (Random.Range(0, 65535) >= b)
            {
                break;
            }
            else shakesCount++;
        }

        return shakesCount;
    }

    IEnumerator TryToEscape()
    {
        battleStates = BattleStates.Busy;
        if (battleType != BattleType.WildPokemon)
        {
            yield return dialog.SetDialog("No puedes huir de batallas contra entrenadores");
            battleStates = BattleStates.LoseTurn;
            yield break;
        }

        escapeAttempts++;
        int playerSpeed = playerUnit.pokemon.Speed;
        int enemySpeed = enemyUnit.pokemon.Speed;

        if (playerSpeed >= enemySpeed)
        {
            yield return dialog.SetDialog("Has escapado con exito");
            yield return new WaitForSeconds(1);
            OnPokemonBattleFinish(true);
        }
        else
        {
            int oddEscape = (Mathf.FloorToInt(playerSpeed * 128 / enemySpeed) + 30 * escapeAttempts) % 256;
            if (Random.Range(0, 256) < oddEscape)
            {
                yield return dialog.SetDialog("Has escapado con exito");
                yield return new WaitForSeconds(1);
                OnPokemonBattleFinish(true);
            }
            else
            {
                yield return dialog.SetDialog("Has fallado en huir");
                battleStates = BattleStates.LoseTurn;
            }
        }
    }

    IEnumerator HandlePokemonFainted(BattleUnit faintedUnit)
    {
        yield return dialog.SetDialog($"{faintedUnit.pokemon.BasePokemon.Namae} se ha debilitado");
        faintedUnit.PlayFaintAnimation();
        yield return new WaitForSeconds(1.5f);

        if (!faintedUnit.IsPlayer)
        {
            int baseExp = faintedUnit.pokemon.BasePokemon.XpBase;
            int level = faintedUnit.pokemon.Level;
            float multiplier = (battleType == BattleType.WildPokemon ? 1f : 1.5f);
            int expGain = Mathf.FloorToInt(baseExp * level * multiplier / 7);
            playerUnit.pokemon.Experience += expGain;
            yield return dialog.SetDialog($"{playerUnit.pokemon.BasePokemon.Namae} ha ganado {expGain} puntos de exp");
            yield return playerUnit.HUD.SetSmoothExp();
            yield return new WaitForSeconds(1f);

            while (playerUnit.pokemon.NeedsToLevelUp())
            {
                playerUnit.HUD.SetLevelText();
                yield return playerUnit.HUD.UpdatePokemonData(playerUnit.pokemon.HP);
                yield return dialog.SetDialog($"¡¡{playerUnit.pokemon.BasePokemon.Namae} ha subido de nivel!!");
                var newLearnableMove = playerUnit.pokemon.GetLearnableMoveAtCurrentLevel();

                if (newLearnableMove != null)
                {
                    if (playerUnit.pokemon.Moves.Count < PokemonBase.CANT_OF_LEARNABLE_MOVES)
                    {
                        playerUnit.pokemon.LearnMove(newLearnableMove);
                        yield return dialog.SetDialog($"{playerUnit.pokemon.BasePokemon.Namae} ha aprendido {newLearnableMove.Move.Namae}");
                        dialog.PlayerMovements(playerUnit.pokemon.Moves);
                    }
                    else
                    {
                        yield return dialog.SetDialog($"{playerUnit.pokemon.BasePokemon.Namae} intenta aprender {newLearnableMove.Move.Namae}");
                        yield return dialog.SetDialog($"Pero no puede aprender mas de {PokemonBase.CANT_OF_LEARNABLE_MOVES} movimientos");
                        yield return ChooseMovementToReplace(playerUnit.pokemon, newLearnableMove.Move);
                        yield return new WaitUntil(() => battleStates != BattleStates.MoveReplace);
                    }
                }

                yield return playerUnit.HUD.SetSmoothExp(true);
            }
        }

        CheckForBattleFinish(faintedUnit);
    }

    IEnumerator ChooseMovementToReplace(Pokemon learner, MovementBase newMove)
    {
        battleStates = BattleStates.Busy;

        yield return dialog.SetDialog("Selecciona el movimiento que quieres olvidar");
        moveSelection.gameObject.SetActive(true);

        moveSelection.SetMovement(learner.Moves.Select(mv => mv.Base).ToList(), newMove);
        moveToLearn = newMove;
        battleStates = BattleStates.MoveReplace;
    }
}
