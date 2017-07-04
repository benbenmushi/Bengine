namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;
    using Sirenix.OdinInspector;
    public class CombineGroupAttributeExample3 : MonoBehaviour
    {
        [Range(0, 10)]
        [HorizontalGroup("Split")]
        [FoldoutGroup("Split/Left")]
        [FoldoutGroup("Split/Right")]
        [BoxGroup("Box")]
        [BoxGroup("Split/Left/Box")]
        [BoxGroup("Split/Right/Box")]
        public float A;

        [FoldoutGroup("Split/Right")]
        [FoldoutGroup("Split/Left")]
        private void Button()
        {
        }
    }
}