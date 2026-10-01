using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    [Header("Set Player Count")]
    [SerializeField, Range(1, 6)] private int playerCount = 2;

    private List<Player> players = new List<Player>();
    public IReadOnlyList<Player> Players => players;

    [SerializeField] private Material[] playerColours;
    public Material[] PlayerColours => playerColours;


    private void Start()
    {
        InitialisePlayers();
        StartGame();
    }

    private void InitialisePlayers()
    {
        players.Clear();

        for (int i = 0; i < playerCount; i++)
        {
            players.Add(new Player(i, $"Player {i + 1}", playerColours[i]));
        }

        Debug.Log($"Initialised {players.Count} players");
    }

    public Player GetPlayer(int index)
    {
        if (index < 0 || index >= players.Count)
        {
            Debug.LogError($"Invalid player index: {index}");
            return null;
        }

        return players[index];
    }

    private void StartGame()
    {
        HexGridManager.Instance.StartGeneration();
        SpawnLocationManager.Instance.InitiateSpawnLocations();
        UnitManager.Instance.CreateStartingUnits();
        TurnManager.Instance.StartGame();
    }
}
