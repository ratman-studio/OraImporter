/// base on 
/// https://fishtrone.wordpress.com/2018/09/16/saving-scriptable-object-variables/
/// demo https://github.com/randalfien/scriptable-variables
/// 
using System;

using UnityEngine;

namespace com.szczuro.variables
{
    /// <summary>
    /// This class is needed for Variable Manager to work properly
    /// </summary>
    public class BaseVariable : ScriptableObject {
        
    }

    /// <summary>
    ///  this is  base variable to all scriptable pbject  variables derive from
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Variable<T> : BaseVariable
    {
        public T DefaultValue;

        /// <summary>
        /// Value thati is changed at runtime without modifing initial variable 
        /// </summary>
        public T RuntimeValue { get; set; }
        public void onEnable()
        {
            RuntimeValue = DefaultValue;
        }

    }


}