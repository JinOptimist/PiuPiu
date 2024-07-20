using MazeGenerator;
using MazeGenerator.Models.MazeModels;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WallType = MazeGenerator.Models.MazeModels.Wall;

public class MazeBuilder : MonoBehaviour
{
    public GameObject Wall;
    public GameObject Stair;

    private const int WALL_SIZE = 4;
    private const int HALF_WALL_SIZE = WALL_SIZE / 2;

    // will set on Start
    private int zMargin = -1 * WALL_SIZE;

    // Start is called before the first frame update
    void Start()
    {
        var generator = new Generator();
        var maze = generator.Generate(5, 4, 3,
            startPoint: new System.Numerics.Vector3(0, 0, 0),
            seed: 42);
        zMargin = -1 * (maze.Width + 1) * WALL_SIZE;
        BuildMaze(maze);
    }

    private void BuildMaze(Maze maze)
    {
        for (int z = 0; z < maze.Height; z++)
        {
            for (int y = 0; y < maze.Width; y++)
            {
                for (int x = 0; x < maze.Length; x++)
                {
                    BuildRoom(maze[x, y, z]);
                }
            }
        }

        // Build only first floor
        //for (int y = 0; y < maze.Width; y++)
        //{
        //    for (int x = 0; x < maze.Length; x++)
        //    {
        //        BuildRoom(maze[x, y, 0]);
        //    }
        //}
    }

    private void BuildRoom(Cell cell)
    {
        if (cell.InnerPart != InnerPart.None)
        {
            BuildStair(cell.X, cell.Y, cell.Z, cell.InnerPart);
        }

        // TODO Create an Enter to the maze
        // For now just a HACK IT ^_^
        if (cell.X == 0 && cell.Y == 0 && cell.Z == 0)
        {
            return;
        }

        var wall = cell.Wall;
        if (wall.HasFlag(WallType.North))
        {
            BuildWallNorthSouth(cell.X, cell.Y + 1, cell.Z);
        }
        if (wall.HasFlag(WallType.East))
        {
            BuildWallEastWest(cell.X + 1, cell.Y, cell.Z);
        }
        if (wall.HasFlag(WallType.South))
        {
            BuildWallNorthSouth(cell.X, cell.Y, cell.Z);
        }
        if (wall.HasFlag(WallType.West))
        {
            BuildWallEastWest(cell.X, cell.Y, cell.Z);
        }
        if (wall.HasFlag(WallType.Roof))
        {
            BuildRoof(cell.X, cell.Y, cell.Z);
        }



    }

    private void BuildWallEastWest(int x, int y, int z)
    {
        var wall = Instantiate(Wall);
        wall.transform.position = new Vector3(
            x * WALL_SIZE,
            z * WALL_SIZE + WALL_SIZE / 2,
            y * WALL_SIZE + zMargin);
    }

    private void BuildWallNorthSouth(int x, int y, int z)
    {
        var wall = Instantiate(Wall);
        wall.transform.Rotate(0, 90, 0);

        wall.transform.position = new Vector3(
            x * WALL_SIZE + HALF_WALL_SIZE,
            z * WALL_SIZE + WALL_SIZE / 2,
            (y - 1) * WALL_SIZE + HALF_WALL_SIZE + zMargin);
    }

    private void BuildRoof(int x, int y, int z)
    {
        var wall = Instantiate(Wall);
        wall.transform.Rotate(0, 0, 90);

        var half = WALL_SIZE / 2;
        wall.transform.position = new Vector3(
            x * WALL_SIZE + half,
            z * WALL_SIZE + WALL_SIZE,
            y * WALL_SIZE + zMargin);
    }

    private void BuildStair(int x, int y, int z, InnerPart stairType)
    {
        var stair = Instantiate(Stair);

        switch (stairType)
        {
            case InnerPart.StairFromSouthToNorth:
                stair.transform.Rotate(0, 0, 0);
                break;
            case InnerPart.StairFromNorthToSouth:
                stair.transform.Rotate(0, 180, 0);
                break;
            case InnerPart.StairFromWestToEast:
                stair.transform.Rotate(0, 90, 0);
                break;
            case InnerPart.StairFromEastToWest:
                stair.transform.Rotate(0, -90, 0);
                break;
        }

        stair.transform.position = new Vector3(
            x * WALL_SIZE + (WALL_SIZE / 2),
            z * WALL_SIZE + WALL_SIZE / 2,
            y * WALL_SIZE + zMargin);
    }
}
