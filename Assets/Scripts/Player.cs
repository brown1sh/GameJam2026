using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player
{
    public string playerName;
    public int playerID;
    public Material playerColour;

    public HexTile startingTile;

    public List<Unit> units = new List<Unit>();

    public Player(int id, string name, Material colour)
    {
        playerID = id;
        playerName = name;
        this.playerColour = colour;
    }
}
