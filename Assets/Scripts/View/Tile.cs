using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public int TileNumber { get; set; }
    public Image TileImage { get; set; }

    private RectTransform rectTransform;
    //private Text tileNumberLabel;
    private TextMeshProUGUI tileNumberLabel;
    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
        TileImage = GetComponent<Image>();
        tileNumberLabel = GetComponentInChildren<TextMeshProUGUI>();
    }

    public RectTransform GetRectTransform() { return rectTransform; }
    public void SetTileNumberText(int number) {
        tileNumberLabel.text = number.ToString();
    }
}
