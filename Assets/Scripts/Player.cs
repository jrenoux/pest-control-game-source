public class Player 
{
    public enum PlayerType  {
                                PROSOCIAL,
                                EGOISTIC,
                                OPTIMAL,
                            }

    private PlayerType type;

    public Player(PlayerType t)
    {
        type = t;
        switch(type)
        {
            case PlayerType.PROSOCIAL:
            break;

            case PlayerType.EGOISTIC:
            break;

            case PlayerType.OPTIMAL:
            break;
        } 
    }
}