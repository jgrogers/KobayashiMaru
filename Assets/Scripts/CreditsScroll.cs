using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsScroll : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 50.0f;
    [SerializeField] private TMPro.TMP_Text credits;
    [SerializeField] private Canvas canvas;
    public float canvasHeight;
    public float textHeight;
    // Start is called before the first frame update
    private IEnumerator LoadIntro()
    {
        yield return new WaitForSeconds(20.0f);
        SceneManager.LoadScene(1);
    }
    void Start()
    {
        canvasHeight = canvas.GetComponent<RectTransform>().rect.height;
        textHeight = credits.GetPreferredValues().y * 0.8f;
        transform.position += new Vector3(0f, -textHeight / 2.0f, 0f);
        StartCoroutine(LoadIntro());
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0f, scrollSpeed * Time.deltaTime, 0f);
        if (transform.position.y > canvasHeight + textHeight / 2.0f)
        {
            SceneManager.LoadScene(0);
        }
    }
}
