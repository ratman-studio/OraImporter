using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace com.szczuro.importer.ora
{
    [CustomEditor(typeof(OraImporter))]
    public class OraImporterEditor : ScriptedImporterEditor
    {
        private SerializedProperty _importAs;

        public override void OnEnable()
        {
            base.OnEnable();
            _importAs = serializedObject.FindProperty("ImportAs");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            base.OnInspectorGUI();

            EditorGUI.showMixedValue = _importAs.hasMultipleDifferentValues;
            _importAs.intValue = EditorGUILayout.IntPopup(GUITexts.ImportTypeTitle, _importAs.intValue,
                GUITexts.ImportTypeOptions, GUITexts.ImportTypeValues);
            EditorGUI.showMixedValue = false;

            switch ((MultiLayerImporter.ImportType) _importAs.intValue)
            {
                case MultiLayerImporter.ImportType.Multi:
                    EditorGUILayout.LabelField("Layers");
                    MultiLayerImportGUI();
                    break;

                default:
                    EditorGUILayout.LabelField("Single");
                    SingleImageImportGUI();
                    //ImportAs.intValue = (int) ImportType.Single;
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void SingleImageImportGUI()
        {
            //throw new System.NotImplementedException();
        }

        private void MultiLayerImportGUI()
        {
            //throw new System.NotImplementedException();
        }
    }

    internal static class GUITexts
    {
        public static readonly GUIContent ImportTypeTitle =
            new GUIContent("Texture Type", "What will this texture be used for?");

        public static readonly GUIContent[] ImportTypeOptions =
        {
            new GUIContent("Merged",
                "This is the default Texture Type usage when an image is imported without a specific Texture Type selected"),
            new GUIContent("Layers", "Texture is imported as layers."),
        };

        public static readonly int[] ImportTypeValues =
        {
            (int) MultiLayerImporter.ImportType.Single,
            (int) MultiLayerImporter.ImportType.Multi
        };
    }
}