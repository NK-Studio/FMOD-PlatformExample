using System;
using System.Collections.Generic;
using System.Reflection;
using BehaviorDesigner.Editor;
using FMODPlus;
using FMODUnity;
using UnityEditor;
using UnityEngine;
using AudioType = FMODPlus.AudioType;

namespace NKStudio.FMODPlus.BehaviorDesigner
{
    [CustomObjectDrawer(typeof(FMODPlay))]
    public class FMODPlayEditor : ObjectDrawer
    {
        private static readonly Texture WarningIcon = EditorUtils.LoadImage("NotFound.png");
        private static readonly GUIContent NotFoundWarning = new GUIContent("Event Not Found", WarningIcon);

        public override void OnGUI(GUIContent label)
        {
            GUIStyle boldLabelStyle = new(EditorStyles.label);
            boldLabelStyle.fontStyle = FontStyle.Bold;

            if (task is FMODPlay fmodPlay)
            {
                EditorGUILayout.LabelField("Setting", boldLabelStyle);
                fmodPlay.FMODAudioSource = (SharedFMODAudioSource)FieldInspector.DrawSharedVariable(fmodPlay,
                    new GUIContent("FMOD Audio Source"),
                    fmodPlay.GetType().GetField("FMODAudioSource"),
                    typeof(SharedFMODAudioSource),
                    fmodPlay.FMODAudioSource);

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                fmodPlay.Style = (PathStyle)FieldInspector.DrawField(fmodPlay,
                    new GUIContent("Style", "경로"),
                    fmodPlay.GetType().GetField("Style", BindingFlags.Instance | BindingFlags.Public),
                    fmodPlay.Style);

                switch (fmodPlay.Style)
                {
                    case PathStyle.EventReference:
                        fmodPlay.Path = (string)FieldInspector.DrawField(fmodPlay,
                            new GUIContent("Event", "EventReference to play sound"),
                            fmodPlay.GetType().GetField("Path", BindingFlags.Instance | BindingFlags.Public),
                            fmodPlay.Path);

                        bool isNotEmptyPath = !string.IsNullOrEmpty(fmodPlay.Path);

                        if (isNotEmptyPath)
                        {
                            EventReference clip = EventReference.Find(fmodPlay.Path);

                            if (FMODEditorUtility.IsNull(clip))
                            {
                                var rect = EditorGUILayout.GetControlRect();
                                rect.xMin += EditorGUIUtility.labelWidth;
                                GUI.Label(rect, NotFoundWarning);
                            }
                        }
                        else
                        {
                            var rect = EditorGUILayout.GetControlRect();
                            rect.xMin += EditorGUIUtility.labelWidth;
                            GUI.Label(rect, NotFoundWarning);
                        }

                        break;
                    case PathStyle.Key:
                        fmodPlay.UseGlobalKeyList = (bool)FieldInspector.DrawField(fmodPlay,
                            new GUIContent("Use Global KeyList", "전역 키 리스트를 사용하는가?"),
                            fmodPlay.GetType().GetField("UseGlobalKeyList",
                                BindingFlags.Instance | BindingFlags.Public),
                            fmodPlay.UseGlobalKeyList);

                        bool isEditable = true;

                        if (fmodPlay.UseGlobalKeyList)
                        {
                            fmodPlay.AudioType = (AudioType)FieldInspector.DrawField(fmodPlay,
                                new GUIContent("Audio Type", "오디오 타입"),
                                fmodPlay.GetType().GetField("AudioType", BindingFlags.Instance | BindingFlags.Public),
                                fmodPlay.AudioType);

                            fmodPlay.Key = (string)FieldInspector.DrawField(fmodPlay,
                                new GUIContent("Key"),
                                fmodPlay.GetType().GetField("Key", BindingFlags.Instance | BindingFlags.Public),
                                fmodPlay.Key);

                            bool keyExists = !string.IsNullOrWhiteSpace(fmodPlay.Key);
                            if (keyExists)
                            {
                                EditorEventRef existEvent;

                                switch (fmodPlay.AudioType)
                                {
                                    case AudioType.AMB:
                                        existEvent = AMBKeyList.Instance.GetEventRef(fmodPlay.Key);
                                        break;
                                    case AudioType.BGM:
                                        existEvent = BGMKeyList.Instance.GetEventRef(fmodPlay.Key);
                                        break;
                                    case AudioType.SFX:
                                        existEvent = SFXKeyList.Instance.GetEventRef(fmodPlay.Key);
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }

                                if (existEvent == null)
                                {
                                    HandleMessage(
                                        "연결된 이벤트 주소가 유효하지 않습니다.",
                                        "The connected event address is invalid.");
                                }
                            }
                            else
                            {
                                HandleMessage(
                                    "Key가 비어있습니다.",
                                    "Key is empty.");
                            }
                        }
                        else
                        {
                            fmodPlay.LocalKeyList = (SharedLocalKeyList)FieldInspector.DrawSharedVariable(fmodPlay,
                                new GUIContent("Local Key List"),
                                fmodPlay.GetType().GetField("LocalKeyList"),
                                typeof(SharedLocalKeyList),
                                fmodPlay.LocalKeyList);

                            if (fmodPlay.LocalKeyList.Value == null)
                                isEditable = false;

                            GUI.enabled = isEditable;

                            string emptyLocalKeyList = Application.systemLanguage == SystemLanguage.Korean
                                ? "로컬 키 리스트가 연결되어 있지 않습니다."
                                : "Local key list is null.";

                            fmodPlay.Key = (string)FieldInspector.DrawField(fmodPlay,
                                new GUIContent("Key", fmodPlay.LocalKeyList.Value == null ? emptyLocalKeyList : null),
                                fmodPlay.GetType().GetField("Key", BindingFlags.Instance | BindingFlags.Public),
                                fmodPlay.Key);

                            GUI.enabled = true;

                            if (isEditable)
                            {
                                bool keyExists = !string.IsNullOrWhiteSpace(fmodPlay.Key);
                                if (keyExists)
                                {
                                    EditorEventRef existEvent = null;

                                    LocalKeyList targetKeyList = fmodPlay.LocalKeyList.Value;
                                    List<EventReferenceByKey> lists = targetKeyList.Clips.GetList();

                                    foreach (EventReferenceByKey list in lists)
                                        if (list.Key == fmodPlay.Key)
                                        {
                                            existEvent = EventManager.EventFromPath(list.Value.Path);
                                            break;
                                        }

                                    if (existEvent == null)
                                    {
                                        HandleMessage(
                                            "연결된 이벤트 주소가 유효하지 않습니다.",
                                            "The connected event address is invalid.");
                                    }
                                }
                                else
                                {
                                    HandleMessage(
                                        "Key가 비어있습니다.",
                                        "Key is empty.");
                                }
                            }
                        }

                        break;
                }
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
            EditorGUILayout.HelpBox(msg, MessageType.Error);
        }
    }
}