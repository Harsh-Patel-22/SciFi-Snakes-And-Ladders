using SnakesAndLadders;
using UnityEngine;
using UnityEngine.UI;

public class TutorialScreenHandler : MonoBehaviour
{
    [SerializeField] private Button exitTutorial;

    private void Start() {
        exitTutorial.onClick.AddListener(() => {
            exitTutorial.gameObject.SetActive(false);
            GameManager.Instance.SetPlayMode();
        });
    }
}
