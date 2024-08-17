using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class FloatAnimation : MonoBehaviour
{
    [Header("Misc")] 
    [SerializeField] private float amplitude = .2f;

    [SerializeField] private float speed = 1f;
    
    void Update()
    {
        transform.position = transform.position.WithY(Mathf.Sin(Time.time * speed) * amplitude);
    }
}
