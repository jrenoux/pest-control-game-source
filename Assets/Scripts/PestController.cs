using Newtonsoft.Json;
using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine;
public class PestController
{
    public enum PestControllerTypes {
        Scripted,
        Random,
    }
    
    private PestControllerTypes type;
    private System.Random  random;
    private (int x, int y)[] patternPestProgression;

    private List<(int x, int y)> pestProgression;
    private (int x, int y) newPestTile;

    private int minX = -2;
    private int maxX = 1;
    private int minY = -5;
    private int maxY = -1;


    public PestController(System.Random random, (int x, int y) initialPestTile)
    {
        pestProgression = new List<(int x, int y)>();
        this.type = PestControllerTypes.Random;
        this.random = random;
        pestProgression.Add(initialPestTile);
    }

    public PestController(string patternFile)
    {
        this.type = PestControllerTypes.Scripted;
        pestProgression = new List<(int x, int y)>();

           // load the json 
        string contents = File.ReadAllText(@patternFile);
        patternPestProgression = JsonConvert.DeserializeObject<(int, int)[]>(contents); 

        pestProgression.Add(patternPestProgression[0]);
    }

    public (int x, int y) GetNextPestTile() 
    {
        switch(type) 
        {
            case PestControllerTypes.Random:
            return  GetNextPestTileRandom();

            case PestControllerTypes.Scripted:
            return GetNextPestTileScripted();
        }
        return (-6, -6); // weird that this is needed
    }

    private (int x, int y) GetNextPestTileRandom()
    {
        // find all the "candidates" i.e. all the tiles non pested and adjacent to a pested tile
        List<(int x, int y)> candidates = FindCandidateTiles();

        // select a tile among the candidates
        int index = random.Next(0, candidates.Count);
        newPestTile = candidates[index];

        // add the tile to the pestStatus
        pestProgression.Add(newPestTile);

        // return the tile
        return newPestTile;
    }

    private (int x, int y) GetNextPestTileScripted()
    {
        return (0, 0);
    }

    private List<(int x, int y)> FindCandidateTiles()
    {
        List<(int x, int y)> candidates = new List<(int x, int y)>();
        // for each pested tile
        foreach((int x, int y) pestedTile in this.pestProgression)
        {
            Debug.Log("Tile: (" + pestedTile.x + "," + pestedTile.y + ")");
            // if tile is (x, y), neighbors are: 
            // 1. (x+1, y)
            (int x, int y) neighbor1 = (pestedTile.x + 1, pestedTile.y);
            if(IsInGrid(neighbor1) && !candidates.Contains(neighbor1) && !this.pestProgression.Contains(neighbor1))
            {
                candidates.Add(neighbor1);
                Debug.Log("c1: (" + neighbor1.x + "," + neighbor1.y + ")");
            } 
            

            // 2. (x+1, y+1)
            (int x, int y) neighbor2 = (pestedTile.x + 1, pestedTile.y + 1);
            if(IsInGrid(neighbor2) && !candidates.Contains(neighbor2) && !this.pestProgression.Contains(neighbor2))
            {
                candidates.Add(neighbor2);
                Debug.Log("c2: (" + neighbor2.x + "," + neighbor2.y + ")");
            } 

            // 3. (x, y+1)
            (int x, int y) neighbor3 = (pestedTile.x, pestedTile.y + 1);
            if(IsInGrid(neighbor3) && !candidates.Contains(neighbor3) && !this.pestProgression.Contains(neighbor3))
            {
                candidates.Add(neighbor3);
                Debug.Log("c3: (" + neighbor3.x + "," + neighbor3.y + ")");
            }

            // 4. (x-1, y)
            (int x, int y) neighbor4 = (pestedTile.x - 1, pestedTile.y);
            if (IsInGrid(neighbor4) && !candidates.Contains(neighbor4) && !this.pestProgression.Contains(neighbor4))
            {
                candidates.Add(neighbor4);
                Debug.Log("c4: (" + neighbor4.x + "," + neighbor4.y + ")");
            }

            // 5. (x, y-1)
            (int x, int y) neighbor5 = (pestedTile.x, pestedTile.y - 1);
            if(IsInGrid(neighbor5) && !candidates.Contains(neighbor5) && !this.pestProgression.Contains(neighbor5))
            {
                candidates.Add(neighbor5);
                Debug.Log("c5: (" + neighbor5.x + "," + neighbor5.y + ")");
            }

            // 6. (x+1, y-1)
            (int x, int y) neighbor6 = (pestedTile.x + 1, pestedTile.y - 1);
            if(IsInGrid(neighbor6) && !candidates.Contains(neighbor6) && !this.pestProgression.Contains(neighbor6))
            {
                candidates.Add(neighbor6);
                Debug.Log("c6: (" + neighbor6.x + "," + neighbor6.y + ")");
            }

        }

        Debug.Log(candidates.Count + " Candidates.");
        foreach(var c in candidates)
        {
            Debug.Log("(" + c.x + "," + c.y + ")");
        }
        return candidates;
    }

    private bool IsInGrid((int x, int y) tile)
    {
        if(tile.x >= minX && tile.x <= maxX
        && tile.y >= minY && tile.y <= maxY)
        {
            return true;
        } 
        return false;
    } 

    public (int x, int y) GetNewPestTile() 
    {
        return newPestTile;
    }

    public (int x, int y)[] GetPestTiles()
    {
        return pestProgression.ToArray();
    }
}