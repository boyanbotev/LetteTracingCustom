using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private void OnEnable()
    {
        TracingManager.OnComplete += Complete;
    }

    private void OnDisable()
    {
        TracingManager.OnComplete -= Complete;
    }

    void Complete()
    {
        StartCoroutine(CompleteRoutine());
    }

    public void GoToNextScene()
    {
        var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        var nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("No more scenes to load");
        }
    }

    IEnumerator CompleteRoutine()
    {
        yield return new WaitForSeconds(1f);
        GoToNextScene();
    }
}
