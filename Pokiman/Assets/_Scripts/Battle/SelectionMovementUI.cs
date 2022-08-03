using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SelectionMovementUI : MonoBehaviour
{
    [SerializeField] private Text[] movementText;

    private int currentMoveReplaceSelected = 0;
    [SerializeField] Color selectedColor;

    public void SetMovement(List<MovementBase> pokemonMoves, MovementBase newMove)
    {
        currentMoveReplaceSelected = 0;

        for (var i = 0; i < pokemonMoves.Count; i++)
        {
            movementText[i].text = pokemonMoves[i].Namae;
        }
        movementText[pokemonMoves.Count].text = newMove.Namae;
    }

    public void HandleMoveReplaceSelection(Action<int> OnSelected)
    {
        if (Input.GetAxisRaw("Vertical") != 0)
        {
            int direction = Mathf.FloorToInt(Input.GetAxisRaw("Vertical"));
            currentMoveReplaceSelected -= direction;
            currentMoveReplaceSelected = Mathf.Clamp(currentMoveReplaceSelected, 0, PokemonBase.CANT_OF_LEARNABLE_MOVES);
            UpdateReplaceMoveSelector();
            OnSelected?.Invoke(-1);
        }

        if (Input.GetAxisRaw("Submit") != 0)
        {
            OnSelected?.Invoke(currentMoveReplaceSelected);
        }
    }

    public void UpdateReplaceMoveSelector()
    {
        for (var i = 0; i <= PokemonBase.CANT_OF_LEARNABLE_MOVES; i++)
        {
            movementText[i].color = (i == currentMoveReplaceSelected? selectedColor : Color.black);
        }
    }
}
