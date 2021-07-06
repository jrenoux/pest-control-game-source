using System;
using System.Collections;
public class RandomSingleton
{
    private static Random random = null;

    private RandomSingleton()
    {
        
    }

    public static Random GetInstance()
    {
        if(random == null)
        {
            random = new Random();
        }
        return random;
    }

    public static Random GetInstance(int seed)
    {
        if(random == null)
        {
            random = new Random(seed);
        }
        return random;
    }
}