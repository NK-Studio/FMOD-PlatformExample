using System;
using System.Reflection;
using BehaviorDesigner.Editor;
using BehaviorDesigner.Runtime;
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
                
                var rect = EditorGUILayout.GetControlRect();
                rect.xMin += EditorGUIUtility.labelWidth;
                GUI.Label(rect ,NotFoundWarning);

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                
                fmodPlay.Style = (PathStyle)FieldInspector.DrawField(fmodPlay,
                    new GUIContent("Style", "경로"),
                    fmodPlay.GetType().GetField("Style", BindingFlags.Instance | BindingFlags.Public),
                    fmodPlay.Style);

                switch (fmodPlay.Style)
                {
                    case PathStyle.EventReference:
                        fmodPlay.Path = (string)FieldInspector.DrawField(fmodPlay,
                            new GUIContent("Path", "경로"),
                            fmodPlay.GetType().GetField("Path", BindingFlags.Instance | BindingFlags.Public),
                            fmodPlay.Path);
                        break;
                    case PathStyle.Key:
                        fmodPlay.UseGlobalKeyList = (bool)FieldInspector.DrawField(fmodPlay,
                            new GUIContent("Use Global KeyList", "전역 키 리스트를 사용하는가?"),
                            fmodPlay.GetType().GetField("UseGlobalKeyList", BindingFlags.Instance | BindingFlags.Public),
                            fmodPlay.UseGlobalKeyList);

                        if (fmodPlay.UseGlobalKeyList)
                        {
                            fmodPlay.AudioType = (AudioType)FieldInspector.DrawField(fmodPlay,
                                new GUIContent("Audio Type", "오디오 타입"),
                                fmodPlay.GetType().GetField("AudioType", BindingFlags.Instance | BindingFlags.Public),
                                fmodPlay.AudioType);
                        }
                        else
                        {
                            fmodPlay.LocalKeyList = (SharedLocalKeyList)FieldInspector.DrawSharedVariable(fmodPlay,
                                new GUIContent("Local Key List"),
                                fmodPlay.GetType().GetField("LocalKeyList"),
                                typeof(SharedLocalKeyList),
                                fmodPlay.LocalKeyList);
                        }

                        fmodPlay.KeyName = (string)FieldInspector.DrawField(fmodPlay,
                            new GUIContent("Key", "List key to use"),
                            fmodPlay.GetType().GetField("KeyName", BindingFlags.Instance | BindingFlags.Public),
                            fmodPlay.KeyName);
                        
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

    }
}
