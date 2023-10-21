using System.Collections.Generic;
using UnityEngine;

namespace NKStudio
{
    public enum Behaviour
    {
        None,
        PlayOnDestroy,
    }

    [AddComponentMenu("Layout/Folder")]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-32000)]
    public class Folder : MonoBehaviour
    {
        public Behaviour behaviour = Behaviour.PlayOnDestroy;

        private void Awake()
        {
            if (behaviour == Behaviour.PlayOnDestroy)
                FlattenAndDestroy(transform);
        }

        /// <summary>
        /// 폴더 부모를 제거하고 자식 요소들을 부모 밖으로 빼냅니다.
        /// </summary>
        /// <param name="transform">부모 오브젝트</param>
        private static void FlattenAndDestroy(Transform transform)
        {
            List<GameObject> setChildrenActiveDelayed = null;
            Transform hierarchyFolderParent = transform.parent;

            if (transform.gameObject.activeSelf)
            {
                if (hierarchyFolderParent == null)
                    transform.DetachChildren();
                else
                {
                    int childCount = transform.childCount;
                    Transform[] children = new Transform[childCount];

                    for (int n = 0; n < childCount; n++)
                        children[n] = transform.GetChild(n);

                    for (int n = 0; n < childCount; n++)
                    {
                        Transform child = children[n];
                        child.SetParent(hierarchyFolderParent, true);
                    }

                    transform.SetParent(null, false);
                }
            }
            else
            {
                int childCount = transform.childCount;
                Transform[] children = new Transform[childCount];

                for (int n = 0; n < childCount; n++)
                    children[n] = transform.GetChild(n);

                for (int n = 0; n < childCount; n++)
                {
                    Transform child = children[n];

                    if (child != null)
                        if (child.gameObject.activeSelf)
                        {
                            if (setChildrenActiveDelayed == null)
                                setChildrenActiveDelayed = new List<GameObject>();

                            child.gameObject.SetActive(false);

                            setChildrenActiveDelayed.Add(child.gameObject);
                        }

                    child.SetParent(hierarchyFolderParent, true);
                }

                // Destroy는 짧은 지연 후에만 GameObject를 제거하기 때문에
                // 다른 스크립트로부터 가능한 한 숨기기 위해 root로 이동시킵니다.
                // 이 방법을 사용하면 GameObject를 Transform.parent, Transform.root 또는 Transform.GetChild를 사용하여 찾을 수 없게 됩니다.
                transform.SetParent(null, false);
            }

            // Destroy는 짧은 지연 후에만 GameObject를 제거하기 때문에
            // 다른 스크립트로부터 가능한 한 숨기기 위해 GameObject를 비활성화 하십시오.
            // 이 방법을 사용하면 예를 들어 FindObjectsOfType ()을 사용하여 GameObject를 찾을 수 없게 됩니다.
            transform.gameObject.SetActive(false);

            if (transform.gameObject != null)
                Destroy(transform.gameObject);
        }
    }
}
