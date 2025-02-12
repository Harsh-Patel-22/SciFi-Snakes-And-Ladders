using SnakesAndLadders;
using UnityEngine;
using UnityEngine.UI;

public class TutorialScreenHandler : MonoBehaviour
{
    [SerializeField] private Button continue1;
    [SerializeField] private Button continue2;

    private void Start() {
        continue1.onClick.AddListener(() => {
            continue1.gameObject.SetActive(false);
            continue2.gameObject.SetActive(true);
        });

        continue2.onClick.AddListener(() => {
            continue2.gameObject.SetActive(false);
            GameManager.Instance.SetPlayMode();
        });
    }
}
