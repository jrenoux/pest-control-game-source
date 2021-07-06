using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;



public class World 
{
    public int minX {get; set;}
    public int maxX {get; set;}
    public int minY {get; set;}
    public int maxY {get; set;}

    public string humanPlayer {get; set;}
    public Dictionary<string, Location> farmsLocations {get; set;}
    public List<Location> pestProgressionPattern {get; set;}
    public List<GridTile> currentPestProgression {get;}

    private PestController pestController;

    public Location initialPestLocation {get; set;}

    private HashSet<GridTile> tileList;

    private System.Random random;

    private Tilemap tilemap;

    private Tile pestTileObject =  TilesResourcesLoader.GetPestTile();

    public enum PestControllerTypes {
        Scripted,
        Random,
    }
    
    private PestControllerTypes type;

    public World() {
        tileList = new HashSet<GridTile>();
        currentPestProgression = new List<GridTile>();
    }

    public void Init(Tilemap tm, System.Random r)
    {
        this.tilemap = tm;
        this.random = r;
        //1. Create the farms in the tile list
        foreach(string playerColor in farmsLocations.Keys)
        {
            Location playerFarmLocation = farmsLocations[playerColor];
            Tile playerTileObject = TilesResourcesLoader.GetPlayerTile(playerColor);
            // creates the tile
            GridTile playerTile = new GridTile(GridTile.GridTileType.FARM, playerFarmLocation);
            tileList.Add(playerTile);

            // paint it
            this.tilemap.SetTile(playerTile.coordinates, playerTileObject);
        
        }

        //2. Create the initial pest location
        GridTile pestTile = new GridTile(GridTile.GridTileType.PEST, initialPestLocation);
        // add it in the list
        tileList.Add(pestTile);
        currentPestProgression.Add(pestTile);

        // paint it
        this.tilemap.SetTile(pestTile.coordinates, pestTileObject);

        // 3. Create the rest as grass tiles
        Tile grassTileObject = TilesResourcesLoader.GetGrassTile();
        Debug.Log("MinX = " + minX + ", MaxX = " + maxX);
        Debug.Log("MinY = " + minY + ", MaxY = " + maxY);
        for(int x = minX ; x <= maxX ; x++)
        {
            for(int y = minY ; y <= maxY ; y++)
            {
                Debug.Log("x = " + x + ", y = " + y);
                GridTile grassTile = new GridTile(GridTile.GridTileType.GRASS, new Location(x, y));
                if(!tileList.Contains(grassTile))
                {
                    tileList.Add(grassTile);
                    tilemap.SetTile(grassTile.coordinates, grassTileObject);
                }
            }
        }


        // 3. check if pestProgression is present of not (if non present: random, if not: scripted)
        if(this.pestProgressionPattern == null)
        {   
            // random
            type = PestControllerTypes.Random;
            Debug.Log("It's random");
        }
        else
        {
            // scripted
            type = PestControllerTypes.Scripted;
            Debug.Log("It's scripted");
        }

        // Put the camera at center and right zoom to see the whole map
        // TODO

    }

    public void GetNeighbors(GridTile origin)
    {
        // TODO
    }
    
    public GridTile GetNextPestTile() 
    {
        switch(type) 
        {
            case PestControllerTypes.Random:
            return  GetNextPestTileRandom();

            case PestControllerTypes.Scripted:
            return GetNextPestTileScripted();
        }
        return null; // weird that this is needed
    }

    private GridTile GetNextPestTileRandom()
    {
        Debug.Log("GetNextPestTileRandom");
        // find all the "candidates" i.e. all the tiles non pested and adjacent to a pested tile
        List<GridTile> candidates = FindCandidateTiles();

        foreach(var c in candidates)
        {
            Debug.Log("Candidate: " + c);
        }
        // select a tile among the candidates
        int index = random.Next(0, candidates.Count);
        GridTile newPestTile = candidates[index];

        // add the tile to the pestStatus
        currentPestProgression.Add(newPestTile);

        // change the pest type in the tile list
        newPestTile.type = GridTile.GridTileType.PEST;
    
        // draw it
        this.tilemap.SetTile(newPestTile.coordinates, pestTileObject);

        // return the tile
        return newPestTile;
    }

    private GridTile GetNextPestTileScripted()
    {
        return null;
    }

    private List<GridTile> FindCandidateTiles()
    {
        Debug.Log("FindCandidateTiles");
        List<GridTile> candidates = new List<GridTile>();
       foreach(var pestedTile in currentPestProgression)
       {
           Debug.Log(pestedTile);
           foreach(var neighbor in Neighbors(pestedTile))
           {
               if(!candidates.Contains(neighbor) && !this.currentPestProgression.Contains(neighbor))
               {
                   candidates.Add(neighbor);
               }
           }
       }

        return candidates;
    }

    static Vector3Int
    LEFT = new Vector3Int(-1, 0, 0),
    RIGHT = new Vector3Int(1, 0, 0),
    DOWN = new Vector3Int(0, -1, 0),
    DOWNLEFT = new Vector3Int(-1, -1, 0),
    DOWNRIGHT = new Vector3Int(1, -1, 0),
    UP = new Vector3Int(0, 1, 0),
    UPLEFT = new Vector3Int(-1, 1, 0),
    UPRIGHT = new Vector3Int(1, 1, 0);

    static Vector3Int[] directions_when_y_is_even = 
    { LEFT, RIGHT, DOWN, DOWNLEFT, UP, UPLEFT };
    static Vector3Int[] directions_when_y_is_odd = 
      { LEFT, RIGHT, DOWN, DOWNRIGHT, UP, UPRIGHT };

    public IEnumerable<GridTile> Neighbors(GridTile tile) {
        Vector3Int[] directions = (tile.coordinates.y % 2) == 0? 
            directions_when_y_is_even: 
            directions_when_y_is_odd;
        foreach (var direction in directions) {
            Vector3Int neighborPos = tile.coordinates + direction;
            if(IsInGrid(neighborPos))
            {
                yield return GetTileFromCoordinates(neighborPos);
            }
        }
    }

    private bool IsInGrid(Vector3Int tile)
    {
        if(tile.x >= minX && tile.x <= maxX
        && tile.y >= minY && tile.y <= maxY)
        {
            return true;
        } 
        return false;
    } 

    public GridTile GetTileFromCoordinates(Vector3Int coord)
    {
        foreach(GridTile t in tileList)
        {
            if(t.coordinates == coord)
            {
                return t;
            }
        }
        return null;
    }
}

public class Location
{
    public int x {get; set;}
    public int y {get; set;}

    public Location(int x, int y)
    {
        this.x = x;
        this.y = y;
    }


}

public class GridTile
{
    public enum GridTileType {
        GRASS, 
        PEST, 
        FARM
    }
    public Vector3Int coordinates {get;}
    public GridTileType type {get; set;}
    public GridTile(GridTileType tileType, Location location)
    {
        this.type = tileType;
        this.coordinates  = new Vector3Int(location.x, location.y, 0);
    }

    // override object.Equals
    public override bool Equals(object obj)
    {
        //
        // See the full list of guidelines at
        //   http://go.microsoft.com/fwlink/?LinkID=85237
        // and also the guidance for operator== at
        //   http://go.microsoft.com/fwlink/?LinkId=85238
        //
        
        if (obj == null)
        {
            return false;
        }
        
        var other = obj as GridTile;
        if(this.coordinates == other.coordinates)
        {
            return true;
        }
        return false;
    }
    
    // override object.GetHashCode
    public override int GetHashCode()
    {
        return this.coordinates.GetHashCode();
    }

    public override string ToString()
    {
        return "(" + type + ")(" + coordinates + ")";
    }
    
}