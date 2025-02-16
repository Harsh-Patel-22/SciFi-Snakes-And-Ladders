using System;
using System.Collections;
using UnityEngine;

namespace SnakesAndLadders {

    public class HomeUIManager : MonoBehaviour
    {
        private const string PLAY_DRONE_ANIMATION = "PlayDroneAnimation";
        [SerializeField] private Animator droneAnimator;
        [SerializeField] private ParticleSystem[] clickParticleSystem;

        private float droneAnimationMax = 7f;
        private float droneAnimationDelayTimer;
        public void VsButtonClicked(int index) {
            clickParticleSystem[index].Play();
            StartCoroutine(ExecuteAfterDelay(() => { GameManager.Instance.VsButtonCLicked(index); }, clickParticleSystem[index].main.duration + clickParticleSystem[index].main.startLifetimeMultiplier));
        }

        private IEnumerator ExecuteAfterDelay(Action callback, float delay) {
            yield return new WaitForSeconds(delay);
            callback();
        }

        private void Start() {
            droneAnimationDelayTimer = droneAnimationMax;
        }

        private void Update() {
            droneAnimationDelayTimer -= Time.deltaTime;
            if(droneAnimationDelayTimer <= 0) {
                droneAnimator.SetTrigger(PLAY_DRONE_ANIMATION);
                droneAnimationDelayTimer = droneAnimationMax;
            }
        }
    }
}
