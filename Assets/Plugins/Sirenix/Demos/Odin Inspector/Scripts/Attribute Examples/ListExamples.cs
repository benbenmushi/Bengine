namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;
    using System.Collections.Generic;

#if UNITY_EDITOR
    using Sirenix.Utilities.Editor;
#endif

    public class ListExamples : MonoBehaviour
    {
        public List<float> FloatList;

        [Range(0, 1)]
        public float[] FloatRangeArray;

        [ReadOnly]
        public int[] ReadOnlyArray1 = new int[] { 1, 2, 3 };

        [ListDrawerSettings(IsReadOnly = true)]
        public int[] ReadOnlyArray2 = new int[] { 1, 2, 3 };

        [ListDrawerSettings(DraggableItems = false, Expanded = false, ShowIndexLabels = true, ShowPaging = false, ShowItemCount = false)]
        public int[] MoreListSettings = new int[] { 1, 2, 3 };

        [ListDrawerSettings(HideAddButton = true, OnTitleBarGUI = "DrawAddButton")]
        public List<int> CustomButtons;

#if UNITY_EDITOR
        private void DrawAddButton()
        {
            if (SirenixEditorGUI.ToolbarButton(EditorIcons.Plus))
            {
                this.CustomButtons.Add(Random.Range(0, 100));
            }
            var prev = GUI.enabled;
            GUI.enabled = GUI.enabled && this.CustomButtons.Count > 0;
            if (SirenixEditorGUI.ToolbarButton(EditorIcons.Minus))
            {
                this.CustomButtons.RemoveAt(this.CustomButtons.Count - 1);
            }
            GUI.enabled = prev;
        }
#endif
    }
}