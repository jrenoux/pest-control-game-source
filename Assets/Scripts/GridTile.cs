using UnityEngine;
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