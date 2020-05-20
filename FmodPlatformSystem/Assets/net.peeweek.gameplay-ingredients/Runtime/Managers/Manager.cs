using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

namespace GameplayIngredients
{
    public abstract class Manager : MonoBehaviour
    {
        private static readonly Dictionary<Type, Manager> s_Managers = new Dictionary<Type, Manager>();

        public static T Get<T>() where T : Manager
        {
            if (s_Managers.ContainsKey(typeof(T)))
                return (T) s_Managers[typeof(T)];
            else
            {
                if (GameplayIngredientsSettings.currentSettings.ShowDebugCustomManager)
                    Debug.LogError($"매니저 : '{typeof(T)}'가 액세스 되지 않았습니다. GameplayIngredientsSettings에서 해당 매니저가 제외됬는지 확인해주세요.");
                return null;
            }
        }

        public static bool Has<T>() where T : Manager
            => s_Managers.ContainsKey(typeof(T));


        private static readonly Type[] kAllManagerTypes = GetAllManagerTypes();

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
#else
        [RuntimeInitializeOnLoadMethod]
#endif
        private static void AutoCreateAll()
        {
            s_Managers.Clear();

            var exclusionList = GameplayIngredientsSettings.currentSettings.excludedeManagers;

            if (GameplayIngredientsSettings.currentSettings.ShowDebugCustomManager)
                Debug.Log("모든 매니저 초기화 중 ...");

            foreach (var type in kAllManagerTypes)
            {
                if (exclusionList != null && exclusionList.ToList().Contains(type.Name))
                {
                    if (GameplayIngredientsSettings.currentSettings.ShowDebugCustomManager)
                        Debug.Log(
                            $"매니저 : {type.Name}가 GameplayIngredientSettings에 excludedeManagers리스트에 있습니다. 생성을 무시합니다.");
                    continue;
                }

                var attrib = type.GetCustomAttribute<ManagerDefaultPrefabAttribute>();
                GameObject gameObject;

                if (attrib != null)
                {
                    var prefab = Resources.Load<GameObject>(attrib.prefab);

                    if (prefab == null) // Try loading the "Default_" prefixed version of the prefab
                        prefab = Resources.Load<GameObject>("Default_" + attrib.prefab);

                    if (prefab != null)
                        gameObject = Instantiate(prefab);
                    else
                    {
                        if (GameplayIngredientsSettings.currentSettings.ShowDebugCustomManager)
                            Debug.LogError(
                                $"{type}에 대한 기본 프리 팹을 인스턴스화 할 수 없습니다. 리소스 폴더에 프리 팹 '{attrib.prefab}'이 (가) 없습니다.");
                        continue;
                    }
                }
                else
                {
                    gameObject = new GameObject();
                    gameObject.AddComponent(type);
                }

                gameObject.name = type.Name;
                DontDestroyOnLoad(gameObject);
                var comp = (Manager) gameObject.GetComponent(type);
                s_Managers.Add(type, comp);

                if (GameplayIngredientsSettings.currentSettings.ShowDebugCustomManager)
                   Debug.Log($" -> {type.Name} 생성 완료");
            }
        }

        private static Type[] GetAllManagerTypes()
        {
            var types = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] assemblyTypes = null;

                try
                {
                    assemblyTypes = assembly.GetTypes();
                }
                catch
                {
                    Debug.LogError($"어셈블리에서 유형을 로드 할 수 없습니다 : {assembly.FullName}");
                }

                if (assemblyTypes != null)
                    types.AddRange(assemblyTypes.Where(t => typeof(Manager).IsAssignableFrom(t) && !t.IsAbstract));
            }

            return types.ToArray();
        }
    }
}
