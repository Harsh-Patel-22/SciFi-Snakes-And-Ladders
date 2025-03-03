using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SnakesAndLadders {

    public class AnimationHandler : MonoBehaviour
    {
        private const string PLAY_DISPLAY_TEXT_ANIMATION = "PlayDisplayTextAnimation";
        private const string PLAY_ROLL_TEXT_ANIMATION = "PlayRollTextAnimation";
        [SerializeField] private Animator displayImageAnimator;
        [SerializeField] private Animator[] rollTextAnimators;
        [SerializeField] private Image drone;
        [SerializeField] private ParticleSystem[] abilityUnlockparticleSystems;
        [SerializeField] private ParticleSystem displayParticleSystem;
        [SerializeField] private Sprite[] diceSprites;
        private TextMeshProUGUI[] rollTexts;


        private void Start() {
            rollTexts = new TextMeshProUGUI[rollTextAnimators.Length];
            for (int i = 0; i < rollTextAnimators.Length; i++) {
                rollTexts[i] = rollTextAnimators[i].gameObject.GetComponent<TextMeshProUGUI>();
                rollTextAnimators[i].gameObject.SetActive(false);
            }
        }

        public void HideObject() {
            gameObject.SetActive(false);
        }

        public void PlayDisplayImageAnimation(Color color, Action callback) {
            displayImageAnimator.SetTrigger(PLAY_DISPLAY_TEXT_ANIMATION);
            // TODO - code to play particle effect
            displayParticleSystem.startColor = color;
            displayParticleSystem.Play();
            StartCoroutine(WaitForAnimationFinish(displayImageAnimator.GetCurrentAnimatorStateInfo(0).length, callback));
        }
        public void PlayRollTextAnimation(int index, int rollNumber) {
            rollTextAnimators[index].gameObject.SetActive(true);
            rollTexts[index].text = rollNumber.ToString();
            rollTextAnimators[index].SetTrigger(PLAY_ROLL_TEXT_ANIMATION);
            StartCoroutine(WaitForAnimationFinish(rollTextAnimators[index].GetCurrentAnimatorStateInfo(0).length, () => HideRollText(index)));
        }

        public void PlayAbilityUnlockParticleSystem(int index) {
            abilityUnlockparticleSystems[index].Play();
        }

        private IEnumerator WaitForAnimationFinish(float seconds, Action callback) {
            yield return new WaitForSeconds(seconds);
            callback();
        }

        private void HideRollText(int index) {
            rollTexts[index].gameObject.SetActive(false);
        }

        public void PlayBlastAnimation(Tile tile, Action callback) {
            StartCoroutine(BlastAnimationCoroutine(tile, callback));
        }

        private IEnumerator BlastAnimationCoroutine(Tile tile, Action callback) {
            float waitTime = 0.1f;
            float x;
            drone.gameObject.SetActive(true);
            WaitForSeconds wait = new WaitForSeconds(waitTime);
            drone.transform.position = tile.transform.position + new Vector3(500f, 0f, 0f);
            Vector3 endPosition = -drone.transform.position;
            while(waitTime < 1f) {
                x = Mathf.Lerp(drone.transform.position.x, endPosition.x, waitTime);
                drone.transform.position = new Vector3(x, drone.transform.position.y);
                waitTime += 0.1f;
                yield return wait;
            } 
            drone.gameObject.SetActive(false);
            callback();
        }

        public void PlayDiceAnimation(int index, int finalValueIndex, Button diceButton) {
            StartCoroutine(PlayDiceAnimationCoroutine(index, finalValueIndex, diceButton));
        }

        // TODO - Some weird bug in game due to this
        private IEnumerator PlayDiceAnimationCoroutine(int index, int finalValueIndex, Button diceButton) {

            int times = 4;
            WaitForSeconds wait = new WaitForSeconds(0.03f);
            for (int i = 0; i < times; i++) {
                int randomNumber = finalValueIndex;
                while (randomNumber == finalValueIndex) {
                    randomNumber = UnityEngine.Random.Range(1, 6);
                }
                diceButton.image.sprite = diceSprites[randomNumber];
                yield return wait;
            }
            PlayRollTextAnimation(index, finalValueIndex + 1);
            diceButton.image.sprite = diceSprites[finalValueIndex];
        }

    }

}