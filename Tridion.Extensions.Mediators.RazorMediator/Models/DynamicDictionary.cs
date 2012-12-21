using System;
using System.Collections.Generic;
using System.Dynamic;
using Tridion.ContentManager.Templating;

namespace Tridion.Extensions.Mediators.Razor.Models
{
    /// <summary>
    /// Dynamic Dictionary - Allows setting and getting of instance properties dynamically.
    /// </summary>
    public class DynamicDictionary : DynamicObject
    {
        /// <summary>
        /// The inner dictionary.
        /// </summary>
        protected Dictionary<string, object> _dictionary = new Dictionary<string, object>();

        /// <summary>
        /// The Tridion Templating Logger.
        /// </summary>
        private TemplatingLogger _logger;

        /// <summary>
        /// The Tridion Templating Logger.
        /// </summary>
        protected TemplatingLogger Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = TemplatingLogger.GetLogger(this.GetType());
                }
                return _logger;
            }
        }

        /// <summary>
        /// This property returns the number of elements in the inner dictionary.
        /// </summary>
        public int Length
        {
            get
            {
                return _dictionary.Count;
            }
        }

        // If you try to get a value of a property 
        // not defined in the class, this method is called.
        public override bool TryGetMember(
            GetMemberBinder binder, out object result)
        {
            // Converting the property name to lowercase
            // so that property names become case-insensitive.
            string name = binder.Name.ToLower();

            if (!_dictionary.ContainsKey(name))
            {
                Logger.Error(String.Format("No Key Found In Dictionary for '{0}'", name));
                result = String.Empty;
                return false;
            }

            // If the property name is found in a dictionary,
            // set the result parameter to the property value and return true.
            // Otherwise, return false.
            return _dictionary.TryGetValue(name, out result);
        }

        // If you try to set a value of a property that is
        // not defined in the class, this method is called.
        public override bool TrySetMember(
            SetMemberBinder binder, object value)
        {
            // Converting the property name to lowercase
            // so that property names become case-insensitive.
            _dictionary[binder.Name.ToLower()] = value;

            // You can always add a value to a dictionary,
            // so this method always returns true.
            return true;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            
            string index = ((string)indexes[0]).ToLower();
            Logger.Debug("TrySetIndex For " + index);

            if (_dictionary.ContainsKey(index))
            {
                _dictionary[index] = value;
            }
            else
            {
                _dictionary.Add(index, value);
            }

            return true;
        }
    }
}
