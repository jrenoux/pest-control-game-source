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

    public Player(int id, PlayerType t, (int , int) location, FundManager manager = null)
    {
        this.id = id;
        type = t;
        farmLocation = location;

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
        // TODO 
        return 0;
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
}