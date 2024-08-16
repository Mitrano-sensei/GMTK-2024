using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

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

    public int HitsLeft => _hitNumber;

    public void Start()
    {
        if (hideCursor) Cursor.visible = false;
    }

    public void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mousePosition.x, mousePosition.y, 0f);
        
        if (Input.GetMouseButtonDown(0))
        {
            if (CanHit()) Hit();
            else Debug.Log("No more hits left!");
        }
    }

    private bool CanHit()
    {
        if (HitsLeft > 0)
        {
            _hitNumber--;
            number.text = _hitNumber.ToString();
            return true;
        }

        return false;
    }

    public bool SetHits(int hitNumber)
    {
        if (_hitNumber != 0) return false;
        
        _hitNumber = hitNumber;
        number.text = _hitNumber.ToString();
        return true;
    }
    
    private void Hit()
    {
        if (_isHitting) return;
        _isHitting = true;
        
        var t = axeSprite.transform;
        var originalScale = t.localScale;
        var originalRotation = t.localRotation;

        var sequence = DOTween.Sequence();
        sequence.Append(t.DOLocalRotate(originalRotation.eulerAngles + axeRotation, hitDuration/2f).SetEase(Ease.InBounce));
        sequence.Join(t.DOScaleY(originalScale.y * hitScaleFactor, hitDuration/2f).SetEase(Ease.InBounce));
        sequence.Append(t.DOScaleY(originalScale.y, hitDuration/2f).SetEase(Ease.InBounce));
        sequence.Join(t.DOLocalRotate(originalRotation.eulerAngles, hitDuration/2f).SetEase(Ease.InBounce));
        sequence.AppendCallback(() => _isHitting = false);

        sequence.Play();
    }
    

}