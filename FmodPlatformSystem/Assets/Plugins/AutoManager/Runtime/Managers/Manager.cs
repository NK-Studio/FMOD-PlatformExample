using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

namespace AutoManager
{
    public abstract class Manager : MonoBehaviour
    {
        private static readonly Dictionary<Type, Manager> Managers = new Dictionary<Type, Manager>();

        public static T Get<T>() where T : Manager
        {
            if (Managers.ContainsKey(typeof(T)))
                return (T)Managers[typeof(T)];

            if (AutoManagerSettings.CurrentSettings.ShowDebugCustomManager)
            {
                string msg = Application.systemLanguage == SystemLanguage.Korean
                    ? $"매니저 : '{typeof(T)}'가 액세스 되지 않았습니다. AutoManagerSettings에서 해당 매니저가 제외됬는지 확인해주세요."
                    : $"Manager : '{typeof(T)}' was not accessed. Check if the manager is excluded from AutoManagerSettings.";
                Debug.LogError(msg);
            }

            return null;
        }

        public static bool Has<T>() where T : Manager
            => Managers.ContainsKey(typeof(T));


        private static readonly Type[] KAllManagerTypes = GetAllManagerTypes();

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
#else
        [RuntimeInitializeOnLoadMethod]
#endif
        private static void AutoCreateAll()
        {
            Managers.Clear();

            string[] exclusionList = AutoManagerSettings.CurrentSettings.ExcludedManagers;

            if (AutoManagerSettings.CurrentSettings.ShowDebugCustomManager)
            {
                string msg = Application.systemLanguage == SystemLanguage.Korean
                    ? "모든 매니저 초기화 중 ..."
                    : "Initializing all managers ...";
                Debug.Log(msg);
            }

            foreach (Type type in KAllManagerTypes)
            {
                if (exclusionList != null && exclusionList.ToList().Contains(type.Name))
                {
                    if (AutoManagerSettings.CurrentSettings.ShowDebugCustomManager)
                    {
                        string msg = Application.systemLanguage == SystemLanguage.Korean
                            ? $"매니저 : {type.Name}가 AutoManagerSettings에 excludedManagers리스트에 있습니다. 생성을 무시합니다."
                            : $"Manager : {type.Name} is in the excludedManagers list in AutoManagerSettings. Ignoring creation.";
                        Debug.Log(msg);
                    }

                    continue;
                }

                var attribute = type.GetCustomAttribute<ManagerDefaultPrefabAttribute>();

                if (attribute != null)
                {
                    GameObject prefab = Resources.Load<GameObject>(attribute.Prefab);

                    if (prefab != null)
                    {
                        GameObject gameObject = Instantiate(prefab);
                        gameObject.name = type.Name;

                        DontDestroyOnLoad(gameObject);
                        Manager comp = (Manager)gameObject.GetComponent(type);
                        Managers.Add(type, comp);

                        if (AutoManagerSettings.CurrentSettings.ShowDebugCustomManager)
                        {
                            string msg = Application.systemLanguage == SystemLanguage.Korean
                                ? $" -> {type.Name} 생성 완료"
                                : $" -> {type.Name} created";
                            Debug.Log(msg);
                        }
                    }
                    else
                    {
                        if (AutoManagerSettings.CurrentSettings.ShowDebugCustomManager)
                        {
                            string msg = Application.systemLanguage == SystemLanguage.Korean
                                ? $"{type}에 대한 기본 프리팹을 인스턴스화 할 수 없습니다. 리소스 폴더에 프리 팹 '{attribute.Prefab}'이(가) 없습니다."
                                : $"Could not instantiate default prefab for {type}. Prefab '{attribute.Prefab}' does not exist in resource folder.";

                            Debug.LogError(msg);
                        }
                    }
                }
            }
        }

        private static Type[] GetAllManagerTypes()
        {
            List<Type> types = new List<Type>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] assemblyTypes = null;

                try
                {
                    assemblyTypes = assembly.GetTypes();
                }
                catch
                {
                    string msg = Application.systemLanguage == SystemLanguage.Korean
                        ? $"어셈블리에서 유형을 로드 할 수 없습니다 : {assembly.FullName}"
                        : $"Could not load types from assembly : {assembly.FullName}";

                    Debug.LogError(msg);
                }

                if (assemblyTypes != null)
                    types.AddRange(assemblyTypes.Where(t => typeof(Manager).IsAssignableFrom(t) && !t.IsAbstract));
            }

            return types.ToArray();
        }
    }
}