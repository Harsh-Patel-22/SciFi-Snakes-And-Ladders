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
                DontDestroyOnLoad(gameObject);
            } else {
                Destroy(gameObject);
            }
        }

        private void Start() {
            state = GameState.Home;
        }

        public void VsButtonCLicked(int index) {
            gameConfigDataIndex = index;
            Load();
        }

        public void LoadingFinish() {
            state = GameState.Instructions;
            GameSceneManager.Instance.SetActiveScene("Play");
        }

        public void RestartTheGame() {
            Load();
        }

        private void Load() {
            AudioHandler.Instance.PlaySound(AudioHandler.Sounds.start);
            GameSceneManager.Instance.SetActiveScene("Load");
            float delay = AudioHandler.Instance.PlaySound(AudioHandler.Sounds.loading);
            Time.timeScale = 1f;
            state = GameState.Loading;
            StartCoroutine(ExecuteAfterDelay(LoadingFinish, delay));
        }

        private IEnumerator ExecuteAfterDelay(Action callback, float delay) {
            yield return new WaitForSeconds(delay);
            callback();
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