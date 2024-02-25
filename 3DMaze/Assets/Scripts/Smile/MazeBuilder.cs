using Assets.Scripts.Smile.Maze;
using UnityEngine;

public class MazeBuilder : MonoBehaviour
{
    public GameObject Wall;
    public GameObject Stair;

    public MazeGenerator MazeGenerator;

    private const int WALL_SIZE = 4;

    // will set on Start
    private int zMargin = -1 * WALL_SIZE;

    // Start is called before the first frame update
    void Start()
    {
        var maze = MazeGenerator.GenerateMaze();
        zMargin = -1 * (maze.Length + 1) * WALL_SIZE;
        BuildMaze(maze);
    }

    private void BuildMaze(MazeCube maze)
    {
        for (int z = 0; z < maze.Height; z++)
        {
            for (int y = 0; y < maze.Length; y++)
            {
                for (int x = 0; x < maze.Width; x++)
                {
                    BuildRoom(maze[x, y, z]);
                }
            }
        }
    }

    private void BuildRoom(Cell cell)
    {
        var wall = cell.Wall;
        if (wall.HasFlag(WallTypes.North))
        {
            BuildWallNorthSouth(cell.X, cell.Y + 1);
        }
        if (wall.HasFlag(WallTypes.East))
        {
            BuildWallEastWest(cell.X + 1, cell.Y);
        }
        if (wall.HasFlag(WallTypes.South))
        {
            BuildWallNorthSouth(cell.X, cell.Y);
        }
        if (wall.HasFlag(WallTypes.West))
        {
            BuildWallEastWest(cell.X, cell.Y);
        }

        //if (wall.HasFlag(WallTypes.StairToNorth))
        //{
        //    BuildStair(cell.X, cell.Y);
        //}

        //if (!wall.HasFlag(WallTypes.StairToNorth)
        //    && !wall.HasFlag(WallTypes.StairToEast)
        //    && !wall.HasFlag(WallTypes.StairToSouth)
        //    && !wall.HasFlag(WallTypes.StairToWest))
        //{
        //    BuildRoof(cell.X, cell.Y);
        //}
    }

    private void BuildWallEastWest(int x, int y)
    {
        var wall = Instantiate(Wall);
        wall.transform.position = new Vector3(
            x * WALL_SIZE,
            WALL_SIZE / 2,
            y * WALL_SIZE + zMargin);
    }

    private void BuildWallNorthSouth(int x, int y)
    {
        var wall = Instantiate(Wall);
        wall.transform.Rotate(0, 90, 0);

        var half = WALL_SIZE / 2;
        wall.transform.position = new Vector3(
            x * WALL_SIZE + half,
            half,
            (y - 1) * WALL_SIZE + half + zMargin);
    }

    private void BuildRoof(int x, int y)
    {
        var wall = Instantiate(Wall);
        wall.transform.Rotate(0, 0, 90);

        var half = WALL_SIZE / 2;
        wall.transform.position = new Vector3(
            x * WALL_SIZE + half,
            WALL_SIZE,
            y * WALL_SIZE + zMargin);
    }

    private void BuildStair(int x, int y)
    {
        var stair = Instantiate(Stair);
        stair.transform.position = new Vector3(
            x * WALL_SIZE + (WALL_SIZE / 2),
            WALL_SIZE / 2,
            y * WALL_SIZE + zMargin);
    }
}
