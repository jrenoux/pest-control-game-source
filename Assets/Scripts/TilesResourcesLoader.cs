using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public static class TilesResourcesLoader
{
    private const string PathHorizontal = "horizontal";
    private const string StartStop = "start_stop";    
    
    public static Tile GetPestTile()
    {
        return Resources.Load("Tiles/sand_E") as Tile;
    }
}

