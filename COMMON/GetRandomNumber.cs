using System;

namespace COMMON
{
    public static class GetRandomNumber
    {
        public static int GenerateRandomNumber(int min, int max)
        {
            Random rnd = new Random();
            return rnd.Next(min, max);
        }


    }
}
