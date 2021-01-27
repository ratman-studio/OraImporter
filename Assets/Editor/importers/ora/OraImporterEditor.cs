using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace com.szczuro.importer.ora
{
    [ScriptedImporter(1, "ora")]
    public class OraImporter : MultiLayerImporter
    {
    }


    [CustomEditor(typeof(OraImporter))]
    public class OraImporterEditor : ScriptedImporterEditor
    {
        private SerializedProperty ImportAs;

        public override void OnEnable()
        {
            ImportAs = serializedObject.FindProperty("ImportAs");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            base.OnInspectorGUI();

            EditorGUI.showMixedValue = ImportAs.hasMultipleDifferentValues;
            ImportAs.intValue = EditorGUILayout.IntPopup(GUITexts.ImportTypeTitle, ImportAs.intValue,
                GUITexts.ImportTypeOptions, GUITexts.ImportTypeValues);
            EditorGUI.showMixedValue = false;

            switch ((MultiLayerImporter.ImportType) ImportAs.intValue)
            {
                case MultiLayerImporter.ImportType.Multi:
                    EditorGUILayout.LabelField("Layers");
                    MultiLayerImportGUI();
                    break;
                    ;
                case MultiLayerImporter.ImportType.Single:
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
        public static GUIContent ImportTypeTitle =
            new GUIContent("Texture Type", "What will this texture be used for?");

        public static GUIContent[] ImportTypeOptions =
        {
            new GUIContent("Merged",
                "This is the default Texture Type usage when an image is imported without a specific Texture Type selected"),
            new GUIContent("Layers", "Texture is imported as layers."),
        };

        public static int[] ImportTypeValues =
        {
            (int) MultiLayerImporter.ImportType.Single,
            (int) MultiLayerImporter.ImportType.Multi
        };
    }
}