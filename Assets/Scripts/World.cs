using System.Collections.Generic;

public class World 
{
    public int minX {get; set;}
    public int maxX {get; set;}
    public int minY {get; set;}
    public int maxY {get; set;}

    public string humanPlayer {get; set;}
    public Dictionary<string, Location> farmsLocations {get; set;}
    public List<Location> pestProgression {get; set;}

    public Location initialPestLocation {get; set;}

    
}

public class Location
{
    public int x {get; set;}
    public int y {get; set;}
}