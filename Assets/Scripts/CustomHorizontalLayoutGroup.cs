using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomHorizontalLayoutGroup : MonoBehaviour, ILayoutGroup
{
    [SerializeField] private float _spacing; 
    [SerializeField] private RectOffset _padding;
    [SerializeField] private bool _setChildPreferredSettings;
    private readonly List<RectTransform> _childrenRects = new List<RectTransform>();

    private void Awake()
    {
        _childrenRects.Clear();

        foreach (Transform child in transform)
        {
            if (child is RectTransform rect && child.gameObject.activeSelf)
            {
                var layoutElement = child.GetComponent<LayoutElement>();
                if (layoutElement != null && layoutElement.ignoreLayout)
                    continue;
                
                _childrenRects.Add(rect);
            }
        }
    }

    private void LateUpdate()
    {
        SetLayoutHorizontal();
        SetLayoutVertical();
        LayoutRebuilder.MarkLayoutForRebuild((RectTransform)transform);
    }

    public void SetLayoutHorizontal()
    {
        float posX = _padding.left;
        for (int i = 0; i < _childrenRects.Count; i++)
        {
            var childRect = _childrenRects[i];
            var childWidth = LayoutUtility.GetPreferredWidth(childRect);
            SetChild(childRect, RectTransform.Axis.Horizontal, posX, childWidth);
            posX += childWidth + _spacing;
        }
    }

    public void SetLayoutVertical()
    {
        for (int i = 0; i < _childrenRects.Count; i++)
        {
            var childRect = _childrenRects[i];
            var height = LayoutUtility.GetPreferredHeight(childRect);
            SetChild(childRect, RectTransform.Axis.Vertical, _padding.top, height);
        }
    }

    private void SetChild(RectTransform child, RectTransform.Axis axis, float pos, float size)
    {
        child.anchorMin = Vector2.up;
        child.anchorMax = Vector2.up;
        child.pivot = Vector2.up;
        if (_setChildPreferredSettings)
            child.SetSizeWithCurrentAnchors(axis, size);
        
        switch (axis)
        {
            case RectTransform.Axis.Horizontal:
                child.anchoredPosition = new Vector2(pos, child.anchoredPosition.y);
                break;
            case RectTransform.Axis.Vertical:
                child.anchoredPosition = new Vector2(child.anchoredPosition.x, -pos);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
        }
    }
}
