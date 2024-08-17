using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utilities;
using Random = System.Random;

public class FlowerManager : MonoBehaviour
{

    [Header("Misc")]
    [SerializeField] private float giggleAmplitude = .6f;
    [SerializeField] private float giggleSpeed = 1f;
    [SerializeField] private float flowerSpawnRate = .4f;

    private Tilemap _islandTileMap;
    private Tilemap _flowerTileMap;
    
    private TileBase _flowerTile;

    private void Start()
    {
        _islandTileMap = GameReferences.Instance.IslandTileMap;
        _flowerTileMap = GameReferences.Instance.FlowerTileMap;
        
        _flowerTile = GameReferences.Instance.FlowerTile;
        
        GetIslandTilesPositions().ForEach(GiggleFlower);
        GetIslandTilesPositions().ForEach(GiggleTrees);
        
        SpawnFlowers();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SpawnFlowers(); // TODO --> Remove
        }
    }
    
    public void SpawnFlowers()
    {
        ResetFlowers();
        
        List<Vector3Int> tilePositions = GetIslandTilesPositions();
        
        foreach (var pos in tilePositions)
        {
            if (UnityEngine.Random.value < flowerSpawnRate)
            {
                _flowerTileMap.SetTile(pos, _flowerTile);
                
                // Remove Flags
                _flowerTileMap.SetTileFlags(pos, TileFlags.None);
            }
        }
        
    }

    private void GiggleFlower(Vector3Int pos)
    {
        Matrix4x4 matrix4X4;
        float reverse = UnityEngine.Random.value < .5f ? -1f : 1f;
        float rValue = -30f * giggleAmplitude * reverse;

        var c = CoroutineHelpers.AfterDelay(UnityEngine.Random.value, () =>
        {
            DOTween
                .To(() => rValue, x => rValue = x, 30f * giggleAmplitude * reverse, 1 / giggleSpeed)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .OnUpdate(() =>
                    {
                        matrix4X4 = Matrix4x4.Rotate(Quaternion.Euler(0, 0, rValue));
                        _flowerTileMap.SetTransformMatrix(pos, matrix4X4);
                    }
                ).Play();
        });

        StartCoroutine(c);
    }

    private void GiggleTrees(Vector3Int pos)
    {
        Matrix4x4 matrix4X4;
        float reverse = UnityEngine.Random.value < .5f ? -1f : 1f;
        float rValue = -30f * giggleAmplitude * reverse;
        
        GameReferences.Instance.TreeTileMap.SetTileFlags(pos, TileFlags.None);
        
        var c = CoroutineHelpers.AfterDelay(UnityEngine.Random.value, () =>
        {
            DOTween
                .To(() => rValue, x => rValue = x, 30f * giggleAmplitude * reverse, 1 / giggleSpeed)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .OnUpdate(() =>
                    {
                        matrix4X4 = Matrix4x4.Rotate(Quaternion.Euler(0, 0, rValue));
                        GameReferences.Instance.TreeTileMap.SetTransformMatrix(pos, matrix4X4);
                    }
                ).Play();
        });

        StartCoroutine(c);
    }

    private List<Vector3Int> GetIslandTilesPositions()
    {
        List<Vector3Int> tilePositions = new List<Vector3Int>();
        
        BoundsInt bounds = _islandTileMap.cellBounds;
        
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                if (_islandTileMap.HasTile(cellPosition))
                {
                    tilePositions.Add(cellPosition);
                }
            }
        }

        return tilePositions;
        
    }

    private void ResetFlowers()
    {
        _flowerTileMap.ClearAllTiles();
    }
}
