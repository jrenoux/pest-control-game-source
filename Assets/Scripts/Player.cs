using System;
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

    private Random random;

    public Player(int id, PlayerType t, (int , int) location, FundManager manager)
    {
        this.id = id;
        type = t;
        farmLocation = location;
        this.fundManager = manager;
        this.random = new Random();

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

        double mean = (- c_max / d_max ) * distance + c_max;
        double deviation = 1;

        int contribution = (int)Math.Round(SampleGaussian(random, mean, deviation));
        // TODO 
        return contribution;
    }

    public double SampleGaussian(Random random, double mean, double stddev)
        {
            // The method requires sampling from a uniform random of (0,1]
            // but Random.NextDouble() returns a sample of [0,1).
            double x1 = 1 - random.NextDouble();
            double x2 = 1 - random.NextDouble();

            double y1 = Math.Sqrt(-2.0 * Math.Log(x1)) * Math.Cos(2.0 * Math.PI * x2);
            return y1 * stddev + mean;
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
        return 2;
    }
}