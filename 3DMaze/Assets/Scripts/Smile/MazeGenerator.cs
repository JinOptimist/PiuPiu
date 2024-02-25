using Assets.Scripts.Smile;
using Assets.Scripts.Smile.Maze;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class MazeGenerator : MonoBehaviour
{
    public int Length;
    public int Width;
    public int Height;

    private List<CellForBuilding> _cellForBuildings;

    public MazeCube GenerateMaze()
    {
        int? seed = null;
        if (seed is null)
        {
            seed = DateTime.Now.Second;
        }
        var random = new Random(seed.Value);
        RandomHelper.Random = random;

        var mazeCube = GenerateRoomFullOfWalls();

        _cellForBuildings = mazeCube
            .Cells
            .Select(x => new CellForBuilding(x))
            .ToList();

        var miner = _cellForBuildings.First();
        miner.IsVisited = true;

        while (_cellForBuildings.Any(x => !x.IsVisited))
        {
            var canStepCells = GetNearNotVisted(miner);
            while (!canStepCells.Any())
            {
                miner.IsCompleted = true;
                miner = _cellForBuildings
                    .Where(x => !x.IsCompleted && x.IsVisited)
                    .ToList()
                    .GetRandom();
                canStepCells = GetNearNotVisted(miner);
            }

            var cellToStep = canStepCells.GetRandom();
            var direction = GetDirection(miner, cellToStep);
            BreakWall(miner, direction);
            cellToStep.IsVisited = true;
            miner = cellToStep;
        }

        var enter = _cellForBuildings
            .First(x => x.X == 0 && x.Y == mazeCube.Length - 1 && x.Z == 0);
        BreakWall(enter, WallTypes.North);

        _cellForBuildings.ForEach(cell =>
            mazeCube.ReplaceCell(cell.X, cell.Y, cell.Z, cell.Wall)
        );

        return mazeCube;
    }

    public void BreakWall(Cell cell, WallTypes wall)
    {
        var x = cell.X;
        var y = cell.Y;
        var z = cell.Z;
        cell.Wall = cell.Wall.Remove(wall);
        Cell oppositeCell;

        switch (wall)
        {
            case WallTypes.None:
                throw new ArgumentException("Why do you try remove None wall?");
            case WallTypes.North:
                oppositeCell = GetCell(x, y + 1, z);
                if (oppositeCell is not null)
                {
                    oppositeCell.Wall = oppositeCell.Wall.Remove(WallTypes.South);
                }
                break;
            case WallTypes.South:
                oppositeCell = GetCell(x, y - 1, z);
                if (oppositeCell is not null)
                {
                    oppositeCell.Wall = oppositeCell.Wall.Remove(WallTypes.North);
                }
                break;
            case WallTypes.West:
                oppositeCell = GetCell(x - 1, y, z);
                if (oppositeCell is not null)
                {
                    oppositeCell.Wall = oppositeCell.Wall.Remove(WallTypes.East);
                }
                break;
            case WallTypes.East:
                oppositeCell = GetCell(x + 1, y, z);
                if (oppositeCell is not null)
                {
                    oppositeCell.Wall = oppositeCell.Wall.Remove(WallTypes.West);
                }
                break;
            case WallTypes.StairToNorth:
            case WallTypes.StairToEast:
            case WallTypes.StairToSouth:
            case WallTypes.StairToWest:
                throw new NotImplementedException();
            default:
                break;
        }
    }

    private CellForBuilding GetCell(int x, int y, int z)
        => _cellForBuildings.FirstOrDefault(c => c.X == x && c.Y == y && c.Z == z);

    private WallTypes GetDirection(CellForBuilding miner, CellForBuilding cellToStep)
    {
        if (miner.X + 1 == cellToStep.X)
        {
            return WallTypes.East;
        }
        if (miner.X - 1 == cellToStep.X)
        {
            return WallTypes.West;
        }
        if (miner.Y + 1 == cellToStep.Y)
        {
            return WallTypes.North;
        }
        if (miner.Y - 1 == cellToStep.Y)
        {
            return WallTypes.South;
        }

        throw new Exception($"I can't undrestand where you try to step. " +
            $"Initial cell {miner}. " +
            $"Destination {cellToStep}");
    }

    private IEnumerable<CellForBuilding> GetNearCell(CellForBuilding currentCell)
    {
        return _cellForBuildings
            .Where(cell =>
                Math.Abs(cell.X - currentCell.X) == 1
                    && cell.Y == currentCell.Y
                    && cell.Z == currentCell.Z
                || cell.X == currentCell.X
                    && Math.Abs(cell.Y - currentCell.Y) == 1
                    && cell.Z == currentCell.Z
                || cell.X == currentCell.X
                    && cell.Y == currentCell.Y
                    && Math.Abs(cell.Z - currentCell.Z) == 1
            );
    }

    private List<CellForBuilding> GetNearNotVisted(CellForBuilding currentCell)
    {
        return GetNearCell(currentCell)
            .Where(x => !x.IsVisited)
            .ToList();
    }

    private MazeCube GenerateRoomFullOfWalls()
    {
        var maze = new MazeCube
        {
            Length = Length,
            Width = Width,
            Height = Height
        };

        var all = RoomWallAll();
        for (int z = 0; z < Height; z++)
        {
            for (int y = 0; y < Length; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    maze[x, y, z] = new Cell { X = x, Y = y, Z = z, Wall = all };
                }
            }
        }

        return maze;
    }

    private WallTypes RoomWallAll()
    {
        return Enum
            .GetValues(typeof(WallTypes))
            .Cast<WallTypes>()
            .Aggregate((x, y) => x | y);
    }
}
