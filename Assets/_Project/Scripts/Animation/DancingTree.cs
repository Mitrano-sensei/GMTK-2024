using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class DancingTree : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private Sprite[] sprites;


    void Awake(){
        if (spriteRenderer == null) Debug.LogError("SpriteRenderer is not set!");

        SetRandomSprite();
    }

    private void Start()
    {
        var sequence = DOTween.Sequence();
        var originalScale = transform.localScale;

        sequence
            .Append(transform.DOScaleX(1.1f * originalScale.x, .7f))
            .Join(transform.DOScaleY(0.9f * originalScale.y, .7f))
            .Append(transform.DOScaleY(1.1f * originalScale.y, .7f))
            .Join(transform.DOScaleX(0.9f * originalScale.x, .7f))
            .SetLoops(-1, LoopType.Yoyo)
            .Play();
    }
    
    public void SetRandomSprite()
    {
        var randomSprite = sprites[Random.Range(0, sprites.Length)];
        spriteRenderer.sprite = randomSprite;
    }
}
