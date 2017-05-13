// ******************************************************************
// Apollo Framework: Game Engine Framework for MonoGame
//
// MIT License
// Copyright(c) 2017 MysteriousMilk
//
// This source code file is subject to the terms and conditions defined 
// in the LICENSE file, distributed as part of this source code package.
// ******************************************************************

using System.Collections;
using System.Collections.Generic;

namespace Apollo.Core
{
    /// <summary>
    /// A collection of user-defined propertie/data.
    /// Use this when there is no other method to store custom object specific data.
    /// </summary>
    public class PropertyCollection : IEnumerable<KeyValuePair<string, string>>
    {
        private Dictionary<string, string> _tagTable;

        public PropertyCollection()
        {
            _tagTable = new Dictionary<string, string>();
        }

        /// <summary>
        /// Sets a custom <see cref="Node"/> property.
        /// </summary>
        /// <param name="key">The property key/name.</param>
        /// <param name="value">The value of the property.</param>
        public void SetProperty(string key, string value)
        {
            if (_tagTable.ContainsKey(key))
                _tagTable.Remove(key);

            _tagTable.Add(key, value);
        }

        /// <summary>
        /// Gets a custom <see cref="Node"/> property.
        /// </summary>
        /// <param name="key">The property key/name.</param>
        /// <returns>The value of the property.</returns>
        public string GetProperty(string key)
        {
            string value = string.Empty;

            if (_tagTable.TryGetValue(key, out value))
                return value;

            return string.Empty;
        }

        #region IEnumerable
        /// <summary>
        /// Iterates through the key/values of the <see cref="PropertyCollection"/>.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            foreach (var property in _tagTable)
                yield return property;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion
    }
}
