using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GameplayIngredients.Editor
{
    partial class WelcomeScreen : EditorWindow
    {
        private int tipIndex;

        private void InitTips() => 
            tipIndex = Random.Range(0, tips.Count);

        private void OnTipsGUI()
        {
            GUILayout.Label("Tip of the Day", EditorStyles.boldLabel);

            using (new GUILayout.VerticalScope(Styles.helpBox))
            {
                var tip = tips[tipIndex];
                GUILayout.Label(tip.Title, Styles.title);
                GUILayout.Space(12);
                GUILayout.Label(tip.Body, Styles.body);
                GUILayout.FlexibleSpace();
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("<<"))
                    {
                        tipIndex--;
                        if (tipIndex < 0)
                            tipIndex = tips.Count - 1;
                    }
                    if (GUILayout.Button(">>"))
                    {
                        tipIndex++;
                        if (tipIndex == tips.Count)
                            tipIndex = 0;
                    }
                }
            }
        }

        private struct Tip
        {
            public string Title;
            public string Body;
        }

        private static readonly List<Tip> tips = new List<Tip>()
        {
            new Tip(){ Title = "Gameplay Ingredients Setttings", Body = "이 파일은 GameplayIngredient를 구성하는데, 매니저 자동 생성 부분을 컨트롤 할 수 있습니다. 이 에셋은 필수는 아니지만 프로젝트를 구성 할 수 있도록 생성하는 것이 좋습니다. \n \n 상단에 Window-GameplayIngredient'메뉴를 클릭하여 만드실 수 있습니다."},
            new Tip(){ Title = "Gameplay Ingredients Window", Body = "현재보고있는 창입니다. 창을 닫으면 '창 / 게임 플레이 구성 요소'메뉴를 클릭하거나 편집기를 다시 시작하여 ( '시작시 표시'플래그가 선택된 경우) 나중에 액세스 할 수 있습니다. \n \n이 창에는 툴팁이 표시되고 프로젝트의 기본 구성을 수행 할 수 있습니다. "},
            new Tip(){ Title = "Game View Link", Body = "Game View Link를 사용하면 씬 View와 게임 View를 연결하여 씬 View가 게임 View에서 복제됩니다. \n \n씬 View에서 '게임' 버튼을 클릭하여 Game View Link를 활성화/비활성화 할 수 있습니다. \n \n 버튼 옆의 잠금 아이콘을 사용하면 특정 SceneView 창에 대한 잠금 기능을 사용할 수 있습니다 (여러 장면보기를 사용할 때 유용함)."},
            new Tip(){ Title = "Advanced Hierarchy View", Body = "헤어라이키에서 오브젝트에 컴포넌트를 수록시 아이콘을 표시해줍니다. \n \n 해당 기능은 상단에 Edit - Advanced Hierarchy View 를 클릭하여 활성화/비활성화 할 수 있습니다."},
            new Tip(){ Title = "Scene View POVs", Body = "씬 View에서 POV를 사용하면 해당 시점을 저장할 수 있습니다. 사용하려면 추가 장면 뷰 도구 모음에서 POV 드롭 다운을 선택하십시오."},
            new Tip(){ Title = "Managers", Body = "매니저는 어플리케이션이 시작할 때, 자동으로 인스턴스를 생성하는 단일 동작입니다. [ManagerDefaultPrefab] 속성을 정의하여 다른 객체와 컴포넌트를 처리해야하는 경우 관리자를 포함하는 프리팹을 로드 할 수 있습니다. \n \n GameplayIngredientsSettings 에셋의 'Excluded Managers'리스트를 편집하여 특정 매니저가 생성되지 않도록 설정할 수 있습니다."},
        };

    }
}
