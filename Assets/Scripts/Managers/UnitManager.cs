using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoSingleton<UnitManager>
{
    [Header("Starting Unit")]
    [SerializeField] private UnitDefinitions startingUnit;
    [SerializeField] private UnitDefinitions boatUnit;

    [SerializeField] private int startingSoldierCount = 1;

    private GameObject[] playerUnitHolders;

    public void CreateStartingUnits()
    {
        int count = GameManager.Instance.Players.Count;
        playerUnitHolders = new GameObject[count];

        for (int i = 0; i < GameManager.Instance.Players.Count; i++)
        {
            playerUnitHolders[i] = new GameObject($"Player {i + 1}");
            playerUnitHolders[i].transform.SetParent(transform);
            
            Player player = GameManager.Instance.GetPlayer(i);

            CreateStartingUnitsForPlayer(player, i);

            if (SpawnLocationManager.Instance.TileSurroundedByOcean(player.startingTile, out List<HexTile> boatStartingTiles))
            {
                int randomTileIndex = Random.Range(0, boatStartingTiles.Count);

                CreateStartingBoat(player, boatStartingTiles[randomTileIndex], i);
            }
        }
    }

    private void CreateStartingUnitsForPlayer(Player player, int playerNumber)
    {
        for (int i = 0; i < startingSoldierCount; i++)
        {
            GameObject unitObject = Instantiate(startingUnit.Prefab, player.startingTile.transform.position, Quaternion.identity, playerUnitHolders[playerNumber].transform);

            unitObject.transform.localScale = new Vector3(5f, 5f, 5f);

            unitObject.name = $"Player{playerNumber + 1} : Soldier{i + 1}";

            Unit unit = unitObject.GetComponent<Unit>();

            unit.InitialiseUnit(startingUnit, player, player.startingTile);

            player.units.Add(unit);
        }
    }

    public void CreateStartingBoat(Player player, HexTile chosenTile, int playerNumber)
    {
        GameObject boatObject = Instantiate(boatUnit.Prefab, chosenTile.transform.position, Quaternion.identity, playerUnitHolders[playerNumber].transform);

        Unit boat = boatObject.GetComponent<Unit>();

        boat.InitialiseUnit(boatUnit, player, chosenTile);

        player.units.Add(boat);
    }
}
