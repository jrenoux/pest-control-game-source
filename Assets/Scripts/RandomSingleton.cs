using System;
using System.Collections;
public class RandomSingleton
{
    private static Random random = null;

    private RandomSingleton()
    {
        
    }

    public static Random GetInstance(int? seed = null)
    {
        if(random == null)
        {
            if(seed == null)
            {
                random = new Random();
            }
            else 
            {
                random = new Random(seed.Value);
            }
        }
        return random;
    }
}