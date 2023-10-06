using UnityEditor;
using UnityToolbarExtender;

namespace NKStudio
{
    [InitializeOnLoad]
    public class ToolbarRegister
    {
        static ToolbarRegister()
        {
            // LeftToolbarGUI는 왼쪽부터 렌더링됩니다.
            ToolbarExtender.LeftToolbarGUI.Add(EnterPlayModeOptionToolbars.OnToolbarGUI);
#if USE_FMOD
            ToolbarExtender.LeftToolbarGUI.Add(FMODDebugToolbars.OnToolbarGUI);
#endif
            ToolbarExtender.LeftToolbarGUI.Add(SceneSwitchLeftButton.OnToolbarGUI);
            
            // RightToolbarGUI는 오른쪽부터 렌더링됩니다.
            ToolbarExtender.RightToolbarGUI.Add(SceneSwitchRightButton.OnToolbarGUI);
        }
    }
}