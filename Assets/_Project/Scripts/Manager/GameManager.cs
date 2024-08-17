using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utilities;

public class GameManager : Singleton<GameManager>
{
    private MouseObject Axe => GameReferences.Instance.MouseObject;
    
    #region Unity Methods
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Axe.HitsLeft == 0)
            {
                GrowTrees();
                Axe.SetHits(2);
                Axe.ResetDestroyedTrees();
            }
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            Undo();
        }
    }

    #endregion
    
    private void GrowTrees()
    {
        var treeTileMap = GameReferences.Instance.TreeTileMap;
        var islandTileMap = GameReferences.Instance.IslandTileMap;
        
        List<Vector3Int> currentTrees = new();
        
        var bounds = islandTileMap.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                var cellPosition = new Vector3Int(x, y, 0);
                if (treeTileMap.HasTile(cellPosition))
                {
                    currentTrees.Add(cellPosition);
                }
            }
        }

        List<Vector3Int> newTreesPositions = new();
        foreach (var tree in currentTrees)
        {
            var neighbours = GetNeighbours(tree);
            foreach (var neighbour in neighbours)
            {
                if (islandTileMap.HasTile(neighbour) && !treeTileMap.HasTile(neighbour) && !Axe.DestroyedTrees.Contains(neighbour))
                {
                    newTreesPositions.Add(neighbour);
                }
            }
        }
        
        foreach (var tree in newTreesPositions)
        {
            GrowTree(tree);
        }

    }

    private void GrowTree(Vector3Int tree)
    {
        // TODO --> Add animation
        GameReferences.Instance.TreeTileMap.SetTile(tree, GameReferences.Instance.TreeTile);
        
        // Remove Flags
        GameReferences.Instance.TreeTileMap.SetTileFlags(tree, TileFlags.None);
    }

    private List<Vector3Int> GetNeighbours(Vector3Int tree)
    {
        List<Vector3Int> neighbours = new()
        {
            tree.AddX(1),
            tree.AddX(-1),
            tree.AddY(1),
            tree.AddY(-1)
        };

        return neighbours;
    }

    private bool ContainsTree(Vector3Int tile)
    {
        return GameReferences.Instance.TreeTileMap.HasTile(tile);
    }
    
    private void Undo()
    {
        foreach (var tree in Axe.DestroyedTrees)
        {
            GrowTree(tree);
        }

        Axe.ResetDestroyedTrees();
    }
}
