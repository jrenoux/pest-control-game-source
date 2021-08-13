using System.Collections.Generic;
using UnityEngine.Tilemaps;
using Newtonsoft.Json;
public class PestProgression
{
    public Location initialPestLocation {get; set;}
    public List<Location> pattern {get; set;}

    public List<int> years;

    public string type {get; set;}

    [JsonIgnore]
    public List<GridTile> currentPestProgression {get;}

    [JsonIgnore]
    public GridTile latestPestSpread {get; set;}
    
    [JsonIgnore]
    public bool latestPestControlSuccess {get; set;}

    public PestProgression() 
    {
        currentPestProgression = new List<GridTile>();
        latestPestSpread = null;
    }



}