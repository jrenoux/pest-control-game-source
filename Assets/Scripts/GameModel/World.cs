using System;
using System.Collections.Generic;
using UnityEngine;

public class World 
{
    public int minX {get; set;}
    public int maxX {get; set;}
    public int minY {get; set;}
    public int maxY {get; set;}

    public int maxYear {get; set;}

    public bool tutorial {get; set;} = true;

    public double easeOfPestControl {get; set;} = 0.01;
    public Player humanPlayer {get; set;}

    public PestProgression pestProgression {get; set;}


    public HashSet<GridTile> tileList {get;}

    private System.Random random;

    public List<Player> activePlayers {get; set;}

    public int currentYear {get; set;} 

    private int pestSpreadingIndex;

    
    public World() {
        tileList = new HashSet<GridTile>();
        random = RandomSingleton.GetInstance();
        humanPlayer = null;
        currentYear = 1;
        pestSpreadingIndex = 0;
    }

    public void Init()
    {
        // make some routine check
        if(!ValidateConfig())
        {
            return;
        }

        //1. Create the farms in the tile list
        foreach(Player player in activePlayers)
        {
            // creates the tile
            GridTile playerTile = new GridTile(GridTile.GridTileType.FARM, player.farmLocation, player.id);
            tileList.Add(playerTile);
        
        }

        //2. Create the initial pest location
        GridTile pestTile = new GridTile(GridTile.GridTileType.PEST, pestProgression.initialPestLocation);
        // add it in the list
        tileList.Add(pestTile);
        pestProgression.currentPestProgression.Add(pestTile);

        // 3. Create the rest as grass tiles
        for(int x = minX ; x <= maxX ; x++)
        {
            for(int y = minY ; y <= maxY ; y++)
            {
                GridTile grassTile = new GridTile(GridTile.GridTileType.GRASS, new Location(x, y));
                if(!tileList.Contains(grassTile))
                {
                    tileList.Add(grassTile);
                }
            }
        }

        // Put the camera at center and right zoom to see the whole map
        // TODO

        // set the world reference for each player and store   the human player for easier access
        foreach (Player player in activePlayers)
        {
            if(player.type.Equals("human"))
            {
                humanPlayer = player;
            }
            player.theWorld = this;
        }
        if(humanPlayer == null)
        {
            Debug.LogError("No human player found");    
        }
    
    }

    public bool ValidateConfig()
    {
        // test that if the pest says scripted, then all info is here
        if(pestProgression.type.Equals("scripted")) 
        {
            if(pestProgression.pattern == null || pestProgression.years == null)
            {
                Debug.LogError("Pest progression set to scripted, but pattern or years field empty");
                return false;
            }
            if(pestProgression.years.Count != pestProgression.pattern.Count)
            {
                Debug.LogError("Pest progression pattern and years have different size");
                return false;
            }
            if(pestProgression.pattern.Count > maxYear)
            {
                Debug.LogError("Pest progression pattern and years is higher than the max number of years");
                return false;
            }
        }
        
        if(pestProgression.type.Equals("semiscripted") 
        && pestProgression.pattern == null)
        {
            Debug.LogError("Pest progression set to semi-scripted, but pattern field is empty");
            return false;
        }

        return true;
    } 

    public void SpawnPestTile(GridTile pestTile) 
    {
        pestProgression.latestPestSpread = pestTile;
        if(pestTile != null)
        {
            // add the tile to the pestStatus
            pestProgression.currentPestProgression.Add(pestTile);

            // change the pest type in the tile list
            pestTile.type = GridTile.GridTileType.PEST;

            // we check if the pest reached a player
            bool found = false;
            Player playerToRemove = null;
            foreach(Player player in activePlayers)
            {
                if(player.farmLocation.x == pestTile.coordinates.x 
                && player.farmLocation.y == pestTile.coordinates.y)
                {
                    // it has reached the player, which is out of the game
                    // we remove it from the list
                    found = true;
                    playerToRemove = player;
 
                    break;
                }
            }
            if(found)
            {
                activePlayers.Remove(playerToRemove);
            }
        }
    }

    public GridTile GetNextPestTileRandom()
    {
        // find all the "candidates" i.e. all the tiles non pested and adjacent to a pested tile
        List<GridTile> candidates = FindCandidateTiles();

//        foreach(var c in candidates)
//        {
//            Debug.Log("Candidate: " + c);
//        }
        // select a tile among the candidates
        int index = random.Next(0, candidates.Count);
        GridTile newPestTile = candidates[index];

        

        // return the tile
        return newPestTile;
    }

    public GridTile GetNextPestTileScripted()
    {
        if(pestSpreadingIndex >= pestProgression.years.Count)
        {
            Debug.LogError("The pest spreading index is higher than available scripted tiles");
            return null;
        }
        if(pestProgression.years[pestSpreadingIndex] == currentYear)
        {
            GridTile tile =  GetTileFromLocation(pestProgression.pattern[pestSpreadingIndex]);
            pestSpreadingIndex = pestSpreadingIndex + 1;
            Debug.Log("The pest has spread to " + tile.coordinates);
            return tile;
        }
        return null;
    }

    public GridTile GetNextPestTileSemiScripted()
    {
        if(pestSpreadingIndex < pestProgression.pattern.Count)
        {
            GridTile tile = GetTileFromLocation(pestProgression.pattern[pestSpreadingIndex]);
            pestSpreadingIndex = pestSpreadingIndex + 1;
            Debug.Log("The pest has spread to " + tile.coordinates);
            return tile;
        }
        return null;
    }

    private List<GridTile> FindCandidateTiles()
    {
        List<GridTile> candidates = new List<GridTile>();
       foreach(var pestedTile in pestProgression.currentPestProgression)
       {
           foreach(var neighbor in Neighbors(pestedTile))
           {
               if(!candidates.Contains(neighbor) && !pestProgression.currentPestProgression.Contains(neighbor))
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
    
    public GridTile GetTileFromLocation(Location location)
    {
        foreach(GridTile t in tileList)
        {
            if(t.coordinates.x == location.x 
            && t.coordinates.y == location.y)
            {
                return t;
            }
        }
        return null;
    }

    public int CalculateDistanceToPest(Location location)
    {
        GridTile locationTile = GetTileFromCoordinates(new Vector3Int(location.x, location.y, 0));
        int minDistance = Math.Max(Math.Abs(maxX - minX), Math.Abs(maxY - minY)) + 1; // not sure if this is right
        foreach(var pestTile in pestProgression.currentPestProgression)
        {
            int distance = hexDistance(pestTile, locationTile);
            if(distance < minDistance)
            {
                minDistance = distance;
            }
        }

        return minDistance;

    }

    private int hexDistance(GridTile hex1, GridTile hex2)
    {
        if(hex1.coordinates.x == hex2.coordinates.x)
        {
            return Math.Abs(hex2.coordinates.y - hex1.coordinates.y);
        }
        else if (hex1.coordinates.y == hex2.coordinates.y)
        {
            return Math.Abs(hex2.coordinates.x - hex1.coordinates.x);
        }
        else
        {
            int dx = Math.Abs(hex2.coordinates.x - hex1.coordinates.x);
            int dy = Math.Abs(hex2.coordinates.y - hex1.coordinates.y);
            if(hex1.coordinates.y < hex2.coordinates.y)
            {
                int distance = dx + dy - (int)Math.Ceiling(dx / 2.0);
                //Debug.Log("Player " + id + ", D-" + hex1 + "-" + hex2 + ": " + distance);
                return distance;
            }
            else
            {
                int distance = dx + dy - (int)Math.Floor(dx / 2.0);
                //Debug.Log("Player " + id + ", D-" + hex1 + "-" + hex2 + ": " + distance);
                return distance;
            }

        }
    }
}


