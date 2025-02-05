using UnityEngine;
using UnityEngine.UI;

namespace SnakesAndLadders {

    public class GameOverUIManager : MonoBehaviour
    {
        [SerializeField] private Button homeButton;
        [SerializeField] private Button replayButton;

        private void Start() {
            homeButton.onClick.AddListener(() => {
                GameManager.Instance.ExitPlay();
            });

            replayButton.onClick.AddListener(() => {
                GameManager.Instance.RestartTheGame();
            });
        }
    }

}