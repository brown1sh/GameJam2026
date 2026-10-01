using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoSingleton<TurnManager>
{
    [SerializeField] private MapCameraController cameraController;

    private int currentPlayerIndex = 0; 
    public int CurrentPlayerIndex => currentPlayerIndex;

    public Player CurrentPlayer => GameManager.Instance.Players[currentPlayerIndex];

    public void StartGame()
    {
        DecideTurnOrder();
        StartTurn();
    }

    private void DecideTurnOrder()
    {
        // Randomise Starting Player
        //currentPlayerIndex = Random.Range(1, GameManager.Instance.Players.Count);
        //CurrentPlayer = GameManager.Instance.GetPlayer(CurrentPlayerIndex);
    }

    public void EndTurn()
    {
        currentPlayerIndex++;

        // Go back to player 1 after final player
        if (currentPlayerIndex >= GameManager.Instance.Players.Count)
        {
            currentPlayerIndex = 0;
        }

        StartTurn();
    }

    private void StartTurn()
    {
        Debug.Log($"It is now {CurrentPlayer.playerName}'s turn.");

        Player player = CurrentPlayer;

        foreach (Unit unit in player.units)
        {
            unit.StartTurn();
        }

        cameraController.FocusOn(player.startingTile);
    }
}
