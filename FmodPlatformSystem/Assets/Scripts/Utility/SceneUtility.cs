using UnityEngine.SceneManagement;

namespace NKStudio.Utility
{
    public static class SceneUtility
    {
        /// <summary>
        /// 현재 씬이 인자와 같은 씬인지 확인합니다.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        public static bool CompareSceneByName(string sceneName)
        {
            return SceneManager.GetActiveScene().name.Equals(sceneName);
        }
    }
}