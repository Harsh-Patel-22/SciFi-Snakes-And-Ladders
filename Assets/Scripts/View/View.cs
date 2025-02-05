using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SnakesAndLadders {

    public class View : MonoBehaviour
    {
        public event Action<int> OnPlayerTurnClicked;
        public event Action<int> OnPlayerAbilityClicked;
        public event Action OnPauseButtonClicked;
        public event Action OnPlayerSet;
        public event Action OnSnakeLadderInteractionComplete;
        public event Action OnAbilityAnimationComplete;

        public event Action<int, string> OnAbilityAnimationPlay;


        // Think of a way to store the diceBUttons, playerimages, ability icons together & store access with a single index
        [SerializeField] private TileManager tileManager;
        [SerializeField] private AnimationHandler animationHandler;
        [SerializeField] private Image[] playerImages;
        [SerializeField] private Button[] diceButtons;
        [SerializeField] private Sprite[] diceSprites;
        [SerializeField] private GameObject displayScreen;
        [SerializeField] private Image displayScreenImage;
        [SerializeField] private DisplayScreenListSO displayScreenSprites;
        [SerializeField] private Button[] abilityButtons;
        [SerializeField] private Animator[] abilityLocks;
        [SerializeField] private GameObject gameOverScreen;
        public enum DisplayText {
            SnakeBit,
            LadderClimbed,
            EnergyEruption
        }

        private bool shrinkComplete;
        private bool enlargenComplete;


        public void Setup() {
            shrinkComplete = false;
            enlargenComplete = false;
            UpdateAbilitiesState(null);
            tileManager.Setup();

            displayScreen.gameObject.SetActive(false);
            gameOverScreen.SetActive(false);

            foreach (var button in abilityButtons) {
                button.interactable = false;
            }
        }

        public void UpdateAbilitiesState(bool[] toBeTrue) {
            if(toBeTrue != null) {
                for (int i = 0; i < abilityButtons.Length; i++) {
                    abilityButtons[i].interactable = toBeTrue[i];
                }
            }
            
        }

        public void UnlockAbility(int playerIndex) {
            abilityLocks[playerIndex].Play("unlock");
            SetAbilityButton(playerIndex, true);
        }

        public void SetDiceSprite(int diceIndex, int spriteIndex) {
            StartCoroutine(PlayDiceAnimation(diceIndex, spriteIndex));
        }

        public void SetAbilityButton(int index, bool state) {
            abilityButtons[index].interactable = state;
        }

        public void SetPlayerDirect(int playerIndex, int position) {
            RectTransform tileRectTransform = tileManager.GetTile(position).GetRectTransform();
            float width = tileRectTransform.rect.width;
            float height = tileRectTransform.rect.height;
            //Debug.Log(width + "\n" + height);
            ConfigPlayer(playerImages[playerIndex].rectTransform, tileRectTransform, width, height);
        }

        public void SetPlayerOn(int playerIndex, int startPosition, int endPosition) {
            DisableAllDiceButtons();
            StartCoroutine(SetPlayerOnTile(playerIndex, startPosition, endPosition));
        }

        public void SetPlayerAnchor(int playerIndex, int position, string anchor) {
            Tile tile = tileManager.GetTile(position);
            SetOnTile(tile, playerImages[playerIndex].rectTransform, anchor);
        }

        public void SnakeLadderInteraction(int playerIndex, DisplayText displayText, int start, int end) {
            Tile startTile = tileManager.GetTile(start);
            Tile endTile = tileManager.GetTile(end);
            SetAndShowDisplayText(displayText);
            StartCoroutine(SnakeladderInteractionHandlerCoroutine(playerIndex, startTile, endTile, displayText));
        }

        public void SetAndShowDisplayText(DisplayText displayText) {
            switch (displayText) {
                case DisplayText.SnakeBit:
                    displayScreenImage.sprite = displayScreenSprites.snakeBitImage;
                    break;
                case DisplayText.LadderClimbed:
                    displayScreenImage.sprite = displayScreenSprites.ladderClimbedImage;
                    break;
                case DisplayText.EnergyEruption:
                    displayScreenImage.sprite = displayScreenSprites.energyEruptionImage;
                    break;
            }
            //displayLabel.text = text;
            displayScreen.gameObject.SetActive(true);
            animationHandler.PlayDisplayImageAnimation(OnDisplayAnimationFinish);
        }


        public void OnDisplayAnimationFinish() {
            displayScreen.gameObject.SetActive(false);
        }

        public void BlastAbilityAnimation() {
            SetAndShowDisplayText(DisplayText.EnergyEruption);
            animationHandler.PlayBlastAnimation(tileManager.GetTile(51), OnDisplayAnimationFinish);
        }


        private IEnumerator SetPlayerOnTile(int playerIndex, int startPosition, int endPosition) {
            WaitForSeconds wait = new WaitForSeconds(0.2f);
            for (int i = startPosition + 1; i <= endPosition; i++) { // Fix the condition, in case of snake bite, it wont trigger
                SetOnTile(tileManager.GetTile(i), playerImages[playerIndex].rectTransform, "center");
                yield return wait;
            }
            OnPlayerSet?.Invoke();
        }

        private IEnumerator SnakeladderInteractionHandlerCoroutine(int playerIndex, Tile startTile, Tile endTile, DisplayText DisplayText) {
            RectTransform playerRectTransform = playerImages[playerIndex].rectTransform;
            StartCoroutine(ZoomEffect(startTile, playerRectTransform, false));
            yield return new WaitUntil(() => shrinkComplete);
            shrinkComplete = false;
            StartCoroutine(ZoomEffect(endTile, playerRectTransform, true));
            yield return new WaitUntil(() => enlargenComplete);
            enlargenComplete = false;   
            OnSnakeLadderInteractionComplete?.Invoke();
        }

        private void SetOnTile(Tile tile, RectTransform playerRectTransform ,string corner) {
            var targetTileRectTransform = tile.GetRectTransform();
            var sizeScaler = 1f;
            var offset = Vector3.zero;
            float offsetMultiplier = 4f;
            float width = targetTileRectTransform.rect.width;
            float height = targetTileRectTransform.rect.height;
            if (corner != "center") {
                sizeScaler = 0.45f;
                if (corner == "topleft") {
                    offset = new Vector3(-width * (sizeScaler) / offsetMultiplier, height * (sizeScaler) / offsetMultiplier);
                } else if (corner == "bottomright") {
                    offset = new Vector3(width * (sizeScaler) / offsetMultiplier, -height * (sizeScaler) / offsetMultiplier);
                }
            }
            // make the config function generic to add the custom logic. Default arguments!!

            //ConfigPlayer(playerRectTransform, targetTileRectTransform, width * sizeScaler, height * sizeScaler); add fields to accept offset
            playerRectTransform.position = targetTileRectTransform.position + offset;
            playerRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width * sizeScaler);
            playerRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height * sizeScaler);
        }

        private IEnumerator ZoomEffect(Tile tile, RectTransform playerRectTransform, bool enlargen) {
            int numberOfZooms = 10;
            int zoomFactor = 10;
            RectTransform tileRectTransform = tile.GetRectTransform();
            float width;
            float height;

            WaitForSeconds wait = new WaitForSeconds(0.1f);

            if (enlargen) {
                
                for (int i = 1; i < numberOfZooms; i++) {
                    //Debug.Log("Enlargening");
                    width = (float) i * tileRectTransform.rect.width / zoomFactor;
                    height = (float) i * tileRectTransform.rect.height / zoomFactor;
                    ConfigPlayer(playerRectTransform, tileRectTransform, width, height);
                    yield return wait;
                }
                enlargenComplete = true;
            } else {
                for (int i = numberOfZooms; i > 1; i--)
                {
                //Debug.Log("shrinking"); 
                    width = (float) i * tileRectTransform.rect.width / zoomFactor;
                    height = (float) i * tileRectTransform.rect.height / zoomFactor;
                    ConfigPlayer(playerRectTransform, tileRectTransform, width, height);
                    yield return wait;
                }
                shrinkComplete = true;
            }
        }

        private IEnumerator PlayDiceAnimation(int index, int finalValueIndex) {
            int times = 3;
            int randomNumber;
            WaitForSeconds wait = new WaitForSeconds(0.05f);
            for (int i = 0; i < times; i++) {
                randomNumber = UnityEngine.Random.Range(1, 6);
                diceButtons[index].image.sprite = diceSprites[randomNumber];
                yield return wait;
            }
            animationHandler.PlayRollTextAnimation(index, finalValueIndex + 1);
            diceButtons[index].image.sprite = diceSprites[finalValueIndex];
        }

        private void ConfigPlayer(RectTransform playerRectTransform, RectTransform tileRectTransform, float width, float height) {
            playerRectTransform.position = tileRectTransform.position;
            playerRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            playerRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

        }

        public void DisableOtherDiceButtons(int index) {
            for (int i = 0; i < diceButtons.Length; i++) {
                if (i != index) {
                    diceButtons[i].interactable = false;
                } else {
                    diceButtons[i].interactable = true;
                }
            }
        }

        private void DisableAllDiceButtons() {
            for (int i = 0; i < diceButtons.Length; i++) {
                diceButtons[i].interactable = false;
            }
        }

        

        public void SetSnakePositions(List<int> positions) {
            tileManager.RGBEffect = false;
            tileManager.SetSnakes(positions);
            tileManager.RGBEffect = true;
        }
        public void SetLadderPositions(List<int> positions) {
            tileManager.RGBEffect = false;
            tileManager.SetLadders(positions);
            tileManager.RGBEffect = true;
        }

        // Listener functions to on screen buttons
        public void HandlePlayerTurn(int playerIndex) {
            OnPlayerTurnClicked?.Invoke(playerIndex);
        }

        public void HandlePlayerAbilityClicked(int playerIndex) {
            OnPlayerAbilityClicked?.Invoke(playerIndex);
        }

        public void HandlePauseButtonClicked() { }


        public void ShowGameOverScreen() {
            gameOverScreen.SetActive(true);
        }
    }

}