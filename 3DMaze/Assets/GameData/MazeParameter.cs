using MazeGenerator.Models.GenerationModels;
using System.Numerics;

namespace Assets.GameData
{
    public static class MazeParameter
    {
        public static int Length = 3;
        public static int Width = 3;
        public static int Height = 3;

        public static GenerationWeights GenerationWeights = GenerationWeights.GenericBuilding();

        public static Vector3? ExitLocation = null;

        public static int? Seed = 100;
    }
}
