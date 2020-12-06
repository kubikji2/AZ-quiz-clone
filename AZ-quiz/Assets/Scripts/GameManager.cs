using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public GameObject button_prefab;

    public Canvas game_canvas;

    private List<List<BoardTile>> _game_board;

    // This is kinda mess, I know
    private List<string> labels = new List<string> {"A", "B", "C", "Č", "D", "E", "F", "G", "H", "Ch", "I", "J", "K", "L",
                                                    "M", "N", "O", "P", "R", "Ř", "S", "Š", "T", "U", "V", "W", "Z", "Ž" };

    // Start is called before the first frame update
    void Start()
    {
        InitBoard();
    }

    private void InitBoard()
    {
        string arr = "";
        int cnt = 0;
        Vector2 screen_center = new Vector2 (Screen.currentResolution.width/2, Screen.currentResolution.height/2);
        // Iterate over the game board
        for (int row = 0; row < 7; row++)
        {
            float off = (row/2f)*35;
            float delta = 35;
            for (int i = 0; i <= row; i++)
            {
                // label 
                string current_label = labels[cnt];
                cnt++;

                // position computation
                Vector2 position = screen_center;
                position += new Vector2(-off+delta*i,-delta*row);
                print(position);

                // button instantiation
                GameObject current_button_go = Instantiate(button_prefab) as GameObject;
                Button current_button = current_button_go.GetComponent<Button>();

                // board tile creation
                BoardTile current_board_tile = new BoardTile(current_button, current_label);

                // based on:
                // https://answers.unity.com/questions/1288510/buttononclickaddlistener-how-to-pass-parameter-or.html
                current_button.onClick.AddListener(delegate{OnBoardButtonClicked(current_board_tile);});
                
                // setting canvas as parent
                // based on:
                // https://stackoverflow.com/questions/36042439/add-items-to-canvas-unity-c-sharp
                current_button.transform.SetParent(game_canvas.transform);
                current_button.transform.position = position;
                current_button.GetComponentInChildren<Text>().text = current_label;

            }   
            arr += "\n";
        }

        Debug.Log(arr);
    }

    private void OnBoardButtonClicked(BoardTile my_tile)
    {
        Debug.Log(my_tile.label + " clicked");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
