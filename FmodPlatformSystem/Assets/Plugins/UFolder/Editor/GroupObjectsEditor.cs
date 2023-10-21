using UnityEditor;
using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace NKStudio
{
    struct AABB
    {
        public Vector2 Min;
        public Vector2 Max;

        public AABB(Vector2 min, Vector2 max)
        {
            Min = min;
            Max = max;
        }
    }

    public class GroupObjectsEditor : Editor
    {
        private static Transform[] _lastSelectedTransforms;
        private static int _selectedItemsLeft;
        private static Dictionary<Transform, int> _hierarchyIndexes;

        private const string FolderName = "New Folder";
        private const string TargetTag = "Folder";

        [MenuItem("GameObject/Create Folder", false, 0)]
        private static void CreateFolderCommand(MenuCommand menuCommand)
        {
            GameObject obj = new(FolderName);
            obj.AddComponent<Folder>();

            int count = Selection.gameObjects.Length;
            if (count == 1)
            {
                bool isRectTransform = Selection.gameObjects[0].TryGetComponent(out RectTransform _);
                if (isRectTransform)
                    obj.AddComponent<RectTransform>();
                
                obj.transform.SetParent(Selection.gameObjects[0].transform);
                ResetTransform(obj.transform);
            }
            
            Undo.RegisterCreatedObjectUndo(obj, obj.name);
            EditorUtility.SetDirty(obj);
            Selection.activeGameObject = obj;
            AddTag(obj, TargetTag);
        }

        [MenuItem("GameObject/Create Folder", true, 0)]
        private static bool ValidateFolderCommand(MenuCommand menuCommand)
            => Selection.gameObjects.Length <= 1;

        [MenuItem("GameObject/Create Selection Folder _g", false, 0)]
        private static void CreateSelectionFolderCommand(MenuCommand menuCommand)
        {
            CreateFolder(menuCommand);
        }

        [MenuItem("GameObject/Create Selection Folder _g", true, 0)]
        private static bool ValidateSelectionFolderCommand(MenuCommand menuCommand)
            => Selection.gameObjects.Length > 0;

        /// <summary>
        /// 먼저 시스템에 태그를 추가한 다음 게임 개체에 태그를 추가합니다.
        /// </summary>
        /// <param name="obj">태그를 적용할 게임 오브젝트</param>
        /// <param name="tag">적용할 태그 이름</param>
        private static void AddTag(GameObject obj, string tag)
        {
            // 태그 매니저를 가져옵니다.
            Object[] asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");

            // 만약 가져오는 것에 실패하였다면 아무것도 하지 않습니다.
            if (asset == null || asset.Length <= 0)
                return;

            // 첫번째로 가져온 태그 매니저에 접근합니다.
            SerializedObject so = new(asset[0]);

            // tags 데이터에 접근합니다.
            SerializedProperty tags = so.FindProperty("tags");

            // 태그 매니저에 찾을려고하는 태그가 존재하면 타겟 오브젝트에 해당 태그를 적용한다.
            for (int i = 0; i < tags.arraySize; ++i)
                if (tags.GetArrayElementAtIndex(i).stringValue == tag)
                {
                    obj.tag = tag;
                    return;
                }

            // 만약 태그 매니저에 해당 태그가 없다면 태그를 추가하고 타겟 오브젝트에 해당 태그를 적용한다.
            tags.InsertArrayElementAtIndex(0);
            tags.GetArrayElementAtIndex(0).stringValue = tag;
            so.ApplyModifiedProperties();
            so.Update();

            obj.tag = tag;
        }

        /// <summary>
        /// 해당 타겟의 트랜스폼을 초기화합니다.
        /// </summary>
        /// <param name="target">초기화할 타겟 트랜스폼</param>
        private static void ResetTransform(Transform target)
        {
            target.localPosition = Vector3.zero;
            target.localRotation = Quaternion.identity;
            target.localScale = Vector3.one;
        }
        
        /// <summary>
        /// 선택한 객체를 폴더화 시킵니다.
        /// </summary>
        /// <param name="menuCommand">Menu command.</param>
        private static void CreateFolder(MenuCommand menuCommand)
        {
            // 첫번째 사이클이 돌고나면, _lastSelectedTransforms에 Selection.transforms가 바인딩됩니다.
            // 두번째 사이클 때부터는 사실상 return을 계속적으로 처리되고,
            // _selectedItemsLeft -= 1; 코드를 통해 _selectedItemsLeft가 0이 되면 _lastSelectedTransforms를 null로 초기화합니다.
            if (_lastSelectedTransforms != null)
                if (_lastSelectedTransforms.SequenceEqual(Selection.transforms))
                {
                    _selectedItemsLeft -= 1;
                    if (_selectedItemsLeft <= 0)
                    {
                        // 사용자가 실제로 메뉴 항목을 다시 클릭할 수 있도록 선택 항목을 지우면 작동합니다.
                        _lastSelectedTransforms = null;
                    }
                    return;
                }

            // 선택한 오브젝트 개수에 따라 몇번 return 시킬 것인지에 따른 영향을 끼칩니다.
            _selectedItemsLeft = Selection.transforms.Length - 1;

            // 선택된 오브젝트의 부모 오브젝트를 가져옵니다.
            Transform groupParent = Selection.activeTransform.parent;
            bool parentFound = false;

            // 순서를 유지할 수 있도록 모든 항목의 형제 인덱스를 저장합니다.
            _hierarchyIndexes = new Dictionary<Transform, int>();
            int groupSiblingIndex = 999999999;

            // 선택한 오브젝트들 만큼 반복을 처리합니다.
            foreach (Transform transform in Selection.transforms)
            {
                // 부모가 없는 경우,
                if (!parentFound)
                    if (!Array.Exists(Selection.transforms, element => element == transform.parent))
                    {
                        // 선택 항목 내부에 상위가 없는 변환을 찾았으므로 최고 수준입니다.
                        groupParent = transform.parent;
                        parentFound = true;
                    }

                // 나중에 순서를 복원할 수 있도록 계층 구조의 순서를 저장합니다.
                _hierarchyIndexes[transform] = transform.GetSiblingIndex();

                // 가장 낮은 요소의 인덱스를 저장하여 가장 높은 요소의 위치에 그룹을 삽입합니다.
                groupSiblingIndex = Math.Min(groupSiblingIndex, transform.GetSiblingIndex());
            }
            bool isRectTransform;
            
            if (!groupParent)
                isRectTransform = false;
            else
                isRectTransform = groupParent.TryGetComponent(out RectTransform _);

            // 폴더 오브젝트 인스턴스 입니다.
            GameObject go = new(FolderName);
            if (isRectTransform)
                go.AddComponent<RectTransform>();
            go.AddComponent<Folder>();

            if (isRectTransform)
            {
                var calculateData = CalculateCenterAndSize(Selection.gameObjects);
                var folderRectTransform = ((RectTransform)go.transform);
                folderRectTransform.position = calculateData.centerPosition;
                folderRectTransform.sizeDelta = calculateData.size;
            }

            // 생성된 객체를 Undo 스택에 등록
            Undo.RegisterCreatedObjectUndo(go, "Group Selected");

            // 태그를 지정합니다.
            AddTag(go, TargetTag);

            // 새롭게 생성된 폴더 오브젝트의 부모를 설정합니다.
            go.transform.SetParent(groupParent, false);

            // 계층 구조의 올바른 순서를 순서를 설정합니다.
            go.transform.SetSiblingIndex(groupSiblingIndex);

            // 선택한 항목을 인덱스별로 정렬합니다.
            IEnumerable<Transform> sortedByIndex = Selection.transforms.OrderByDescending(t => _hierarchyIndexes[t]);

            // 선택한 모든 개체를 새로 생성된 그룹 개체의 자식으로 설정합니다.
            foreach (var transform in Selection.transforms)
                Undo.SetTransformParent(transform, go.transform, "Group Selected");

            // 항목을 다시 정렬하여 그룹화 전과 동일한 순서가 되도록 합니다.
            foreach (var item in sortedByIndex)
            {
                // 이 경우, 이전 형제 인덱스 값은 순차적이고 0 기반(예: 0,1,2,3,4)이어야 하므로 그냥 복사할 수 없습니다.
                item.SetAsFirstSibling();
            }

            // 에디터에서 선택 타겟을 새로 생성된 그룹 개체로 설정합니다.
            Selection.activeGameObject = go;

            // 가비지 컬렉션에 수거되도록 합니다.
            _hierarchyIndexes = null;

            // 컨텍스트 메뉴에서 호출하면 마지막으로 선택한 트랜스폼이
            // 저장되므로 명령이 한 번이 아닌 여러 번 실행되지 않습니다.
            if (menuCommand.context)
                _lastSelectedTransforms = Selection.transforms;
        }

        /// <summary>
        /// 인자로 들어온 게임 오브젝트들의 중심 위치를 구합니다.
        /// </summary>
        /// <returns></returns>
        private static (Vector2 centerPosition, Vector2 size) CalculateCenterAndSize(IReadOnlyList<GameObject> gameObjects)
        {
            // 선택한 오브젝트들의 AABB를 계산합니다.
            AABB[] aabbs = new AABB[gameObjects.Count];
            for (int i = 0; i < gameObjects.Count; i++)
                aabbs[i] = ConvertAABB(gameObjects[i].transform);

            // Min중에서 제일 최고의 Min 값과 Max중에서 제일 낮은 Max 값을 가져옵니다.
            Vector2 min = Vector2.zero;
            Vector2 max = Vector2.zero;

            for (int i = 0; i < aabbs.Length; i++)
            {
                if (i == 0)
                {
                    min = aabbs[i].Min;
                    max = aabbs[i].Max;
                }
                else
                {
                    min = Vector2.Min(min, aabbs[i].Min);
                    max = Vector2.Max(max, aabbs[i].Max);
                }
            }

            // min과 max를 사용하여 중앙 위치를 구합니다.
            Vector2 center = (min + max)*0.5f;

            var canvas = gameObjects[0].GetComponentInParent<Canvas>();
            var sampleCenter = canvas.transform.InverseTransformPoint(center);
            var sampleMin = canvas.transform.InverseTransformPoint(min);
            var sampleMax = canvas.transform.InverseTransformPoint(max);

            var size = sampleMax - sampleMin;

            return (sampleCenter, size);
        }

        /// <summary>
        /// 트랜스폼을 RectTransform으로 처리하여 AABB를 계산합니다.
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        private static AABB ConvertAABB(Transform transform)
        {
            RectTransform rectTransform = transform as RectTransform;

            var bounds = CalculateAABB(rectTransform);

            Vector2 min = new(bounds.min.x, bounds.min.y);
            Vector2 max = new(bounds.max.x, bounds.max.y);

            return new AABB(min, max);
        }

        /// <summary>
        /// 인자로 들어온 RectTransform의 AABB를 계산합니다.
        /// </summary>
        /// <param name="rectTransform">AABB를 구할 RectTransform</param>
        /// <returns>AABB로 처리된 Bounds를 반환합니다.</returns>
        private static Bounds CalculateAABB(RectTransform rectTransform)
        {
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);

            Vector3 min = corners[0];
            Vector3 max = corners[0];

            for (var i = 1; i < 4; i++)
            {
                min = Vector3.Min(min, corners[i]);
                max = Vector3.Max(max, corners[i]);
            }

            Bounds bounds = new Bounds();
            bounds.SetMinMax(min, max);

            return bounds;
        }
    }
}
