#if UNITY_EDITOR
using UnityEditor;
using FMODUnity;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(EventCommandSender))]
public class EventCommandSenderEditor : Editor
{
    private EventCommandSender eventCommandSender;

    [SerializeField] private StyleSheet boxGroupStyle;

    private void OnEnable()
    {
        // Event Command Sender
        string darkIconGuid = AssetDatabase.GUIDToAssetPath("a8a48b57aa73c48918267b3bd2c62afa");
        Texture2D darkIcon =
            AssetDatabase.LoadAssetAtPath<Texture2D>(darkIconGuid);

        string whiteIconGuid = AssetDatabase.GUIDToAssetPath("3f4aee5606d0a488c9660e2fb896a0fd");
        Texture2D whiteIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(whiteIconGuid);

        string path = "Assets/Plugins/FMODPlus/Runtime/EventCommandSender.cs";
        MonoScript studioListener = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
        FMODIconEditor.ApplyIcon(darkIcon, whiteIcon, studioListener);

        if (!boxGroupStyle)
            boxGroupStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Plugins/FMODPlus/Editor/ButtonStyle.uss");
    }

    public override VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();

        eventCommandSender = (EventCommandSender)target;
        root.styleSheets.Add(boxGroupStyle);

        var root0 = new VisualElement();
        root0.AddToClassList("GroupBoxStyle");
        var behaviourField = new PropertyField(serializedObject.FindProperty("BehaviourStyle"));
        var audioSourceField = new PropertyField(serializedObject.FindProperty("Source"));

        var root1 = new VisualElement();
        root1.AddToClassList("GroupBoxStyle");
        var clipField = new PropertyField(serializedObject.FindProperty("Clip"));
        var clipStyleField = new PropertyField(serializedObject.FindProperty("ClipStyle"));
        var keyField = new PropertyField(serializedObject.FindProperty("Key"));

        var fadeField = new PropertyField(serializedObject.FindProperty("Fade"));
        var sendOnStart = new PropertyField(serializedObject.FindProperty("SendOnStart"));

        var root2 = new VisualElement();
        var onPlaySend = new PropertyField(serializedObject.FindProperty("OnPlaySend"));
        var onStopSend = new PropertyField(serializedObject.FindProperty("OnStopSend"));

        string appSystemLanguage = Application.systemLanguage == SystemLanguage.Korean
            ? "Fade 기능은 AHDSR 묘듈이 추가되어 있어야 동작합니다."
            : "Fade function requires AHDSR module to work.";

        var fadeHelpBox = new HelpBox(appSystemLanguage, HelpBoxMessageType.Info);

        root.Add(root0);
        root.Add(Space(5));
        root0.Add(behaviourField);
        root0.Add(audioSourceField);

        root.Add(root1);
        root1.Add(clipStyleField);
        root1.Add(clipField);
        root1.Add(keyField);
        root1.Add(fadeField);
        root1.Add(sendOnStart);
        root1.Add(fadeHelpBox);

        // Include root3 in root.
        var eventSpace = Space(5f);
        root2.Add(eventSpace);
        root2.Add(onPlaySend);
        root2.Add(onStopSend);
        root.Add(root2);

        // Init
        var visualElements = new[]
        {
            audioSourceField, clipStyleField, clipField, keyField, fadeField, onPlaySend, onStopSend, eventSpace,
            fadeHelpBox
        };

        ControlField(visualElements);

        clipStyleField.RegisterValueChangeCallback(_ => ControlField(visualElements));
        behaviourField.RegisterValueChangeCallback(_ => ControlField(visualElements));
        fadeField.RegisterValueChangeCallback(_ => ControlField(visualElements));
        return root;
    }

    private void ControlField(VisualElement[] elements)
    {
        var audioSourceField = elements[0];
        var clipStyleField = elements[1];
        var clipField = elements[2];
        var keyField = elements[3];
        var fadeField = elements[4];
        var onPlaySend = elements[5];
        var onStopSend = elements[6];
        var eventSpace = elements[7];
        var helpBox = elements[8];

        // 일단 전부 비활성화
        foreach (var visualElement in elements)
            SetActiveField(visualElement, false);

        if (eventCommandSender.BehaviourStyle == AudioBehaviourStyle.Play)
        {
            SetActiveField(audioSourceField, true);
            SetActiveField(clipField, true);
        }
        else if (eventCommandSender.BehaviourStyle == AudioBehaviourStyle.PlayOnAPI)
        {
            SetActiveField(clipStyleField, true);
            SetActiveField(clipField, true);
            SetActiveField(keyField, true);
            SetActiveField(onPlaySend, true);
            SetActiveField(eventSpace, true);
            ControlClipStyleField(clipField, keyField);
        }
        else if (eventCommandSender.BehaviourStyle == AudioBehaviourStyle.Stop)
        {
            SetActiveField(audioSourceField, true);
            SetActiveField(fadeField, true);
            ControlFadeHelpBoxField(helpBox);
        }
        else // if (eventCommandSender.BehaviourStyle == AudioBehaviourStyle.StopOnAPI)
        {
            SetActiveField(fadeField, true);
            SetActiveField(onStopSend, true);
            SetActiveField(eventSpace, true);
            ControlFadeHelpBoxField(helpBox);
        }
    }

    private void ControlFadeHelpBoxField(VisualElement fadeHelpBox)
    {
        if (eventCommandSender.Fade)
            SetActiveField(fadeHelpBox, true);
    }

    private void ControlClipStyleField(VisualElement clipField, VisualElement keyField)
    {
        if (eventCommandSender.ClipStyle == ClipStyle.EventReference)
        {
            SetActiveField(clipField, true);
            SetActiveField(keyField, false);
        }
        else
        {
            SetActiveField(clipField, false);
            SetActiveField(keyField, true);
        }
    }

    private void SetActiveField(VisualElement field, bool active)
    {
        field.style.display = active ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private VisualElement Space(float height)
    {
        var space = new VisualElement();
        space.style.height = height;
        // Debug : space.style.backgroundColor = new StyleColor(Color.red);
        return space;
    }
}
#endif