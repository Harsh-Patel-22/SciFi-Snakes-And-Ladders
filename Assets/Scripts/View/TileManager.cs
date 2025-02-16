using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TileManager : MonoBehaviour {

    [SerializeField] private Tile[] tiles;
    [SerializeField] private Sprite snakeTeethImage;
    [SerializeField] private Image[] snakeTeethImageList;
    public bool RGBEffect { get; set; }
    private Color color1;
    private Color color2;
    private int colorCycle = 0;
    private float scaleFactor = 0f;

    private List<int> snakeTiles;
    private List<int> ladderTiles;

    public void Setup() { 
        color1 = new Color(176f / 255f, 98f / 255f, 251f / 255f, 255f / 255f);
        color2 = new Color(0f, 0f, 1f, 1f);
        RGBEffect = true;
        for (int i = 0; i < tiles.Length; i++) {
            tiles[i].TileNumber = i;
            tiles[i].SetTileNumberText(i + 1);
        }

        for (int i = 0; i < snakeTeethImageList.Length; i++) {
            snakeTeethImageList[i].sprite = snakeTeethImage;
            snakeTeethImageList[i].gameObject.SetActive(false); 
        }
    }

    public void SetSnakes(List<int> positions) {
        snakeTiles = positions;
        for (int i = 0;i < snakeTiles.Count; i++) {
            Tile tile = GetTile(snakeTiles[i]);
            float width = tile.GetRectTransform().rect.width;
            float height = tile.GetRectTransform().rect.height;

            snakeTeethImageList[i].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            snakeTeethImageList[i].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            snakeTeethImageList[i].transform.position = tile.transform.position;
            snakeTeethImageList[i].gameObject.SetActive(true);
        }
        for (int i = snakeTiles.Count; i < snakeTeethImageList.Length; i++) {
            snakeTeethImageList[i].gameObject.SetActive(false);
        }
    }
    public void SetLadders(List<int> positions) {
        ladderTiles = positions;
    }

    // Not Needed
    public Tile GetTile(int tileNumber) {
        return tiles[tileNumber];
    }


    private void RGBTileEffect(ref float scale) {
        Tile tile;
        Color tileColor;
        for (int i = 0; i < tiles.Length; i++) {
                tile = tiles[i];
                tileColor = tile.TileImage.color;
                if(colorCycle % 2 == 0) {
                    tileColor = Color.Lerp(color1, color2, scale);
                    tile.TileImage.color = tileColor;
                } else {
                    tileColor = Color.Lerp(color2, color1, scale);
                    tile.TileImage.color = tileColor;
                }
                if (tileColor == color2 || tileColor == color1) {
                    colorCycle++;
                    scale = 0;
                    break;
                }
        }
        if (snakeTiles != null && ladderTiles != null) {
            for (int i = 0; i < snakeTiles.Count; i++) {
                tile = GetTile(snakeTiles[i]);
                tile.TileImage.color = Color.red;
            }
            for (int i = 0; i < ladderTiles.Count; i++) {
                tile = GetTile(ladderTiles[i]);
                tile.TileImage.color = Color.green;
               }
        }
    }

    private void FixedUpdate() {
        if (RGBEffect) {
            scaleFactor += Time.deltaTime * 0.75f;
            RGBTileEffect(ref scaleFactor);
        }
    }
}
