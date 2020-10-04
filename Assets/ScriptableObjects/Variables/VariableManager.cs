using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

namespace com.szczuro.variables
{
    [CreateAssetMenu (fileName ="VariableManager", menuName ="Variables/VariableManager")]
    public class VariableManager : ScriptableObject
    {

        public List<BaseVariable> Variables = new List<BaseVariable>();

            private void OnEnable()
        {
#if UNITY_EDITOR
            Variables = new List<BaseVariable>(GetAllVariables());
#endif
        }

        // This method works only in Editor
        private static BaseVariable[] GetAllVariables()
        {
            string[] guids = AssetDatabase.FindAssets("t:BaseVariable");
            BaseVariable[] vars = new BaseVariable[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            { 
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                vars[i] = AssetDatabase.LoadAssetAtPath<BaseVariable>(path);
            }
            return vars;
        }

    }
}