using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class RockTile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    public int Health = 2;
    
    public bool Hit()
    {
        Health--;
        UpdateSprite();
        Tilt();
        return Health == 0;
    }

    private void UpdateSprite()
    {
        switch (Health)
        {
            case 2:
                spriteRenderer.sprite = GameReferences.Instance.RockSprite;
                break;
            case 1:
                spriteRenderer.sprite = GameReferences.Instance.DestroyedRockSprite;
                break;
            default:
                // spriteRenderer.sprite = null;
                break;
        }
    }

    public void DestroyRock(Action destroyCallback)
    {
        var sequence = DOTween.Sequence();
        sequence.Append(spriteRenderer.transform.DOScale(Vector3.zero, .2f).SetEase(Ease.InBounce));
        sequence.AppendCallback(() =>
        {
            spriteRenderer.sprite = null;
            destroyCallback();
        });
    }

    public void ResetHealth()
    {
        Health = 2;
        UpdateSprite();
    }

    private void Tilt()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(spriteRenderer.transform.DOLocalRotate(Vector3.forward * 10, .1f).SetEase(Ease.InBounce));
        sequence.Append(spriteRenderer.transform.DOLocalRotate(Vector3.forward * -10, .1f).SetEase(Ease.InBounce));
        sequence.Append(spriteRenderer.transform.DOLocalRotate(Vector3.zero, .1f).SetEase(Ease.InBounce));
        sequence.Play();
    }
    
}
