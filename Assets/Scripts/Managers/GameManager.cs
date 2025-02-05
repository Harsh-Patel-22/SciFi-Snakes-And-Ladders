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
            GameSceneManager.Instance.SetActiveScene("Load");
            gameConfigDataIndex = index;
            //StartCoroutine(ConfigPresenterAfterDelay());
            checkForSceneLoad = true; 
            
            state = GameState.Loading;
        }

        public void LoadingFinish() {
            state = GameState.Instructions;
            GameSceneManager.Instance.SetActiveScene("Play");
        }

        public void RestartTheGame() {
            GameSceneManager.Instance.SetActiveScene("Load");
            //StartCoroutine(ConfigPresenterAfterDelay()); 
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

        //private IEnumerator ConfigPresenterAfterDelay() {
        //    yield return new WaitForSeconds(0.5f);
        //    Presenter presenter = FindAnyObjectByType<Presenter>();
        //    presenter.Setup(gameConfigData[gameConfigDataIndex]);
        //}

    }
}