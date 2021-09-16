using System;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;

public class Player 
{
    // get/set are needed here because of the json instantiation
    // TODO to check if really needed
    public string type {get; set;}

    public string id {get; set;}

    public Location farmLocation {get; set;}
    public int revenuePerYear {get; set;}

    public int wallet {get;set;}

    public List<int> contributionsHistory;
    private int contribution {get; set;}

    private GaussianRandom gaussianRandom = new GaussianRandom(RandomSingleton.GetInstance());

    public void CollectRevenue()
    {
        this.wallet = this.wallet + this.revenuePerYear;
    }


    public int CalculateContribution()
    {
        int contribution = 0;
        switch(type)
        {
            case "prosocial":
            contribution = CalculateProsocialContribution();
            SetContribution(contribution);
            break;

            case "egoistic":
            contribution = CalculateEgoisticContribution();
            SetContribution(contribution);
            break;

            case "fixed": 
            contribution = this.revenuePerYear - 1;
            SetContribution(contribution); 
            break;

            case "human":
            // nothing to do, this is done when "Pay" is clicked
            break;

            default: 
            Debug.LogError("Player type " + type + " unknown. Known types are (prosocial, egoistic, fixed, human)");
            break;
        }
        return contribution;
    }
    
    private int CalculateProsocialContribution()
    {
        // TODO 
        int c_max = this.wallet - revenuePerYear;
        int d_max = 5;

        int minDistanceFromPlayerToPest = d_max;
        foreach(Player pl in PestApplication.Instance.theWorld.activePlayers)
        {
            int distance =  PestApplication.Instance.theWorld.CalculateDistanceToPest(pl.farmLocation);
            if(distance < minDistanceFromPlayerToPest)
            {
                minDistanceFromPlayerToPest = distance;
            }
        }

        double mean = (- c_max / d_max ) * minDistanceFromPlayerToPest + c_max;
        double deviation = 1;

        double gaussian = gaussianRandom.NextGaussian(mean, deviation);
        //Debug.Log("Gaussian : " + gaussian);

        int contribution = (int)Math.Round(gaussian);
         
        Debug.Log("Player " + id + " contributed " + contribution);
        return contribution;
    }
    
    private int CalculateEgoisticContribution()
    {  
        int c_max = this.wallet - revenuePerYear;
        int d_max = 5;

        // calculate distance to pest
        int distance = PestApplication.Instance.theWorld.CalculateDistanceToPest(farmLocation);
        //Debug.Log("Player " + id + ": Pest is " + distance + " tiles away");

        double mean = (- c_max / d_max ) * distance + c_max;
        double deviation = 1;

        double gaussian = gaussianRandom.NextGaussian(mean, deviation);
        //Debug.Log("Gaussian : " + gaussian);

        int contribution = (int)Math.Round(gaussian);
         
        Debug.Log("Player " + id + " contributed " + contribution);
        return contribution;
    }

    public void SetContribution(int contribution)
    {
        this.contribution = contribution;
        this.wallet = this.wallet - contribution;
        this.contributionsHistory.Add(contribution);
    }

    public int GetContribution()
    {
        return this.contribution;
    }
}