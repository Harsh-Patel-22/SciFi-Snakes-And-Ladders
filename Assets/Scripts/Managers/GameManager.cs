using System;
using System.Collections;
using System.Reflection;
using UnityEngine;


namespace SnakesAndLadders {

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        [SerializeField] GameConfigSO[] gameConfigData;
        private int gameConfigDataIndex;
        private bool checkForSceneLoad;

        private enum GameState {
            Home,
            Loading,
            Instructions,
            Play
        }

        GameState state;

        private void Awake() {
            if (Instance == null) {
                Instance = this;
                checkForSceneLoad = false;
                DontDestroyOnLoad(gameObject);
            } else {
                Destroy(gameObject);
                checkForSceneLoad = false;
            }
        }

        private void Start() {
            state = GameState.Home;
        }

        public void VsButtonCLicked(int index) {
            AudioHandler.Instance.PlaySound(AudioHandler.Sounds.start);
            GameSceneManager.Instance.SetActiveScene("Load");
            gameConfigDataIndex = index;
            checkForSceneLoad = true;
            state = GameState.Loading;
        }

        public void LoadingFinish() {
            state = GameState.Instructions;
            GameSceneManager.Instance.SetActiveScene("Play");
        }

        public void RestartTheGame() {
            AudioHandler.Instance.PlaySound(AudioHandler.Sounds.start);
            GameSceneManager.Instance.SetActiveScene("Load");
            Time.timeScale = 1f;
            checkForSceneLoad = true;
            state=GameState.Loading;
        }

        public void ExitPlay() {
            GameSceneManager.Instance.SetActiveScene("Home");
            Time.timeScale = 1.0f;
        }

        public void SetPlayMode() {
            state = GameState.Play;
            Presenter presenter = FindAnyObjectByType<Presenter>();
            presenter.Setup(gameConfigData[gameConfigDataIndex]);
            
        }

    }
}