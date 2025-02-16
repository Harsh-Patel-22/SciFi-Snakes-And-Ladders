using System.Collections;
using SnakesAndLadders;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUIManager : MonoBehaviour
{
    [SerializeField] private Image loadingCircle;
    private float loadSpeed = 2f;
    private float fillAmount;
    private bool isReseting;
    private bool clockwise;
    private void Start() {
        fillAmount = 0f;
        isReseting = false;
        clockwise = true;
    }
    private void Update() {
        if (clockwise) {
            fillAmount += Time.deltaTime * loadSpeed;
        } else {
            fillAmount -= Time.deltaTime * loadSpeed;
        }
        //Debug.Log(Mathf.Clamp01(fillAmount));
        loadingCircle.fillAmount = Mathf.Clamp01(fillAmount);

        if (clockwise && fillAmount > 0.99f && !isReseting) {
            //StartCoroutine(GenerateDelayForLoad(1));
            //GameManager.Instance.LoadingFinish();
        }
        else if(!clockwise && fillAmount < 0.01f && !isReseting) {
            StartCoroutine(GenerateDelayForLoad(0));

        }
    }

    private IEnumerator GenerateDelayForLoad(float fillAmount) {
        isReseting = true;
        yield return new WaitForSeconds(1f);
        this.fillAmount = fillAmount;
        clockwise = !clockwise;
        loadingCircle.fillClockwise = clockwise;
        isReseting = false;
    }
}
