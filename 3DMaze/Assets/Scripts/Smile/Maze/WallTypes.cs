using System;

namespace Assets.Scripts.Smile.Maze
{
    [Flags]
    public enum WallTypes
    {
        None = 0,
        North = 1,
        South = 2,
        West = 4,
        East = 8,

        StairToNorth = 16,
        StairToSouth = 32,
        StairToWest = 64,
        StairToEast = 128,
    }
}
