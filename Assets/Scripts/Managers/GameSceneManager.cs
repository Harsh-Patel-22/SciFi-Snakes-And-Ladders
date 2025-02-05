using UnityEngine;
using UnityEngine.SceneManagement;

namespace SnakesAndLadders {

    public class GameSceneManager : MonoBehaviour
    {
        public static GameSceneManager Instance { get; private set; }
        private void Awake() {
            if (Instance == null) {
                Instance = this;
            } else {
                Destroy(this);
            }      
        }

        public void SetActiveScene(string name) {
            SceneManager.LoadSceneAsync(name);
        }

        public string GetActiveScene() {
            return SceneManager.GetActiveScene().name;
        }

    }
}
