[System.Serializable]
public class HexData
{
    public int column;
    public int row;
    public TerrainType terrain;

    public HexData(int column, int row, TerrainType terrain)
    {
        this.column = column;
        this.row = row;
        this.terrain = terrain;
    }
}
