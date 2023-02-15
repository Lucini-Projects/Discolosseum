using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneLoader : MonoBehaviour
{
    private List<Scene> scenes;
    public int inspectorsceneindex = 0;

    // Start is called before the first frame update
    void Start()
    {
        scenes = new List<Scene>();
        scenes.Add(SceneManager.GetActiveScene());
        for (int i = scenes[0].buildIndex + 1; i < scenes[0].buildIndex + SceneManager.sceneCount; ++i)
        {
            scenes.Add(SceneManager.GetSceneAt(i));
        }
    }

    public void Load(string s)
    {
        SceneManager.LoadScene(s);
    }

    public void Load(int i)
    {
        SceneManager.LoadScene(i);
    }

    public static void LoadScene(string s)
    {
        SceneManager.LoadScene(s);
    }

    public static void LoadScene(int i)
    {
        SceneManager.LoadScene(i);
    }

    public static void LoadSceneAdd(int i)
    {
        SceneManager.LoadScene(i, LoadSceneMode.Additive);
    }

    public void ReloadCurrentScene()
    {
        var cur = SceneManager.GetActiveScene();
        foreach (var s in scenes)
        {
            if (s == cur)
                SceneManager.LoadScene(s.buildIndex);
            else
                SceneManager.LoadScene(s.buildIndex, LoadSceneMode.Additive);
        }
    }

    public static int GetCurrentSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

    public static void Test()
    {
        //Debug.Log("Button Works");
    }

    public static void LoadVAI()
    {
        GameManager.PVP = false;
        SceneManager.LoadScene("MainGameLoop");
    }

    public static void LoadPVP()
    {
        GameManager.PVP = true;
        SceneManager.LoadScene("MainGameLoop");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}