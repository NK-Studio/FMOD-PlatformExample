using GameplayIngredients;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Setting : MonoBehaviour
{
    public void gotoDemo3()
    {
        //현재 재생중인 BGM을 릴리즈 합니다.
        Manager.Get<AudioManager>().notifyRelease();
        
        //데모 3로 이동합니다.
        SceneManager.LoadScene("Demo3");
    } 
       
}
