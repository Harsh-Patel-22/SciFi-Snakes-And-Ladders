using UnityEngine;

namespace SnakesAndLadders {

    public class HomeUIManager : MonoBehaviour
    {
        private const string PLAY_DRONE_ANIMATION = "PlayDroneAnimation";
        [SerializeField] private Animator droneAnimator;

        private float droneAnimationMax = 7f;
        private float droneAnimationDelayTimer;
        public void VsButtonClicked(int index) {
            GameManager.Instance.VsButtonCLicked(index);
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
