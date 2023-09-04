using System.Reflection;
using BehaviorDesigner.Editor;
using UnityEditor;
using UnityEngine;

namespace NKStudio.FMODPlus.BehaviorDesigner
{
    [CustomObjectDrawer(typeof(FMODStop))]
    public class FMODStopEditor : ObjectDrawer
    {
        public override void OnGUI(GUIContent label)
        {
            GUIStyle boldLabelStyle = new(EditorStyles.label);
            boldLabelStyle.fontStyle = FontStyle.Bold;

            if (task is FMODStop fmodStop)
            {
                EditorGUILayout.LabelField("Setting", boldLabelStyle);

                fmodStop.FMODAudioSource = (SharedFMODAudioSource)FieldInspector.DrawSharedVariable(fmodStop,
                    new GUIContent("FMOD Audio Source"),
                    fmodStop.GetType().GetField("FMODAudioSource"),
                    typeof(SharedFMODAudioSource),
                    fmodStop.FMODAudioSource);

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                string msg = Application.systemLanguage == SystemLanguage.Korean
                    ? "AHDSR묘듈을 사용하여 사운드를 페이드 아웃합니다."
                    : "Fade out the sound using the AHDSR module.";

                fmodStop.Fade = (bool)FieldInspector.DrawField(fmodStop,
                    new GUIContent("Fade", msg),
                    fmodStop.GetType().GetField("Fade", BindingFlags.Instance | BindingFlags.Public),
                    fmodStop.Fade);

                if (fmodStop.Fade)
                    HandleMessage("Fade 기능은 AHDSR 묘듈이 추가되어 있어야 동작합니다.", 
                        "Fade function requires AHDSR module to work.");
            }
        }

        /// <summary>
        /// Displays a message in the HelpBox.
        /// </summary>
        /// <param name="koreanMessage">Korean message</param>
        /// <param name="englishMessage">ENGLISH MESSAGE</param>
        private void HandleMessage(string koreanMessage, string englishMessage)
        {
            string msg = Application.systemLanguage == SystemLanguage.Korean ? koreanMessage : englishMessage;
            EditorGUILayout.HelpBox(msg, MessageType.Info);
        }
    }
}