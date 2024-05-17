using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MovementController
{
    private Vector2Int actualPlayerPosition;
    private List<Vector2Int> positions = new List<Vector2Int>();

    private List<List<TerrainType>> map;

    private GameView view;

    public int Speed { private set; get; }

    public MovementController(GameView gameView, MapBuilder mapBuilder, int playerAmount)
    {
        view = gameView;

        map = mapBuilder.GenerateMap();
        view.InitializeMap(map);

        for (int i = 0; i < playerAmount; i++)
        {
            Vector2Int newPosition = mapBuilder.GetStartPosition();
            positions.Add(newPosition);
            map[newPosition.y][newPosition.x] = TerrainType.Character;
        }

        actualPlayerPosition = positions[0];
        gameView.InitializeCharacterPositions(positions);
    }

    public void MoveCharacterRight()
    {
        if (IsValidPosition(actualPlayerPosition.x + 1, actualPlayerPosition.y))
        {
            MoveCharacterToPosition(actualPlayerPosition.x + 1, actualPlayerPosition.y);
        }
    }

    public void MoveCharacterLeft()
    {
        if (IsValidPosition(actualPlayerPosition.x - 1, actualPlayerPosition.y))
        {
            MoveCharacterToPosition(actualPlayerPosition.x - 1, actualPlayerPosition.y);
        }
    }

    public void MoveCharacterUp()
    {
        if (IsValidPosition(actualPlayerPosition.x, actualPlayerPosition.y + 1))
        {
            MoveCharacterToPosition(actualPlayerPosition.x, actualPlayerPosition.y + 1);
        }
    }

    public void MoveCharacterDown()
    {
        if (IsValidPosition(actualPlayerPosition.x, actualPlayerPosition.y - 1))
        {
            MoveCharacterToPosition(actualPlayerPosition.x, actualPlayerPosition.y - 1);
        }
    }

    public void MoveEnemyRandomly()
    {
        int randomDirection = Random.Range(0, 4);

        switch (randomDirection)
        {
            case 0:
                MoveCharacterRight();
                break;
            case 1:
                MoveCharacterLeft();
                break;
            case 2:
                MoveCharacterUp();
                break;
            case 3:
                MoveCharacterDown();
                break;
        }
    }

    public void StoreCharacterPosition(int turn)
    {
        positions[turn-1] = actualPlayerPosition;
    }

    public void UpdateCharacterPosition(int turn)
    {
        actualPlayerPosition = positions[turn-1];
        Speed = 0;
    }

    public void RemovePositionAfterDeath(int index, int turn)
    {
        if (index == turn - 1)
        {
            positions[index] = actualPlayerPosition;
        }

        map[positions[index].y][positions[index].x] = TerrainType.Grass;
    }

    private void MoveCharacterToPosition(int newX, int newY)
    {
        if (newX == actualPlayerPosition.x && newY == actualPlayerPosition.y)
        {
            return;
        }

        map[actualPlayerPosition.y][actualPlayerPosition.x] = TerrainType.Grass;
        map[newY][newX] = TerrainType.Character;

        actualPlayerPosition = new Vector2Int(newX, newY);

        view.MovePlayerToCell(actualPlayerPosition.x, actualPlayerPosition.y);
        Speed++;
    }

    private bool IsValidPosition(int x, int y)
    {
        return PositionExistsInMap(x, y) && ThereIsNoCharacterInPosition(x, y);
    }

    private bool ThereIsNoCharacterInPosition(int x, int y)
    {
        return map[y][x] == TerrainType.Grass;
    }

    private bool PositionExistsInMap(int x, int y)
    {
        return y >= 0 &&
               y < map.Count &&
               x >= 0 &&
               x < map[y].Count;
    }
}
