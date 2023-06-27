using UnityEditor;
using FMODUnity;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(AudioCommandSender))]
public class AudioCommandSenderEditor : Editor
{
    private AudioCommandSender audioCommandSender;

    public Texture2D DarkIcon;
    public Texture2D WhiteIcon;

    public override VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();

        audioCommandSender = (AudioCommandSender)target;

        var audioSourceField = new PropertyField(serializedObject.FindProperty("Source"));
        var clipField = new PropertyField(serializedObject.FindProperty("Clip"));
        var keyField = new PropertyField(serializedObject.FindProperty("Key")); // DeleteTarget
        var behaviourField = new PropertyField(serializedObject.FindProperty("BehaviourStyle"));
        var fadeField = new PropertyField(serializedObject.FindProperty("Fade"));
        var sendOnStart = new PropertyField(serializedObject.FindProperty("SendOnStart"));

        string appSystemLanguage = Application.systemLanguage == SystemLanguage.Korean
            ? "Fade 기능은 AHDSR 묘듈이 추가되어 있어야 동작합니다."
            : "Fade function requires AHDSR module to work.";

        var fadeHelpBox = new HelpBox(appSystemLanguage, HelpBoxMessageType.Info);

        root.Add(audioSourceField);
        root.Add(clipField);
        root.Add(keyField); // DeleteTarget
        root.Add(behaviourField);
        root.Add(fadeField);
        root.Add(sendOnStart);
        root.Add(fadeHelpBox);

        // Init
        ControlField(audioSourceField, keyField /* Delete Target*/, clipField, fadeField, fadeHelpBox);

        behaviourField.RegisterValueChangeCallback(_ =>
        {
            ControlFadeHelpBoxField(fadeHelpBox);
            ControlField(audioSourceField, keyField /* Delete Target*/, clipField, fadeField, fadeHelpBox);
        });

        fadeField.RegisterValueChangeCallback(_ => ControlFadeHelpBoxField(fadeHelpBox));
        return root;
    }

    private void ControlField(VisualElement audioSourceField, VisualElement keyField, VisualElement clipField,
        VisualElement fadeField,
        VisualElement fadeHelpBox)
    {
        if (audioCommandSender.BehaviourStyle == AudioBehaviourStyle.Play)
        {
            SetActiveField(audioSourceField, true);
            SetActiveField(keyField, false); // Delete Target
            SetActiveField(clipField, true);
            SetActiveField(fadeField, false);
            SetActiveField(fadeHelpBox, false);
        }
        // Delete Target
        else if (audioCommandSender.BehaviourStyle == AudioBehaviourStyle.PlayOnAPI)
        {
            SetActiveField(audioSourceField, false);
            SetActiveField(keyField, true);
            SetActiveField(clipField, false);
            SetActiveField(fadeField, false);
            SetActiveField(fadeHelpBox, false);
        }
        else if (audioCommandSender.BehaviourStyle == AudioBehaviourStyle.Stop)
        {
            SetActiveField(audioSourceField, true);
            SetActiveField(keyField, false);
            SetActiveField(clipField, false);
            SetActiveField(fadeField, true);
        }
        // Delete Target
        else
        {
            SetActiveField(audioSourceField, false);
            SetActiveField(keyField, false);
            SetActiveField(clipField, false);
            SetActiveField(fadeField, true);
        }
    }

    private void ControlFadeHelpBoxField(VisualElement fadeHelpBox)
    {
        if (audioCommandSender.Fade)
            SetActiveField(fadeHelpBox, true);
        else
            SetActiveField(fadeHelpBox, false);
    }

    private void SetActiveField(VisualElement field, bool active)
    {
        field.style.display = active ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private void OnEnable()
    {
        InitIcon();
    }

    private void InitIcon()
    {
        if (!DarkIcon || !WhiteIcon)
        {
            Debug.LogWarning("No Binding Icon");
            EditorGUIUtility.SetIconForObject(target, null);
            return;
        }

        bool isDarkMode = EditorGUIUtility.isProSkin;

        if (isDarkMode)
        {
            if (DarkIcon)
                EditorGUIUtility.SetIconForObject(target, DarkIcon);
        }
        else
        {
            if (WhiteIcon)
                EditorGUIUtility.SetIconForObject(target, WhiteIcon);
        }
    }
}