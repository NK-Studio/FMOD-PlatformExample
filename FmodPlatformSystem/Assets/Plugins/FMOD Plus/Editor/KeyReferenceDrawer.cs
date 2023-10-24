using System;
using System.Linq;
using FMODUnity;
using UnityEditor;
using UnityEngine;
namespace FMODPlus
{
    [CustomPropertyDrawer(typeof(KeyReference))]
    public class KeyReferenceDrawer : PropertyDrawer
    {
        private static readonly Texture RepairIcon = EditorUtils.LoadImage("Wrench.png");
        private static readonly Texture WarningIcon = EditorUtils.LoadImage("NotFound.png");
        private static readonly GUIContent NotFoundWarning = new GUIContent("Event Not Found", WarningIcon);

        private static GUIStyle buttonStyle;

        private static Vector2 WarningSize()
        {
            return GUI.skin.label.CalcSize(NotFoundWarning);
        }

        private static float GetBaseHeight()
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (buttonStyle == null)
            {
                buttonStyle = new GUIStyle(GUI.skin.button);
                buttonStyle.padding.top = 1;
                buttonStyle.padding.bottom = 1;
            }

            SerializedProperty UseGlobalKeyListProperty = GetUseGlobalKeyListProperty(property);
            SerializedProperty KeyListProperty = GetkeyListProperty(property);
            SerializedProperty keyProperty = GetKeyProperty(property);
            SerializedProperty audioStyleProperty = GetAudioStyleProperty(property);
            SerializedProperty GUIDProperty = GetGUIDProperty(property);
            SerializedProperty pathProperty = GetPathProperty(property);

            Texture copyIcon = EditorUtils.LoadImage("CopyIcon.png");

            using (new EditorGUI.PropertyScope(position, label, property))
            {
                HandleDragEvents(position, property);

                EventReference eventReference = EventReference.Find(pathProperty.stringValue);
                EditorEventRef editorEventRef = GetEditorEventRef(eventReference);

                float baseHeight = GetBaseHeight();
                position.height = baseHeight;

                using (var scope = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUI.PropertyField(position, UseGlobalKeyListProperty);

                    if (scope.changed)
                    {
                        GUIDProperty.SetGuid(new FMOD.GUID());
                        pathProperty.stringValue = string.Empty;
                        keyProperty.stringValue = string.Empty;
                        property.isExpanded = false;
                    }
                }

                position.y += baseHeight;
                position.height = baseHeight;

                if (UseGlobalKeyListProperty.boolValue)
                {
                    using (var scope = new EditorGUI.ChangeCheckScope())
                    {
                        EditorGUI.PropertyField(position, audioStyleProperty);
                        if (scope.changed)
                        {
                            pathProperty.stringValue = string.Empty;
                            GUIDProperty.SetGuid(new FMOD.GUID());
                            
                            var audioStyle = (AudioType)audioStyleProperty.enumValueIndex;
                            switch (audioStyle)
                            {
                                case AudioType.AMB:
                                    editorEventRef = AMBKeyList.Instance.GetEventRef(keyProperty.stringValue);
                                    break;
                                case AudioType.BGM:
                                    editorEventRef = BGMKeyList.Instance.GetEventRef(keyProperty.stringValue);
                                    break;
                                case AudioType.SFX:
                                    editorEventRef = SFXKeyList.Instance.GetEventRef(keyProperty.stringValue);
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }

                            pathProperty.stringValue = editorEventRef == null ? string.Empty : editorEventRef.Path;
                            SetEvent(property, pathProperty.stringValue);
                        }
                    }
                }
                else
                {
                    EditorGUI.PropertyField(position, KeyListProperty);
                }

                position.y += baseHeight + 3;
                Rect headerRect = position;
                headerRect.width = EditorGUIUtility.labelWidth;
                headerRect.height = baseHeight;

                property.isExpanded = EditorGUI.Foldout(headerRect, property.isExpanded, "Key", true);

                Rect keyRect = position;
                keyRect.xMin = headerRect.xMax;
                keyRect.height = baseHeight;

                using (var scope = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUI.PropertyField(keyRect, keyProperty, GUIContent.none);

                    if (scope.changed)
                    {
                        if (UseGlobalKeyListProperty.boolValue)
                        {
                            var audioStyle = (AudioType)audioStyleProperty.enumValueIndex;
                            switch (audioStyle)
                            {
                                case AudioType.AMB:
                                    editorEventRef = AMBKeyList.Instance.GetEventRef(keyProperty.stringValue);
                                    break;
                                case AudioType.BGM:
                                    editorEventRef = BGMKeyList.Instance.GetEventRef(keyProperty.stringValue);
                                    break;
                                case AudioType.SFX:
                                    editorEventRef = SFXKeyList.Instance.GetEventRef(keyProperty.stringValue);
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }

                        }
                        else
                        {
                            LocalKeyList targetKeyList =
                                (LocalKeyList)KeyListProperty.objectReferenceValue;
                            SerializedObject targetLocalKeyList = new(targetKeyList);
                            SerializedProperty lists = targetLocalKeyList
                                .FindProperty("ClipList")
                                .FindPropertyRelative("EventRefList");

                            // 키 리스트에 내가 작성한 키가 존재하는지 체크한다.
                            // 만약 존재한다면, 해당 키의 패스를 가져온다.
                            foreach (SerializedProperty list in lists)
                            {
                                string targetKey = list.FindPropertyRelative("Key")
                                    .stringValue;

                                string targetPath = list.FindPropertyRelative("Value")
                                    .FindPropertyRelative("Path")
                                    .stringValue;

                                if (keyProperty.stringValue == targetKey)
                                {
                                    editorEventRef = EventManager.EventFromPath(targetPath);
                                    break;
                                }
                            }
                        }

                        pathProperty.stringValue = editorEventRef == null ? string.Empty : editorEventRef.Path;
                        SetEvent(property, pathProperty.stringValue);
                    }
                }

                if (editorEventRef != null)
                {
                    float labelY = headerRect.y + baseHeight;
                    MismatchInfo mismatch = GetMismatch(eventReference, editorEventRef);

                    if (mismatch != null)
                    {
                        Rect warningRect = keyRect;
                        warningRect.xMax = position.xMax;
                        warningRect.y = labelY;
                        warningRect.height = WarningSize().y;

                        DrawMismatchUI(warningRect, keyRect.x, keyRect.width, mismatch, property);

                        labelY = warningRect.yMax;
                    }

                    if (property.isExpanded)
                    {
                        using (new EditorGUI.IndentLevelScope())
                        {
                            Rect labelRect = EditorGUI.IndentedRect(headerRect);
                            labelRect.y = labelY;

                            Rect valueRect = labelRect;
                            valueRect.xMin = labelRect.xMax;
                            valueRect.xMax = position.xMax - copyIcon.width - 7;

                            GUI.Label(labelRect, new GUIContent("Path"));
                            GUI.Label(valueRect, eventReference.Path);

                            labelRect.y += baseHeight;
                            valueRect.y += baseHeight;

                            GUI.Label(labelRect, new GUIContent("GUID"));
                            GUI.Label(valueRect, eventReference.Guid.ToString());

                            Rect copyRect = valueRect;
                            copyRect.xMin = valueRect.xMax;
                            copyRect.xMax = position.xMax;

                            if (GUI.Button(copyRect, new GUIContent(copyIcon, "Copy To Clipboard")))
                            {
                                EditorGUIUtility.systemCopyBuffer = eventReference.Guid.ToString();
                            }

                            valueRect.xMax = position.xMax;

                            labelRect.y += baseHeight;
                            valueRect.y += baseHeight;

                            GUI.Label(labelRect, new GUIContent("Banks"));
                            GUI.Label(valueRect, string.Join(", ", editorEventRef.Banks.Select(x => x.Name).ToArray()));
                            labelRect.y += baseHeight;
                            valueRect.y += baseHeight;

                            GUI.Label(labelRect, new GUIContent("Panning"));
                            GUI.Label(valueRect, editorEventRef.Is3D ? "3D" : "2D");
                            labelRect.y += baseHeight;
                            valueRect.y += baseHeight;

                            GUI.Label(labelRect, new GUIContent("Stream"));
                            GUI.Label(valueRect, editorEventRef.IsStream.ToString());
                            labelRect.y += baseHeight;
                            valueRect.y += baseHeight;

                            GUI.Label(labelRect, new GUIContent("Oneshot"));
                            GUI.Label(valueRect, editorEventRef.IsOneShot.ToString());
                            labelRect.y += baseHeight;
                            valueRect.y += baseHeight;
                        }
                    }
                }
                else
                {
                    EditorEventRef renamedEvent = GetRenamedEventRef(eventReference);

                    if (renamedEvent != null)
                    {
                        MismatchInfo mismatch = new MismatchInfo() {
                            Message = $"Moved to {renamedEvent.Path}",
                            HelpText = "This event has been moved in FMOD Studio.\n" + "You can click the repair button to update the path to the new location, or run " + $"the <b>{EventReferenceUpdater.MenuPath}</b> command to scan your project for similar issues and fix them all.",
                            RepairTooltip = $"Repair: set path to {renamedEvent.Path}",
                            RepairAction = (p) => {
                                p.FindPropertyRelative("Path").stringValue = renamedEvent.Path;
                            },
                        };

                        using (new EditorGUI.IndentLevelScope())
                        {
                            Rect mismatchRect = keyRect;

                            mismatchRect.xMin = position.xMin;
                            mismatchRect.xMax = position.xMax;
                            mismatchRect.y += baseHeight;
                            mismatchRect.height = baseHeight;

                            mismatchRect = EditorGUI.IndentedRect(mismatchRect);

                            DrawMismatchUI(mismatchRect, keyRect.x, keyRect.width, mismatch, property);
                        }
                    }
                    else
                    {
                        Rect labelRect = keyRect;
                        labelRect.xMax = position.xMax;
                        labelRect.y += baseHeight + 3;
                        labelRect.height = WarningSize().y;

                        GUI.Label(labelRect, NotFoundWarning);
                    }
                }
            }
        }

        private static void HandleDragEvents(Rect position, SerializedProperty property)
        {
            Event e = Event.current;

            if (e.type == EventType.DragPerform && position.Contains(e.mousePosition))
            {
                if (DragAndDrop.objectReferences.Length > 0 &&
                    DragAndDrop.objectReferences[0] != null &&
                    DragAndDrop.objectReferences[0].GetType() == typeof(EditorEventRef))
                {
                    EditorEventRef eventRef = DragAndDrop.objectReferences[0] as EditorEventRef;

                    property.SetEventReference(eventRef.Guid, eventRef.Path);

                    GUI.changed = true;
                    e.Use();
                }
            }

            if (e.type == EventType.DragUpdated && position.Contains(e.mousePosition))
            {
                if (DragAndDrop.objectReferences.Length > 0 &&
                    DragAndDrop.objectReferences[0] != null &&
                    DragAndDrop.objectReferences[0].GetType() == typeof(EditorEventRef))
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                    DragAndDrop.AcceptDrag();
                    e.Use();
                }
            }
        }

        private class MismatchInfo
        {
            public string Message;
            public string HelpText;
            public string RepairTooltip;
            public Action<SerializedProperty> RepairAction;
        }

        private static void DrawMismatchUI(Rect rect, float repairButtonX, float repairButtonWidth,
            MismatchInfo mismatch, SerializedProperty property)
        {
            rect = EditorUtils.DrawHelpButton(rect, () => new SimpleHelp(mismatch.HelpText, 400));

            Rect repairRect = new Rect(repairButtonX, rect.y, repairButtonWidth, GetBaseHeight());

            if (GUI.Button(repairRect, new GUIContent(RepairIcon, mismatch.RepairTooltip), buttonStyle))
            {
                mismatch.RepairAction(property);
            }

            Rect labelRect = rect;
            labelRect.xMax = repairRect.xMin;

            GUI.Label(labelRect, new GUIContent(mismatch.Message, WarningIcon));
        }

        private static MismatchInfo GetMismatch(EventReference eventReference, EditorEventRef editorEventRef)
        {
            if (EventManager.GetEventLinkage(eventReference) == EventLinkage.Path)
            {
                if (eventReference.Guid != editorEventRef.Guid)
                {
                    return new MismatchInfo() {
                        Message = "GUID doesn't match path",
                        HelpText = "The GUID on this EventReference doesn't match the path.\n" + "You can click the repair button to update the GUID to match the path, or run the " + $"<b>{EventReferenceUpdater.MenuPath}</b> command to scan your project for similar issues and fix them all.",
                        RepairTooltip = $"Repair: set GUID to {editorEventRef.Guid}",
                        RepairAction = (property) => {
                            property.FindPropertyRelative("Guid").SetGuid(editorEventRef.Guid);
                        },
                    };
                }
            }
            else // EventLinkage.GUID
            {
                if (eventReference.Path != editorEventRef.Path)
                {
                    return new MismatchInfo() {
                        Message = "Path doesn't match GUID",
                        HelpText = "The path on this EventReference doesn't match the GUID.\n" + "You can click the repair button to update the path to match the GUID, or run the " + $"<b>{EventReferenceUpdater.MenuPath}</b> command to scan your project for similar issues and fix them all.",
                        RepairTooltip = $"Repair: set path to '{editorEventRef.Path}'",
                        RepairAction = (property) => {
                            property.FindPropertyRelative("Path").stringValue = editorEventRef.Path;
                        },
                    };
                }
            }

            return null;
        }

        private static void SetEvent(SerializedProperty property, string path)
        {
            EditorEventRef eventRef = EventManager.EventFromPath(path);

            if (eventRef != null)
            {
                property.SetEventReference(eventRef.Guid, eventRef.Path);
            }
            else
            {
                property.SetEventReference(new FMOD.GUID(), path);
            }
        }

        private static SerializedProperty GetGUIDProperty(SerializedProperty property)
        {
            return property.FindPropertyRelative("Guid");
        }

        private static SerializedProperty GetPathProperty(SerializedProperty property)
        {
            return property.FindPropertyRelative("Path");
        }

        private static SerializedProperty GetKeyProperty(SerializedProperty property)
        {
            return property.FindPropertyRelative("Key");
        }

        private static SerializedProperty GetAudioStyleProperty(SerializedProperty property)
        {
            return property.FindPropertyRelative("audioStyle");
        }

        private static SerializedProperty GetkeyListProperty(SerializedProperty property)
        {
            return property.FindPropertyRelative("keyList");
        }

        private static SerializedProperty GetUseGlobalKeyListProperty(SerializedProperty property)
        {
            return property.FindPropertyRelative("useGlobalKeyList");
        }

        private static EditorEventRef GetEditorEventRef(EventReference eventReference)
        {
            if (EventManager.GetEventLinkage(eventReference) == EventLinkage.Path)
            {
                return EventManager.EventFromPath(eventReference.Path);
            }
            else // Assume EventLinkage.GUID
            {
                return EventManager.EventFromGUID(eventReference.Guid);
            }
        }

        private static EditorEventRef GetRenamedEventRef(EventReference eventReference)
        {
            if (Settings.Instance.EventLinkage == EventLinkage.Path && !eventReference.Guid.IsNull)
            {
                EditorEventRef editorEventRef = EventManager.EventFromGUID(eventReference.Guid);

                if (editorEventRef != null && editorEventRef.Path != eventReference.Path)
                {
                    return editorEventRef;
                }
            }

            return null;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float baseHeight = GetBaseHeight();

            EventReference eventReference = property.GetEventReference();
            EditorEventRef editorEventRef = GetEditorEventRef(eventReference);

            if (editorEventRef == null)
            {
                return baseHeight*3 + WarningSize().y;
            }

            float height;

            if (property.isExpanded)
            {
                height = baseHeight*3 + baseHeight*6; // 5 lines of info
            }
            else
            {
                height = baseHeight*3;
            }

            if (GetMismatch(eventReference, editorEventRef) != null)
            {
                height += WarningSize().y;
            }
            
            height += 3;

            return height;
        }
    }
}
