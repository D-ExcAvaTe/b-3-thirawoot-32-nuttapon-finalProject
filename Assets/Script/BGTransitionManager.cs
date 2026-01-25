using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BackgroundPatternManager : MonoBehaviour
{
    [Header("Configuration")]
    public string bgSortingLayerName = "BG";
    public float transitionDuration = 2f;

    // Sorting Order Logic:
    // Hidden: -999
    // Standard: 0 
    // Overlay: +10 
    private const int ORDER_OFFSET_HIDDEN = -999; 
    private const int ORDER_OFFSET_STANDARD = 0;  
    private const int ORDER_OFFSET_OVERLAY = 10;  

    [System.Serializable]
    public class BackgroundSet
    {
        [Header("Components")]
        public GameObject maskGO;
        public SpriteMask mask;

        [Header("Manual Assignment")]
        [Tooltip("Drag the specific Tilemaps here. The script will use the Sorting Order they have in the Inspector as their 'Base Order'.")]
        public List<TilemapRenderer> tilemaps;

        private struct TilemapInfo
        {
            public TilemapRenderer renderer;
            public int baseOrder;
        }
        private List<TilemapInfo> _cachedMaps = new List<TilemapInfo>();

        public void Init(string layerName)
        {
            _cachedMaps.Clear();
            int layerID = SortingLayer.NameToID(layerName);

            // 1. Force Mask to the correct Sorting Layer
            if (mask)
            {
                mask.frontSortingLayerID = layerID;
                mask.backSortingLayerID = layerID;
            }

            // 2. Cache Tilemap Orders
            foreach (var r in tilemaps)
            {
                if (r == null) continue;

                r.sortingLayerName = layerName;
                r.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                
                // Capture the order you set in Inspector (e.g., 0 or 1)
                _cachedMaps.Add(new TilemapInfo 
                { 
                    renderer = r, 
                    baseOrder = r.sortingOrder 
                });
            }
        }

        public void SetVisualState(int orderOffset, bool maskActive)
        {
            if (maskGO) maskGO.SetActive(maskActive);

            if (maskActive && mask != null && _cachedMaps.Count > 0)
            {
                int minOrder = int.MaxValue;
                int maxOrder = int.MinValue;

                // Move Tilemaps & Find Range
                foreach (var t in _cachedMaps)
                {
                    if (t.renderer)
                    {
                        int newOrder = t.baseOrder + orderOffset;
                        t.renderer.sortingOrder = newOrder;

                        if (newOrder < minOrder) minOrder = newOrder;
                        if (newOrder > maxOrder) maxOrder = newOrder;
                    }
                }

                // --- SAFETY BUFFER FIX ---
                // We expand the mask by 1 on both sides. 
                // If your maps are 0 and 1, the mask becomes -1 to 2.
                // This guarantees inclusive visibility.
                mask.isCustomRangeActive = true;
                mask.backSortingOrder = minOrder - 1; 
                mask.frontSortingOrder = maxOrder + 1;
            }
            else
            {
                // Just move the tilemaps blindly if mask is inactive
                foreach (var t in _cachedMaps)
                {
                    if (t.renderer) t.renderer.sortingOrder = t.baseOrder + orderOffset;
                }
            }
        }
    }

    [Header("Pattern Backgrounds")]
    public BackgroundSet lineBackground; // Future
    public BackgroundSet coneBackground; // Past

    [Header("Feedbacks")]
    [SerializeField] private MMF_Player swapLevelFeedback;
    private PlayerAttackPattern _currentPattern;
    private Coroutine _transitionCoroutine;

    void Awake()
    {
        lineBackground.Init(bgSortingLayerName);
        coneBackground.Init(bgSortingLayerName);
    }

    public void ForceSetBackground(PlayerAttackPattern pattern)
    {
        _currentPattern = pattern;

        if (pattern == PlayerAttackPattern.Line)
        {
            lineBackground.SetVisualState(ORDER_OFFSET_STANDARD, true);
            coneBackground.SetVisualState(ORDER_OFFSET_HIDDEN, false);
        }
        else
        {
            coneBackground.SetVisualState(ORDER_OFFSET_STANDARD, true);
            lineBackground.SetVisualState(ORDER_OFFSET_HIDDEN, false);
        }
    }

    public void SwitchBackground(PlayerAttackPattern newPattern)
    {
        if (_currentPattern == newPattern) return;

        if (_transitionCoroutine != null) StopCoroutine(_transitionCoroutine);
        _transitionCoroutine = StartCoroutine(SwitchRoutine(newPattern));
        
        swapLevelFeedback.PlayFeedbacks();
    }

    private IEnumerator SwitchRoutine(PlayerAttackPattern newPattern)
    {
        BackgroundSet newBG = (newPattern == PlayerAttackPattern.Line) ? lineBackground : coneBackground;
        BackgroundSet oldBG = (newPattern == PlayerAttackPattern.Line) ? coneBackground : lineBackground;

        // 1. Move OLD BG to Standard (Base + 0)
        oldBG.SetVisualState(ORDER_OFFSET_STANDARD, true); 
        
        // 2. Move NEW BG to Overlay (Base + 10)
        newBG.SetVisualState(ORDER_OFFSET_OVERLAY, true);

        _currentPattern = newPattern;

        yield return new WaitForSeconds(transitionDuration);

        // 3. Settle
        newBG.SetVisualState(ORDER_OFFSET_STANDARD, true);
        oldBG.SetVisualState(ORDER_OFFSET_HIDDEN, false);
    }
}