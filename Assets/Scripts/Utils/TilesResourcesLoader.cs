using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public static class TilesResourcesLoader
{
    private const string PathHorizontal = "horizontal";
    private const string StartStop = "start_stop";    
    
    public static Tile GetGrassTile()
    {
        return Resources.Load("Tiles/grass") as Tile;
    }
    public static Tile GetPestTile()
    {
        return Resources.Load("Tiles/pest") as Tile;
    }

    public static Tile GetPlayerTile(string color)
    {
        Tile farmTile = Resources.Load("Tiles/" + color) as Tile;
        if(farmTile == null)
        {
            farmTile = Resources.Load("Tiles/default_farm") as Tile;
        }
        return farmTile;
    }
}

