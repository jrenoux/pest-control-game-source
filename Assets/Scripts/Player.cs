using System;
using UnityEngine;
public class Player 
{
    public string type {get; set;}

    public string id {get; set;}

    public Location farmLocation {get; set;}

    private int contribution {get; set;}

    private GaussianRandom gaussianRandom = new GaussianRandom(RandomSingleton.GetInstance());

    public int wallet {get;set;}

    public World theWorld {get; set;}

    public int revenuePerYear {get; set;}


    public void CollectRevenue()
    {
        this.wallet = this.wallet + this.revenuePerYear;
    }


    public void CalculateContribution()
    {
        switch(type)
        {
            case "prosocial":
            SetContribution(CalculateProsocialContribution());
            break;

            case "egoistic":
            SetContribution(CalculateEgoisticContribution());
            break;

            case "human":
            // nothing to do, this is done when "Pay" is clicked
            break;

            default: 
            Debug.LogError("Player type " + type + " unknown. Known types are (prosocial, egoistic, human)");
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
        int c_max = this.wallet - revenuePerYear;
        int d_max = 5;

        // calculate distance to pest
        int distance = theWorld.CalculateDistanceToPest(farmLocation);
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
        this.wallet = this.wallet - contribution;        
    }

    public int GetContribution()
    {
        return this.contribution;
    }
}