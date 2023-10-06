using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;

namespace NKStudio
{
    public class EnterPlayModeOptionToolbars
    {
        public static void OnToolbarGUI()
        {
            // 현재 Enter Player Mode Option 상태
            bool currentPlayModeOption = EditorSettings.enterPlayModeOptionsEnabled;

            // 왼쪽 마진을 3정도 적용함.
            GUILayout.Space(2);
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
            // GUI 렌더링
            EditorGUILayout.LabelField("Enter Play Mode", GUILayout.Width(96));
            EditorSettings.enterPlayModeOptionsEnabled =
                EditorGUILayout.Toggle(currentPlayModeOption, GUILayout.Width(16));
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
        }
    }
}