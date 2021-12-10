using System;
using System.Reflection;
using UnityEditor;
using Object = UnityEngine.Object;

namespace studio.ratman.UnityInternalEditorBridge
{
    public static class UnityBridge 
    {
        public static void OpenSpriteEditor(Object obj = null)
        {
            NewWay(obj);
            // Assembly editorAssembly = typeof(EditorWindow).Assembly;
            // var module = editorAssembly.CreateInstance("UnityEditor.SpriteUtilityWindow");
            //
            //
            // var type = module.GetType();
            //
            // Debug.Log(type.Name);
            // var flags = BindingFlags.Static |  BindingFlags.NonPublic;
            // var methodInfo = type.GetMethod("ShowSpriteEditorWindow", flags);
            // object[] parameters = {obj};
            // methodInfo.Invoke(module, parameters);

        }

        private static void NewWay(Object o)
        {
            Selection.activeObject = o;
            EditorWindow.GetWindow(Type.GetType("UnityEditor.U2D.Sprites.SpriteEditorWindow,Unity.2D.Sprite.Editor"));

        }
    }
}