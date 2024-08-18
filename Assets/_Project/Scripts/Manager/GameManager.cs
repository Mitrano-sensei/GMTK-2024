using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utilities;

public class GameManager : Singleton<GameManager>
{
    [SerializeField][Range(1, 5)] private int numberOfHits = 2;
    [SerializeField] private TransitionScreen transitionScreen;
    
    private MouseObject Axe => GameReferences.Instance.MouseObject;
    
    #region Unity Methods

    private void Start()
    {
        Instantiate(transitionScreen);
        Axe.SetHits(numberOfHits);
        StartCoroutine(CoroutineHelpers.AfterDelay(3f, () => Axe.FreezeAxe = false));
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
                Axe.SetHits(numberOfHits);
                Axe.ResetDestroyedTreesAndHits();
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
                if (treeTileMap.HasTile(cellPosition) && null == treeTileMap.GetInstantiatedObject(cellPosition)?.GetComponent<RockTile>())
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
                if (islandTileMap.HasTile(neighbour) && !treeTileMap.HasTile(neighbour) && !(Axe.DestroyedTrees.Contains(neighbour) || Axe.DestroyedRocks.Contains(neighbour)))
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
        Matrix4x4 matrix = Matrix4x4.Scale(Vector3.zero);
        float scale = 0f;
        GameReferences.Instance.TreeTileMap.SetTransformMatrix(tree, matrix);
        GameReferences.Instance.TreeTileMap.SetTile(tree, GameReferences.Instance.TreeTile);
        GameReferences.Instance.TreeTileMap.SetTileFlags(tree, TileFlags.None);
        
        DOTween
            .To(() => scale, x => scale = x, 1f, 1f)
            .SetEase(Ease.InExpo)
            .OnUpdate(() =>
            {
                matrix = Matrix4x4.Scale(new Vector3(1, scale, 1));
                GameReferences.Instance.TreeTileMap.SetTransformMatrix(tree, matrix);
            })
            .Play();
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

        foreach (var rock in Axe.DestroyedRocks)
        {
            var rockTileMap = GameReferences.Instance.TreeTileMap;
            rockTileMap.SetTile(rock, GameReferences.Instance.RockTile);
            rockTileMap.SetTileFlags(rock, TileFlags.None);
            rockTileMap.GetInstantiatedObject(rock).GetComponent<RockTile>().Hit();
        }

        foreach (var rock in Axe.HitRocks)
        {
            var rockTileMap = GameReferences.Instance.TreeTileMap;
            rockTileMap.SetTile(rock, GameReferences.Instance.RockTile);
            rockTileMap.SetTileFlags(rock, TileFlags.None);
            rockTileMap.GetInstantiatedObject(rock).GetComponent<RockTile>().ResetHealth();
        }

        Axe.ResetDestroyedTreesAndHits();
    }
}
