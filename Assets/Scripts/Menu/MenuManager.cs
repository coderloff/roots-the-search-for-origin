using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] Animator Scene;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play(string sceneName)
    {
        StartCoroutine(LoadScene(sceneName));
    }

    public void Exit()
    {
        Application.Quit();
    }

    IEnumerator LoadScene(string sceneName)
    {
        Scene.SetTrigger("Load");
        yield return new WaitForSeconds(0.8f);
        SceneManager.LoadScene(sceneName);
    }
}
