using System.Collections.Generic;
using UnityEngine.Tilemaps;
public class PestProgression
{
    public Location initialPestLocation {get; set;}
    public List<Location> pattern {get; set;}

    public List<int> years;

    public string type {get; set;}

    public List<GridTile> currentPestProgression {get;}

    public GridTile latestPestSpread {get; set;}
    public bool latestPestControlSuccess {get; set;}

    public PestProgression() 
    {
        currentPestProgression = new List<GridTile>();
        latestPestSpread = null;
    }



}