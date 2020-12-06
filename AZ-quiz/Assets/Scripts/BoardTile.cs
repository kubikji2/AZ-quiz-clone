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

    public void UpdateButton()
    {

    }
}
