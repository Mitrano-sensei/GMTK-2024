using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utilities;

public class GameReferences : Singleton<GameReferences>
{
    [Header("TileMaps")]
    [SerializeField] private Tilemap islandTileMap;
    [SerializeField] private Tilemap selectionTileMap;
    [SerializeField] private Tilemap treeTileMap;
    [SerializeField] private Tilemap flowerTileMap;
    
    [Header("Tiles")]
    [SerializeField] private TileBase selectionTile;
    [SerializeField] private TileBase treeTile;
    [SerializeField] private TileBase flowerTile;
    
    [Header("Misc")]
    [SerializeField] private MouseObject mouseObject;
    
    // References
    public Tilemap IslandTileMap => islandTileMap;
    public Tilemap SelectionTileMap => selectionTileMap;
    public Tilemap TreeTileMap => treeTileMap;
    public Tilemap FlowerTileMap => flowerTileMap;
    
    public TileBase SelectionTile => selectionTile;
    public TileBase TreeTile => treeTile;
    public TileBase FlowerTile => flowerTile;
    
    public MouseObject MouseObject => mouseObject;
    
    public void Start()
    {
        if (islandTileMap == null) Debug.LogError("Island TileMap is not set!");
        if (selectionTileMap == null) Debug.LogError("Selection TileMap is not set!");
        if (treeTileMap == null) Debug.LogError("Tree TileMap is not set!");
        if (flowerTileMap == null) Debug.LogError("Flower TileMap is not set!");
        
        if (selectionTile == null) Debug.LogError("Selection Tile is not set!");
        if (treeTile == null) Debug.LogError("Tree Tile is not set!");
        if (flowerTile == null) Debug.LogError("Flower Tile is not set!");
        
        if (mouseObject == null) Debug.LogError("Mouse Object is not set!");
    }
}
