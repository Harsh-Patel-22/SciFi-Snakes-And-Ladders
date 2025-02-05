using UnityEngine;

namespace SnakesAndLadders {

    public class PauseManager : MonoBehaviour
    {

        [SerializeField] private GameObject pauseScreen;
        public void Pause() {
            pauseScreen.SetActive(true);
            Time.timeScale = 0f;
        }

        public void Resume() {
            pauseScreen?.SetActive(false);
            Time.timeScale = 1f;
        }

        public void Quit() {
            GameManager.Instance.ExitPlay();
        }
    }
}
