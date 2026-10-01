using System.IO;
using UnityEngine;

public static class MapSaveSystem
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "GreeceMap.json");

    public static void SaveMap(HexGridManager hexGridManager)
    {
        MapData mapData = new MapData();

        mapData.columns = HexGridManager.Instance.Columns;
        mapData.rows = HexGridManager.Instance.Rows;

        foreach (HexTile tile in HexGridManager.Instance.GetComponentsInChildren<HexTile>())
        {
            HexData data = new HexData(tile.column, tile.row, tile.Terrain);

            mapData.tiles.Add(data);
        }

        string json = JsonUtility.ToJson(mapData, true);

        File.WriteAllText(SavePath, json);

        Debug.Log($"Map Saved to: {SavePath}");
    }

    public static MapData LoadMap()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log($"No saved map found.");
            return null;
        }

        string json = File.ReadAllText(SavePath);

        MapData mapData = JsonUtility.FromJson<MapData>(json);

        Debug.Log("Map loaded.");
        
        return mapData;
    }

    public static bool SaveExists()
    {
        return File.Exists(SavePath);
    }
}
