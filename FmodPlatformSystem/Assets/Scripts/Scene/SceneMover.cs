using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMover : MonoBehaviour
{
    public SceneReference NextScene;

    public void MoveScene()
    {
        SceneManager.LoadScene(NextScene.Path);
    }
}
