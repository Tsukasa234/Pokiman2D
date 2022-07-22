using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] private Text battleDialog;
    [SerializeField] private GameObject actionSelect;
    [SerializeField] private GameObject moveSelect;
    [SerializeField] private GameObject moveDescription;

    [SerializeField] private List<Text> moveTexts;
    [SerializeField] private List<Text> actionTexts;

    [SerializeField] private Text ppText;
    [SerializeField] private Text TypeText;

    [SerializeField] private Color colorSelection = Color.blue;

    public float characterPerSecond;
    public float timeToWaitAfterSecs = 1.0f;

    public bool isWriting = false;


    public IEnumerator SetDialog(string messagge)
    {
        isWriting = true;

        battleDialog.text = "";

        foreach (var character in messagge)
        {
            battleDialog.text += character;

            yield return new WaitForSeconds(1 / characterPerSecond);
        }
        yield return new WaitForSeconds(timeToWaitAfterSecs);
        isWriting = false;
    }

    public void ToggleDialogText(bool activated)
    {
        battleDialog.enabled = activated;
    }
    public void ToggleActions(bool activated)
    {
        actionSelect.SetActive(activated);
    }
    public void ToggleMovesText(bool activated)
    {
        moveSelect.SetActive(activated);
        moveDescription.SetActive(activated);
    }

    public void SelectAction(int selectedAction)
    {
        for (int i = 0; i < actionTexts.Count; i++)
        {
            actionTexts[i].color = (i == selectedAction ? colorSelection : Color.black);
        }
    }

    public void PlayerMovements(List<Move> moves)
    {
        for (int i = 0; i < moveTexts.Count; i++)
        {
            if (i < moves.Count)
            {
                moveTexts[i].text = moves[i].Base.Namae;
            }
            else
            {
                moveTexts[i].text = "-----";
            }
        }
    }

    public void SelectionMovement(int selectedMove, Move move)
    {
        for (int i = 0; i < moveTexts.Count; i++)
        {
            moveTexts[i].color = (i == selectedMove ? colorSelection : Color.black);
        }
        ppText.text = $"PP {move.PP}/{move.Base.Pp}";
        TypeText.text = $"{move.Base.TipoAtaque}";

        ppText.color = (move.PP <= 0? Color.red : Color.black);
    }
}
