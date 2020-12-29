using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSwapper : MonoBehaviour
{
    public Color newColor;
    private Color _originalColor;
    private MeshRenderer _renderer;
    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        _originalColor = _renderer.material.color;
    }


    public void ChangeColor()
    {
        _renderer.material.color = newColor;
    }

    public void ResetColor()
    {
        _renderer.material.color = _originalColor;
    }
    
    
}
