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

    private (int x, int y) farmLocation;

    private int fund = 5;

    private int contribution;

    private FundManager fundManager;

    private GameControllerScript gameManager;
    private GaussianRandom gaussianRandom;

    public Player(int id, PlayerType t, (int , int) location, FundManager manager, GameControllerScript gameController, System.Random random)
    {
        this.id = id;
        type = t;
        farmLocation = location;
        this.fundManager = manager;
        this.gameManager = gameController;
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

    public (int x, int y) GetFarmLocation()
    {
        return farmLocation;
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
        Debug.Log("Gaussian : " + gaussian);

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
        (int x, int y)[] pestTiles = gameManager.GetPestTiles();
        int minDistance = 6;
        
        for(int i = 0 ; i < pestTiles.Length ; i++)
        {
            int distance = hexDistance(pestTiles[i], this.farmLocation);
            if(distance < minDistance)
            {
                minDistance = distance;
            }
        }

        return minDistance;
    }

    private int hexDistance((int x, int y) hex1, (int x, int y) hex2)
    {
        if(hex1.x == hex2.x)
        {
            return Math.Abs(hex2.y - hex1.y);
        }
        else if (hex1.y == hex2.y)
        {
            return Math.Abs(hex2.x - hex1.x);
        }
        else
        {
            int dx = Math.Abs(hex2.x - hex1.x);
            int dy = Math.Abs(hex2.y - hex1.y);
            if(hex1.y < hex2.y)
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