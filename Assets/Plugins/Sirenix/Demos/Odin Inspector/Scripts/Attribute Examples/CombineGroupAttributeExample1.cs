namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;
    using Sirenix.OdinInspector;

    public class CombineGroupAttributeExample1 : MonoBehaviour
    {
        [HorizontalGroup("Split")]
        [BoxGroup("Split/Left")]
        public int[] A;

        [BoxGroup("Split/Left")]
        public int[] C;

        [HorizontalGroup("Split", width: 100)]
        [BoxGroup("Split/Center")]
        public int[] B;

        [BoxGroup("Split/Center")]
        public int[] D;

        [BoxGroup("Split/Right")]
        public int[] E;

        [BoxGroup("Split/Right")]
        public int[] F;
    }
}