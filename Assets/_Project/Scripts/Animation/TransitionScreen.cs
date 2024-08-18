using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class TransitionScreen : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private SpriteRenderer blackScreen;


    private void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.I))
        {
            FadeOut();
        }
        
        if (Input.GetKeyDown(KeyCode.O))
        {
            FadeIn();
        }
        */
    }

    private void Start()
    {
        FadeIn();
    }

    public void FadeIn()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(blackScreen.DOFade(0, 1f))
            .Append(transform.DOMoveY(-5f, 2f).SetEase(Ease.InQuad))
            .AppendCallback(ShuffleTrees);
        sequence.Play();
    }
    
    public void FadeOut()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(transform.DOMoveY(0f, 2f).SetEase(Ease.OutQuad))
            .Append(blackScreen.DOFade(1, 1f))
            .AppendCallback(ShuffleTrees);
        sequence.Play();
    }

    private void ShuffleTrees()
    {
        GetComponentsInChildren<DancingTree>().ToList().ForEach(tree => tree.SetRandomSprite());
    }
}
