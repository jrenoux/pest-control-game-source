using UnityEngine;
using Newtonsoft.Json;
public class Location
{
    public int x {get; set;}
    public int y {get; set;}

    [JsonConstructor]
    public Location(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Location(Vector3Int coordinates)
    {
        this.x = coordinates.x;
        this.y = coordinates.y;
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
        
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        
        // TODO: write your implementation of Equals() here
        Location loc = obj as Location;
        if(loc.x == this.x && loc.y == this.y )
        {
            return true;
        }
        return false;
    }
        
    // override object.GetHashCode
    public override int GetHashCode()
    {
        return HashCode.Of(this.x).And(this.y);
    }

    public override string ToString()
    {
        return "(" + x + "," + y + ")";
    }


}