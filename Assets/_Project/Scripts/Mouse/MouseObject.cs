using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utilities;

public class MouseObject : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer axeSprite;

    [SerializeField] private TextMeshPro number;

    [Header("Misc")]
    [SerializeField] private Vector3 axeRotation = new (0, 0, 75);
    [SerializeField] private float hitDuration = .4f;
    [SerializeField] private float hitScaleFactor = .8f;

    [Header("Debug")] 
    [SerializeField] private bool hideCursor = true;
    
    private int _hitNumber = 2;
    private bool _isHitting;

    private List<Vector3Int> _destroyedTrees = new();
    private List<Vector3Int> _hitRocks = new();
    private List<Vector3Int> _destroyedRocks = new();
    private int _previousHitAmount = 2;
    
    public int HitsLeft => _hitNumber;
    public List<Vector3Int> DestroyedTrees => _destroyedTrees;
    public List<Vector3Int> HitRocks => _hitRocks;
    public List<Vector3Int> DestroyedRocks => _destroyedRocks;
    
    public void Start()
    {
        if (hideCursor) Cursor.visible = false;
    }

    public void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mousePosition.x, mousePosition.y, 0f);

        var hitable = ManageSelection();
        
        if (Input.GetMouseButtonDown(0))
        {
            if (hitable && CanHit()) Hit(Input.mousePosition);
            else Debug.Log("Nope!");
        }

        if (Input.GetMouseButtonDown(1))
        {
            Debug.LogError(GameReferences.Instance.TreeTileMap.HasTile(GridHelpers.MousePositionToCell(GameReferences.Instance.TreeTileMap.layoutGrid)) ? "YA UNE TILE" : "YAPU");
        }
    }

    private bool ManageSelection()
    {
        var selectionTileMap = GameReferences.Instance.SelectionTileMap;
        var islandTileMap = GameReferences.Instance.IslandTileMap;
        var treeTileMap = GameReferences.Instance.TreeTileMap;
        
        var cellPosition = GridHelpers.MousePositionToCell(islandTileMap.layoutGrid);
        
        var isTileValid = islandTileMap.HasTile(cellPosition);
        var isTreeHere = treeTileMap.HasTile(cellPosition);
        
        selectionTileMap.ClearAllTiles();
        
        if (isTileValid) selectionTileMap.SetTile(cellPosition, GameReferences.Instance.SelectionTile);
        selectionTileMap.SetTileFlags(cellPosition, TileFlags.None);
        if (isTreeHere)
        {
            selectionTileMap.SetColor(cellPosition, Color.green*.7f);
        }
        else
        {
            selectionTileMap.SetColor(cellPosition, Color.white);
        }
        
        return isTileValid && isTreeHere;
    }

    private bool CanHit()
    {
        return HitsLeft > 0;
    }

    public void SetHits(int hitNumber)
    {
        _hitNumber = hitNumber;
        _previousHitAmount = hitNumber;
        number.text = _hitNumber.ToString();
    }
    
    private void Hit(Vector3 pos)
    {
        if (_isHitting) return;
        _isHitting = true;
        
        var t = axeSprite.transform;
        var originalScale = t.localScale;
        var originalRotation = t.localRotation;

        var sequence = DOTween.Sequence();
        sequence.Append(t.DOLocalRotate(originalRotation.eulerAngles + axeRotation, hitDuration/2f).SetEase(Ease.InBounce))
            .Join(t.DOScaleY(originalScale.y * hitScaleFactor, hitDuration/2f).SetEase(Ease.InBounce))
            .AppendCallback(() => AttackTree(pos))
            .Append(t.DOScaleY(originalScale.y, hitDuration/2f).SetEase(Ease.InBounce))
            .Join(t.DOLocalRotate(originalRotation.eulerAngles, hitDuration/2f).SetEase(Ease.InBounce))
            .AppendCallback(() =>
                {
                    _hitNumber--;
                    number.text = _hitNumber.ToString();
                    _isHitting = false;
                });

        sequence.Play();
    }

    private void AttackTree(Vector3 pos)
    {
        var treeTileMap = GameReferences.Instance.TreeTileMap;
        var worldPos = Camera.main.ScreenToWorldPoint(pos);
        var cellPosition = GridHelpers.WorldToCell(worldPos, treeTileMap.layoutGrid);

        var rock = treeTileMap.GetInstantiatedObject(cellPosition);
        
        if (rock != null)
        {
            // It's a rock ! Manage the rock
            var rockDestroyed = rock.GetComponent<RockTile>().Hit();
            _hitRocks.Add(cellPosition);
            
            if (rockDestroyed)
            {
                rock.GetComponent<RockTile>().DestroyRock(() => treeTileMap.SetTile(cellPosition, null));
                _destroyedRocks.Add(cellPosition);
                _hitRocks.Remove(cellPosition);
            }
            
            return;
        }
        
        // It's a tree ! Manage the tree
        Matrix4x4 matrix;
        float scale = 1f;
        
        DOTween.To(() => scale, f =>
            {
                scale = f;
                matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1));
                treeTileMap.SetTransformMatrix(cellPosition, matrix);
            }, 0f, hitDuration/2)
            .SetEase(Ease.InBounce)
            .OnComplete(
                () =>
                {
                    treeTileMap.SetTile(cellPosition, null);
                    treeTileMap.SetTransformMatrix(cellPosition, Matrix4x4.identity);
                });
        
        _destroyedTrees.Add(cellPosition);
    }
    
    public void ResetDestroyedTreesAndHits()
    {
        _destroyedTrees.Clear();
        _destroyedRocks.Clear();
        _hitRocks.Clear();
        SetHits(_previousHitAmount);
    }

}