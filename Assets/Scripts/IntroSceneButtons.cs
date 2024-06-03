using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroScene : MonoBehaviour
{
    [SerializeField] private GameObject loadingText;
    private void Start() {
        loadingText.SetActive(false);
    }
    public void OnClickControls() {
        loadingText.SetActive(true);
        SceneManager.LoadScene(2);

    }
    public void OnClickTutorial() {
        loadingText.SetActive(true);
        SceneManager.LoadScene(3);

    }
    public void OnClickMission() {
        loadingText.SetActive(true);
        SceneManager.LoadScene(3);

    }
    public void OnClickCredits()
    {
        loadingText.SetActive(true);
        SceneManager.LoadScene(4);

    }

}
