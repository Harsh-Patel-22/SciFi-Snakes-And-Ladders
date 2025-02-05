using System;
using System.Collections;
using System.Collections.Generic;
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

        public void PlayDisplayImageAnimation(Action callback) {
            displayImageAnimator.SetTrigger(PLAY_DISPLAY_TEXT_ANIMATION);
            StartCoroutine(WaitForAnimationFinish(displayImageAnimator.GetCurrentAnimatorStateInfo(0).length, callback));
        }
        public void PlayRollTextAnimation(int index, int rollNumber) {
            rollTextAnimators[index].gameObject.SetActive(true);
            rollTexts[index].text = rollNumber.ToString();
            rollTextAnimators[index].SetTrigger(PLAY_ROLL_TEXT_ANIMATION);
            StartCoroutine(WaitForAnimationFinish(rollTextAnimators[index].GetCurrentAnimatorStateInfo(0).length, () => HideRollText(index)));
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
            WaitForSeconds wait = new WaitForSeconds(waitTime);
            drone.transform.position = tile.transform.position + new Vector3(100f, 0f, 0f);
            while(waitTime < 2f) {
                x = Mathf.Lerp(drone.transform.position.x, -drone.transform.position.x, waitTime/2);
                drone.transform.position = new Vector3(x, drone.transform.position.y);
                waitTime += 0.1f;
                yield return wait;
            } 
            drone.gameObject.SetActive(false);
            callback();
        }

    }

}