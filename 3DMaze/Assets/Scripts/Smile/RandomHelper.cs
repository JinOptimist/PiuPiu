using System;
using System.Collections.Generic;

namespace Assets.Scripts.Smile
{
    public static class RandomHelper
    {
        public static Random Random;

        public static T GetRandom<T> (this List<T> list)
        {
            var index = Random.Next(list.Count);
            return list[index];
        }
    }
}
