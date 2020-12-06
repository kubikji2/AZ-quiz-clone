using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardTile
{
    public enum EBoardTileState {NONE, PLAYER1, PLAYER2, SHOTOUT};

    public EBoardTileState state;

    private Button _button;

    public string label;

    public BoardTile(Button button, string label)
    {
        _button = button;
        this.label = label;
    }

    // color setting based on:
    // https://forum.unity.com/threads/changing-the-color-of-a-ui-button-with-scripting.267093/
    public void UpdateButton(Team team1, Team team2, Color shotout_color)
    {
        ColorBlock colors = _button.colors;
        Color new_color = Color.black;
        switch (state)
        {
            case EBoardTileState.PLAYER1:
                new_color = team1.team_color;
                break;

            case EBoardTileState.PLAYER2:
                new_color = team2.team_color;
                break;

            
            case EBoardTileState.SHOTOUT:
                new_color = shotout_color;
                break;

            default:
                break;
        }
        // setting colors
        // DO NOT EVER TOUCH THIS COLOR SETUP
        colors.normalColor = Color.black;
        colors.normalColor += new_color;

        colors.highlightedColor = Color.black;
        colors.highlightedColor += new_color;

        colors.disabledColor = Color.black;
        colors.disabledColor += new_color;

        // deactivate in case of ownership by one team
        _button.interactable = !(state == EBoardTileState.PLAYER1 || state == EBoardTileState.PLAYER2);
        
        _button.colors = colors;
    }
}
