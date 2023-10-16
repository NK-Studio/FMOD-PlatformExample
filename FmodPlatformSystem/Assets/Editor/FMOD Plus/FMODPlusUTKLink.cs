using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

// EventRefAtKey를 인스펙터에서 그려내기 위해선 UI Toolkit 기반으로 인스펙터를 그려내는 환경이 필요합니다.
// 안타깝게도 유니티 2022.3 LTS까지도 유니티의 기본은 IMGUI로 인스펙터를 그려내기 때문에,
// 일반적인 에디터 환경에서 UI Toolkit의 PropertyDrawer를 사용하는 경우 'No GUI implemented'를 표시합니다.
// 다음 코드에서는 기본적으로 ScriptableObject와 MonoBehaviour를 인스펙터에서 그려내는 방법을 보여줍니다.
// 만약, 커스텀 에디터를 그려야하는 상황일 경우 UI Toolkit 방식을 사용하여 커스텀 에디터를 그려내십시오.
namespace NKStudio
{
    [CustomEditor(typeof(KeyListSO), true)]
    public class FMODPlusSOLink : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();
            
            InspectorElement.FillDefaultInspector(container, serializedObject, this);

            return container;
        }
    }

    [CustomEditor(typeof(KeyListMono), true)]
    public class FMODPlusMonoLink : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();
            
            InspectorElement.FillDefaultInspector(container, serializedObject, this);

            return container;
        }
    }
}
