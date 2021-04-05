using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace com.szczuro.UnityInternalEditorBridge
{
    public static class UnityBridge 
    {
        public static void OpenSpriteEditor(Object obj = null)
        {
            Assembly editorAssembly = typeof(EditorWindow).Assembly;
            var module = editorAssembly.CreateInstance("UnityEditor.SpriteUtilityWindow");
            
            
            var type = module.GetType();
            
            Debug.Log(type.Name);
            var flags = BindingFlags.Static |  BindingFlags.NonPublic;
            var methodInfo = type.GetMethod("ShowSpriteEditorWindow", flags);
            object[] parameters = {obj};
            methodInfo.Invoke(module, parameters);
            
        }
    }
}