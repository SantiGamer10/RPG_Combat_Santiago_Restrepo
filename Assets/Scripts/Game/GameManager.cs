using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<Player> players;

    [SerializeField] private ActionController ac;
    [SerializeField] private GameView view;

    [SerializeField] private bool enableLogs = false;

    private Player actualPlayer;

    private bool[] playerDeadFlags;

    private int playersTotal = 0;
    private int playerCounter = 0;
    private int enemiesCounter = 0;

    private int turn = 1;
    private int maxTurns = 0;

    private bool gameOver = false;

    private MovementController mc;

    public bool IsWaitingForMovement { set; get; } 

    private void Awake()
    {
        if (!ac)
        {
            Debug.LogError($"{name}: {nameof(ac)} is null!" +
                           $"\nCheck and assign one.");
            enabled = false;
            return;
        }

        if (!view)
        {
            Debug.LogError($"{name}: {nameof(view)} is null!" +
                           $"\nCheck and assign one.");
            enabled = false;
            return;
        }

        if (players.Count <= 0)
        {
            Debug.LogError($"{name}: There are no players in the players list!" +
               $"\nCheck and assign them,.");
            enabled = false;
            return;
        }
    }

    private void OnEnable()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].onDead += KillCounter;
        }
    }

    private void Start()
    {
        actualPlayer = players[0];

        view.SetCurrentPlayer(actualPlayer.gameObject);
        view.SetPlayers(players.ConvertAll(p => p.gameObject));

        ac.Initialize(players);
        ac.SetCurrentPlayer(actualPlayer);

        maxTurns = players.Count;

        playerDeadFlags = new bool[players.Count];

        for (int i = 0; i < playerDeadFlags.Length; i++)
        {
            playerDeadFlags[i] = false;

            if (players[i].IsEnemy) enemiesCounter++;
            else
            {
                playerCounter++;
                playersTotal++;
            }
        }

        IsWaitingForMovement = true;

        mc = new MovementController(view, new MapBuilder(), players.Count);
        StartCoroutine(PlayerTurn());
    }

    private void Update()
    {
        if (!IsWaitingForMovement || gameOver) return;

        if (actualPlayer.IsEnemy)
        {
            mc.MoveEnemyRandomly();
            return;
        }

        Dictionary<KeyCode, Action> movementActions = new Dictionary<KeyCode, Action>
        {
        {KeyCode.LeftArrow, mc.MoveCharacterLeft},
        {KeyCode.A, mc.MoveCharacterLeft},
        {KeyCode.RightArrow, mc.MoveCharacterRight},
        {KeyCode.D, mc.MoveCharacterRight},
        {KeyCode.UpArrow, mc.MoveCharacterUp},
        {KeyCode.W, mc.MoveCharacterUp},
        {KeyCode.DownArrow, mc.MoveCharacterDown},
        {KeyCode.S, mc.MoveCharacterDown}
        };

        foreach (var kvp in movementActions)
        {
            if (Input.GetKeyDown(kvp.Key))
            {
                kvp.Value.Invoke();
                break;
            }
        }
    }

    private IEnumerator PlayerTurn()
    {
        if (gameOver) yield break;

        bool currentPlayerIsDead = IsPlayerDead(turn);

        if (!currentPlayerIsDead)
        {
            UpdateCharacter();

            mc.UpdateCharacterPosition(turn);

            yield return WaitForMovement();
            yield return WaitForAction();

            mc.StoreCharacterPosition(turn);
        }

        CheckIfGameOver();
        turn = (turn % maxTurns) + 1;

        StartCoroutine(PlayerTurn());
    }

    private bool IsPlayerDead(int playerTurn)
    {
        return playerDeadFlags[playerTurn - 1];
    }

    private void UpdateCharacter()
    {
        actualPlayer = players[turn - 1];

        view.SetCurrentPlayer(actualPlayer.gameObject);
        ac.SetCurrentPlayer(actualPlayer);
    }

    private IEnumerator WaitForMovement()
    {
        IsWaitingForMovement = true;

        while (mc.Speed < players[turn - 1].GetMaxSpeed() && IsWaitingForMovement == true)
        {
            yield return new WaitForEndOfFrame();
        }

        IsWaitingForMovement = false;
    }

    private IEnumerator WaitForAction()
    {
        while (!ac.HasChosenAction)
        {
            yield return new WaitForEndOfFrame();
        }
    }

    private void CheckIfGameOver()
    {
        if (playerCounter == 1)
        {
            if (enableLogs)
                Debug.Log("PLAYER " + turn + " WINS!!!");

            gameOver = true;

            ac.gameObject.SetActive(false);
        }

        else if (playerCounter < playersTotal && enemiesCounter > 0)
        {
            if (enableLogs)
                Debug.Log("EVERYBODY LOSES!!!");

            gameOver = true;

            ac.gameObject.SetActive(false);
        }
    }

    private void KillCounter()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].HealthPoints == 0)
            {
                if (enableLogs)
                    Debug.Log("Player " + (i + 1) + " has died.");

                playerDeadFlags[i] = true;

                if (players[i].IsEnemy)
                {
                    enemiesCounter--;
                }

                else
                {
                    playerCounter--;
                }

                mc.RemovePositionAfterDeath(i, turn);
            }
        }
    }
}
