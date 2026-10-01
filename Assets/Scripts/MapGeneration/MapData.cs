using System.Collections.Generic;

[System.Serializable]
public class MapData
{
    public int columns;
    public int rows;

    public List<HexData> tiles = new List<HexData>();
}
