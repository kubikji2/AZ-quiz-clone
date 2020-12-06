using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    // singleton pattern and awake method is based on solution found here:
    // - https://answers.unity.com/questions/891380/unity-c-singleton.html
    #region SINGLETON PATTERN
    private static GameManager _instance;
    public static GameManager Instance
    {
        get {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameManager>();
                
                if (_instance == null)
                {
                    GameObject container = new GameObject("GameManager");
                    _instance = container.AddComponent<GameManager>();
                }
            }
        
            return _instance;
        }
    }
    #endregion
    
    private void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;//Avoid doing anything else
        }
    
        _instance = this;
        //DontDestroyOnLoad( this.gameObject );
    }
    
    public enum EGameStates {CHOOSING, READING, ANSWERING, EVALUATION};

    private EGameStates _game_state;

    public int thinking_time = 7;

    public GameObject button_prefab;

    public GameObject timer_prefab;

    private Text _timer_text;
    
    private GameObject _timer_instance;

    /*********************
    * game board related *
    *********************/
    public Canvas game_canvas;

    private List<List<BoardTile>> _game_board;

    private GameObject _game_board_holder;

    // This is kinda mess, I know
    private List<string> labels = new List<string> {"A", "B", "C", "Č", "D", "E", "F", "G", "H", "Ch", "I", "J", "K", "L",
                                                    "M", "N", "O", "P", "R", "Ř", "S", "Š", "T", "U", "V", "W", "Z", "Ž" };

    
    // currently selected tile
    private BoardTile _current_tile;

    /***************
    * team related *
    ***************/

    public Team team1;

    public Team team2;

    public Color shot_out_color;

    private bool _is_first_team_playing;

    // Start is called before the first frame update
    void Start()
    {
        InitBoard();
        InitScore();
        _game_state = EGameStates.CHOOSING;
    }

    // generate game board
    private void InitBoard()
    {
        string arr = "";
        int cnt = 0;
        Vector2 screen_center = new Vector2 (Screen.currentResolution.width/2, Screen.currentResolution.height/2);

        // creating board tiles grid
        _game_board = new List<List<BoardTile>>();

        // creating game board holder to improve hearchy
        _game_board_holder = new GameObject("Game Board Holder");
        _game_board_holder.transform.SetParent(game_canvas.transform);
        // Iterate over the game board
        for (int row = 0; row < 7; row++)
        {
            List<BoardTile> row_tiles = new List<BoardTile>();

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

                // button instantiation
                GameObject current_button_go = Instantiate(button_prefab) as GameObject;
                current_button_go.name = current_label + " button";
                Button current_button = current_button_go.GetComponent<Button>();

                // board tile creation
                BoardTile current_board_tile = new BoardTile(current_button, current_label, new Vector2Int(i, row));
                row_tiles.Add(current_board_tile);

                // based on:
                // https://answers.unity.com/questions/1288510/buttononclickaddlistener-how-to-pass-parameter-or.html
                current_button.onClick.AddListener(delegate{OnBoardButtonClicked(current_board_tile);});
                
                // setting canvas as parent
                // based on:
                // https://stackoverflow.com/questions/36042439/add-items-to-canvas-unity-c-sharp
                current_button.transform.SetParent(_game_board_holder.transform);
                current_button.transform.position = position;
                current_button.GetComponentInChildren<Text>().text = current_label;

            }   
            arr += "\n";
            _game_board.Add(row_tiles);
        }

        Debug.Log(arr);
    }

    // initialize score table, timers and other stuff
    private void InitScore()
    {
        // set first team to start
        _is_first_team_playing = true;

        // display resolution
        Vector2 screen_center = new Vector2 (Screen.currentResolution.width/2, Screen.currentResolution.height/2);

        _timer_instance = Instantiate<GameObject>(timer_prefab);
        // set its position
        _timer_instance.transform.SetParent(game_canvas.transform);
        _timer_instance.transform.position = screen_center + new Vector2(0, screen_center.y/2);
        // extract text
        _timer_text = _timer_instance.GetComponent<Text>();

        // disables it
        _timer_instance.SetActive(false);
    }


    // Button callback for the button in the grid
    private void OnBoardButtonClicked(BoardTile my_tile)
    {
        if(_game_state == EGameStates.CHOOSING && (my_tile.state == BoardTile.EBoardTileState.NONE || my_tile.state == BoardTile.EBoardTileState.SHOTOUT))
        {
            Debug.Log("clicked on " + my_tile.label);
            // save button
            _current_tile = my_tile;
            SetGameState(EGameStates.READING);
        }

    }

    // just check for key start
    void Update()
    {
        if(_game_state == EGameStates.READING)
        {
            if(Input.GetKeyDown(KeyCode.S))
            {
                SetGameState(EGameStates.ANSWERING);
            }
        }
        
        // debug
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("[GAME MANAGER] " + _is_first_team_playing + ", GAMESTATE " + _game_state);
        }
    }

    // Tick a timer
    // TODO replace with a clock like stuff?
    private IEnumerator Timer(int remains) {
        
        _timer_instance.SetActive(true);
        _timer_text.text = remains+"";

        yield return new WaitForSeconds(1);
        // set text

        if(remains > 0)
        {
            StartCoroutine(Timer(remains-1));
        } else {
            // disables timer
            _timer_instance.SetActive(false);

            // set new game state
            SetGameState(EGameStates.EVALUATION);
        }
    }

    public void LastAnswerWas(bool correctness)
    {
        bool is_there_winner = false;
        if(correctness)
        {
            _current_tile.state = _is_first_team_playing ? BoardTile.EBoardTileState.PLAYER1 : BoardTile.EBoardTileState.PLAYER2;
            is_there_winner = CheckWinner( _is_first_team_playing ? BoardTile.EBoardTileState.PLAYER1 : BoardTile.EBoardTileState.PLAYER2);
        } else {
            _current_tile.state = BoardTile.EBoardTileState.SHOTOUT;
        }   

        Debug.Log(_current_tile.label + " " + _current_tile.state);

        // propagate playing teams
        _current_tile.UpdateButton(team1, team2, shot_out_color);

        // TODO handle case when there is a winner
        Debug.Log("[GAME MANAGER] " + is_there_winner);

        // start new round
        SetGameState(EGameStates.CHOOSING);

    }


    // 
    private void SetGameState(EGameStates new_game_state)
    {
        switch (new_game_state)
        {
            case EGameStates.CHOOSING:
                // switch team
                _is_first_team_playing = !_is_first_team_playing;
                _game_state = new_game_state;
            break;

            case EGameStates.READING:
                _game_state = new_game_state;
                // TODO display question?
            break;

            case EGameStates.ANSWERING:
                _game_state = new_game_state;
                //Debug.Log("Countdown started");

                // activate timer game object            
                _timer_instance.SetActive(true);
                // start countdown
                StartCoroutine(Timer(thinking_time));
            break;

            case EGameStates.EVALUATION:
                _game_state = new_game_state;
                // TODO show right answer in new window on the other screen if possible
                AnswerEvaluator.Instance.ShowQuery("<Zde bude správná odpoveď>");
                // TODO dialog window determine whether team answered correctly
            break;
            
            default:
                Debug.LogError("Something went horribly wrong in setting new game state!");
            break;
        }
    }

    
    // BFS on BF hex grid
    bool CheckWinner(BoardTile.EBoardTileState team)
    {
        // winning condition
        bool left = false;
        bool bottom = false;
        bool diagonal = false;

        Queue<BoardTile> open = new Queue<BoardTile>();
        List<BoardTile> closed = new List<BoardTile>();

        open.Enqueue(_current_tile);
        
        while(open.Count > 0)
        {
            BoardTile cur = open.Dequeue();
            // processing current tile
            left = cur.position_in_grid.x == 0 ? true : left;
            bottom = cur.position_in_grid.y == 6 ? true : bottom;
            diagonal = cur.position_in_grid.x == cur.position_in_grid.y ? true : diagonal;
            
            // check for winning condition
            if (left && bottom && diagonal)
            {
                return true;
            }

            // closing current tile
            closed.Add(cur);
            
            // processing neighbors
            List<BoardTile> neighs = GetNeighbors(cur.position_in_grid);
            foreach (BoardTile neigh in neighs)
            {
                if(!closed.Contains(neigh) && neigh.state == team)
                {
                    open.Enqueue(neigh);
                }
            }

        }

        //Debug.Log("left " + left + " bottom " + bottom + " diagonal " + diagonal);

        return false;

    }

    // expand given node
    private List<BoardTile> GetNeighbors(Vector2Int position)
    {
        List<BoardTile> neighs = new List<BoardTile>();
        
        List<Vector2Int> d_pos = new List<Vector2Int> {new Vector2Int(-1,-1), new Vector2Int(0,-1), new Vector2Int(-1,0), new Vector2Int(1,0), new Vector2Int(0,1), new Vector2Int(1,1)};
        foreach (Vector2Int dp in d_pos)
        {
            Vector2Int neigh_pos = position + dp;
            if(CheckBounds(neigh_pos))
            {
                // index order swapped contraintuitively
                neighs.Add(_game_board[neigh_pos.y][neigh_pos.x]);
            }
        }

        return neighs;
    }

    // check whether given position is within bounds
    private bool CheckBounds(Vector2Int position)
    {
        int x = position.x;
        int y = position.y;
        return x >= 0 && y >= 0 && x <= y && y < 7;

    }


}
