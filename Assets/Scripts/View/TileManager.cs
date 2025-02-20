using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileManager : MonoBehaviour {

    [SerializeField] private Tile[] tiles;
    [SerializeField] private Sprite snakeTeethImage;
    [SerializeField] private Image[] snakeTeethImageList;
    public bool RGBEffect { get; set; }
    private int colorCycle = 0;
    private Color blackForSnakes;
    private Color blackForLadders;
    private float scaleFactor = 0f;

    private List<int> snakeTiles;
    private List<int> ladderTiles;

    public void Setup() { 
        RGBEffect = true;
        blackForSnakes = new Color(1,0,0,0.2f);
        blackForLadders = new Color(0,1,0,0.2f);
        for (int i = 0; i < tiles.Length; i++) {
            tiles[i].TileNumber = i;
            tiles[i].SetTileNumberText(i + 1);
        }

        for (int i = 0; i < snakeTeethImageList.Length; i++) {
            snakeTeethImageList[i].sprite = snakeTeethImage;
            snakeTeethImageList[i].gameObject.SetActive(false); 
        }

        Tile tile;
        
        for (int i = 0; i < tiles.Length; i++) {
            tile = tiles[i];
            tile.TileImage.color = Color.white;
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

        for(int i = 0; i < tiles.Length; i++) {
            tile = tiles[i];
            tile.TileImage.color = Color.white;
        }

        if (snakeTiles != null) {
            for (int i = 0; i < snakeTiles.Count; i++) {
                tile = GetTile(snakeTiles[i]);
                if (colorCycle % 2 == 0) {
                    tileColor = Color.Lerp(blackForSnakes, Color.red, scale);
                    tile.TileImage.color = tileColor;
                } else {
                    tileColor = Color.Lerp(Color.red, blackForSnakes, scale);
                    tile.TileImage.color = tileColor;
                }
                if (tileColor == Color.red || tileColor == blackForSnakes) {
                    colorCycle++;
                    scale = 0;
                    break;
                }
            }
        }

        if (ladderTiles != null) {
            for (int i = 0; i < ladderTiles.Count; i++) {
                tile = GetTile(ladderTiles[i]);
                if (colorCycle % 2 == 0) {
                    tileColor = Color.Lerp(blackForLadders, Color.green, scale);
                    tile.TileImage.color = tileColor;
                } else {
                    tileColor = Color.Lerp(Color.green, blackForLadders, scale);
                    tile.TileImage.color = tileColor;
                }
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
