using UnityEditor;

namespace studio.ratman.importer
{
    [CustomEditor(typeof(OraImporter))]
    public class KraImporterEditor : MultiLayerEditor
    {
        // public override VisualElement CreateInspectorGUI()
        // {
        //     return new Label("This is a Label in a Custom Editor");
        // }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}