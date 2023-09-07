using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderSingleton : MonoBehaviour
{
    public static SceneLoaderSingleton instance;

    [SerializeField]
    private CanvasGroup loadingCanvas;

    // Fade IN 처리 시간
    [Range(0.5f, 2.0f)]
    public float fadeDuration = 1.0f;

    private void Awake()
    {
        if (instance is null)
            instance = this;
    }

    private void Start() { }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().ToString();
    }

    public void LoadSceneAsync(string sceneName)
    {
        StartCoroutine(Fade(1.0f, () => StartCoroutine(AsyncLoader(sceneName))));
    }

    IEnumerator AsyncLoader(string sceneName)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);

        ao.allowSceneActivation = false;

        while (!ao.isDone)
        {
            yield return null;

            // 만일, 씬 로드 진행률이 90%를 넘어가면...
            if (ao.progress >= 0.9f)
            {
                // 로드된 씬을 화면에 보이게 한다.
                ao.allowSceneActivation = true;
            }
        }
        StartCoroutine(Fade(0.0f, () => BackgroundDestroyCheck(sceneName)));
    }

    private void BackgroundDestroyCheck(string _sceneName)
    {
        if(_sceneName == "03 MainScene")
        {
            Destroy(loadingCanvas.gameObject);
        }
        else
        {
            loadingCanvas.gameObject.SetActive(false);
        }
    }

    // Fade In/Out 시키는 함수
    IEnumerator Fade(float finalAlpha)
    {
        loadingCanvas.gameObject.SetActive(true);
        loadingCanvas.blocksRaycasts = true;

        // 절대값 함수로 백분율 계산
        float fadeSpeed = Mathf.Abs(loadingCanvas.alpha - finalAlpha) / fadeDuration;

        // 알파값을 조정
        while (!Mathf.Approximately(loadingCanvas.alpha, finalAlpha))
        {
            // MoveToward 함수는 Lerp 함수와 동일한 함수로 알파값을 보간함
            loadingCanvas.alpha = Mathf.MoveTowards(
                loadingCanvas.alpha,
                finalAlpha,
                fadeSpeed * Time.deltaTime
            );
            yield return null;
        }

        loadingCanvas.blocksRaycasts = false;
    }

    // Fade In/Out 시키는 함수
    IEnumerator Fade(float finalAlpha, Action callback)
    {
        loadingCanvas.gameObject.SetActive(true);
        loadingCanvas.blocksRaycasts = true;

        // 절대값 함수로 백분율 계산
        float fadeSpeed = Mathf.Abs(loadingCanvas.alpha - finalAlpha) / fadeDuration;

        // 알파값을 조정
        while (!Mathf.Approximately(loadingCanvas.alpha, finalAlpha))
        {
            // MoveToward 함수는 Lerp 함수와 동일한 함수로 알파값을 보간함
            loadingCanvas.alpha = Mathf.MoveTowards(
                loadingCanvas.alpha,
                finalAlpha,
                fadeSpeed * Time.deltaTime
            );
            yield return null;
        }

        loadingCanvas.blocksRaycasts = false;

        callback.Invoke();
    }
}
