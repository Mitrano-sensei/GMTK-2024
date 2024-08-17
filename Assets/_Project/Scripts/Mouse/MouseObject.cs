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
    private int _previousHitAmount = 2;
    
    public int HitsLeft => _hitNumber;
    public List<Vector3Int> DestroyedTrees => _destroyedTrees;
    
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
            if (hitable && CanHit()) Hit();
            else Debug.Log("Nope!");
        }
    }

    private bool ManageSelection()
    {
        var selectionTileMap = GameReferences.Instance.SelectionTileMap;
        var islandTileMap = GameReferences.Instance.IslandTileMap;
        var treeTileMap = GameReferences.Instance.TreeTileMap;
        
        var cellPosition = GridHelpers.GetCellFromMousePosition(islandTileMap.layoutGrid);
        
        var isTileValid = islandTileMap.HasTile(cellPosition);
        var isTreeHere = treeTileMap.HasTile(cellPosition);
        
        selectionTileMap.ClearAllTiles();
        
        if (isTileValid) selectionTileMap.SetTile(cellPosition, GameReferences.Instance.SelectionTile);
        selectionTileMap.SetTileFlags(cellPosition, TileFlags.None);
        if (isTreeHere)
        {
            Debug.Log("Tree here!");
            selectionTileMap.SetColor(cellPosition, Color.black);
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
    
    private void Hit()
    {
        if (_isHitting) return;
        _isHitting = true;
        
        var t = axeSprite.transform;
        var originalScale = t.localScale;
        var originalRotation = t.localRotation;

        var sequence = DOTween.Sequence();
        sequence.Append(t.DOLocalRotate(originalRotation.eulerAngles + axeRotation, hitDuration/2f).SetEase(Ease.InBounce))
            .Join(t.DOScaleY(originalScale.y * hitScaleFactor, hitDuration/2f).SetEase(Ease.InBounce))
            .AppendCallback(RemoveTree)
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

    private void RemoveTree()
    {
        var treeTileMap = GameReferences.Instance.TreeTileMap;
        var cellPosition = GridHelpers.GetCellFromMousePosition(treeTileMap.layoutGrid);
        var t = treeTileMap.GetTile(cellPosition);

        Matrix4x4 matrix = Matrix4x4.Scale(Vector3.one);

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
    
    public void ResetDestroyedTrees()
    {
        _destroyedTrees.Clear();
        SetHits(_previousHitAmount);
    }
}