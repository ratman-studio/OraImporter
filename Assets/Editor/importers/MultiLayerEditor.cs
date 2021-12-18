using studio.ratman.UnityInternalEditorBridge;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace studio.ratman.importer
{
    public class MultiLayerEditor : ScriptedImporterEditor
    {
        private const string PROPERTY_NAME_IMPORTAS = "importAs";
        private const string PROPERTY_NAME_ATLAS = "atlas";
        
        private SerializedProperty _importAs;
        private SerializedProperty _atlas; 

        public override void OnInspectorGUI()
        {
            
            serializedObject.Update();
            
            _importAs = serializedObject.FindProperty(PROPERTY_NAME_IMPORTAS);
            _atlas = serializedObject.FindProperty(PROPERTY_NAME_ATLAS);
            
            DoInspector();

            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }

        private void DoInspector()
        {
            var importer = serializedObject.targetObject as MultiLayerImporter;
            
            _importAs.intValue = EditorGUILayout.IntPopup(GUITexts.ImportTypeTitle, _importAs.intValue,
                GUITexts.ImportTypeOptions, GUITexts.ImportTypeValues);
            
            switch ((MultiLayerImporter.ImportType) _importAs.intValue)
            {
                case MultiLayerImporter.ImportType.Atlas:
                    EditorGUILayout.LabelField("Layers");
                    MultiLayerImportGUI();
                    break;

                default:
                    EditorGUILayout.LabelField("Single");
                    SingleImageImportGUI();
                    //ImportAs.intValue = (int) ImportType.Single;
                    break;
            }

            DoSpriteEditorButton();
        }

        private void DoSpriteEditorButton()
        {
            using (new EditorGUI.DisabledScope(targets.Length != 1))
            {
                GUILayout.BeginHorizontal();

                GUILayout.FlexibleSpace();
                if (GUILayout.Button(GUITexts.spriteEditorButtonLabel))
                {
     if (HasModified())
                    {
                        // To ensure Sprite Editor Window to have the latest texture import setting,
                        // We must applied those modified values first.
                        string dialogText = string.Format(GUITexts.unappliedSettingsDialogContent.text,
                            ((AssetImporter) target).assetPath);
                        if (EditorUtility.DisplayDialog(GUITexts.unappliedSettingsDialogTitle.text,
                            dialogText, GUITexts.applyButtonLabel.text, GUITexts.revertButtonLabel.text))
                        {
                            ApplyAndImport();

                            UnityBridge.OpenSpriteEditor(this.assetTarget);
                            // showSpriteEditorWindow();

                            // We reimported the asset which destroyed the editor, so we can't keep running the UI here.
                            GUIUtility.ExitGUI();
                        }
                    }
                    else
                    {
                        Selection.activeObject = assetTarget;
                        UnityBridge.OpenSpriteEditor(this.assetTarget);
                        //showSpriteEditorWindow();
                    }
                }

                GUILayout.EndHorizontal();
            }
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

        public static readonly GUIContent noSpriteEditorWindowTitle =
            EditorGUIUtility.TrTextContent("Sprite Editor Window");

        public static readonly GUIContent noSpriteEditorWindow =
            EditorGUIUtility.TrTextContent(
                "No Sprite Editor Window registered. Please download 2D Sprite package from Package Manager.");

        public static readonly GUIContent okText = EditorGUIUtility.TrTextContent("OK");

        public static readonly GUIContent unappliedSettingsDialogTitle = new GUIContent("Unapplied import settings");

        public static readonly GUIContent unappliedSettingsDialogContent =
            new GUIContent("Unapplied import settings for \'{0}\'.\nApply and continue to sprite editor or cancel.");

        public static readonly GUIContent applyButtonLabel = new GUIContent("Apply");
        public static readonly GUIContent revertButtonLabel = new GUIContent("Revert");
        public static readonly GUIContent spriteEditorButtonLabel = new GUIContent("Sprite Editor");


        public static readonly GUIContent[] ImportTypeOptions =
        {
            new GUIContent("Merged",
                "This is the default Texture Type usage when an image is imported without a specific Texture Type selected"),
            new GUIContent("Layers", "Texture is imported as layers."),
        };

        public static readonly int[] ImportTypeValues =
        {
            (int) MultiLayerImporter.ImportType.Single,
            (int) MultiLayerImporter.ImportType.Atlas
        };
    }
}