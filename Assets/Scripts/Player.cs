using System;
using UnityEngine;
public class Player 
{
    public enum PlayerType  {
                                PROSOCIAL,
                                EGOISTIC,
                                HUMAN,
                            }

    private PlayerType type;

    private int id;

    private GridTile farmLocation;

    private int fund = 5;

    private int contribution;

    private FundManager fundManager;

    private World theWorld;
    private GaussianRandom gaussianRandom;

    public Player(int id, PlayerType t, (int x, int y) location, FundManager manager, World world, System.Random random)
    {
        this.id = id;
        type = t;

        this.fundManager = manager;
        this.theWorld = world;
        farmLocation = theWorld.GetTileFromCoordinates(new Vector3Int(location.x, location.y, 0));
        this.gaussianRandom = new GaussianRandom(random);

        switch(type)
        {
            case PlayerType.PROSOCIAL:
            break;

            case PlayerType.EGOISTIC:
            break;

            case PlayerType.HUMAN:
            break;
        } 
    }

    public Vector3Int GetFarmLocation()
    {
        return farmLocation.coordinates;
    }

    public void CollectRevenue(int revenue)
    {
        this.fund = this.fund + revenue;
    }

    public int GetContribution()
    {
        return this.contribution;
    }

    public void CalculateContribution()
    {
        switch(type)
        {
            case PlayerType.PROSOCIAL:
            SetContribution(CalculateProsocialContribution());
            break;

            case PlayerType.EGOISTIC:
            SetContribution(CalculateEgoisticContribution());
            break;

            case PlayerType.HUMAN:
            // nothing to do, this is done when "Pay" is clicked
            break;
        }
    }
    
    private int CalculateProsocialContribution()
    {
        // TODO 
        return 1;
    }
    
    private int CalculateEgoisticContribution()
    {  
        int c_max = this.fund - fundManager.getRevenuePerYear();
        int d_max = 5;

        // calculate distance to pest
        int distance = CalculateDistanceToPest();
        //Debug.Log("Player " + id + ": Pest is " + distance + " tiles away");

        double mean = (- c_max / d_max ) * distance + c_max;
        double deviation = 1;

        double gaussian = gaussianRandom.NextGaussian(mean, deviation);
        //Debug.Log("Gaussian : " + gaussian);

        int contribution = (int)Math.Round(gaussian);
         
        return contribution;
    }

    public void SetContribution(int contribution)
    {
        this.contribution = contribution;
        this.fund = this.fund - contribution;        
    }

    public int GetFund()
    {
        return this.fund;
    }

    public int GetId()
    {
        return this.id;
    }

    private int CalculateDistanceToPest()
    {
        // for each pest tile, we calculate the distance and keep the shortest
        int minDistance = 6;
        foreach(var pestTile in theWorld.currentPestProgression)
        {
            Debug.Log("Calculate distance to pest: " + pestTile + " - " + this.farmLocation + "(player id: " + id + ")");
            int distance = hexDistance(pestTile, this.farmLocation);
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

    public bool IsHuman()
    {
        if(this.type == PlayerType.HUMAN)
        {
            return true;
        }
        return false;
    }
}