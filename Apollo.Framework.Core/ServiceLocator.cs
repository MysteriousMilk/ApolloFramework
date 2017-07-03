// ******************************************************************
// Apollo Framework: Game Engine Framework for MonoGame
//
// MIT License
// Copyright(c) 2017 MysteriousMilk
//
// This source code file is subject to the terms and conditions defined 
// in the LICENSE file, distributed as part of this source code package.
// ******************************************************************

using System;
using System.Collections.Generic;

namespace Apollo.Framework.Core
{
    public class ServiceLocator : IServiceLocator
    {
        private static Dictionary<Type, object> _InstanceMap;

        static ServiceLocator()
        {
            _InstanceMap = new Dictionary<Type, object>();
        }

        /// <summary>
        /// Registers a new service with the service locator.
        /// </summary>
        /// <typeparam name="T">The type of service.</typeparam>
        /// <param name="service">The instance of the service.</param>
        public void Register<T>(T service)
        {
            Type serviceType = typeof(T);

            _InstanceMap.Remove(serviceType);
            _InstanceMap.Add(serviceType, service);

            if (SubSystem.Instance.Services.HasInstance<ILogger>())
                SubSystem.Instance.Services.GetInstance<ILogger>().WriteLine(LogEntryType.Info, serviceType.Name + " registered as a service.");
        }

        internal void Register(Type type, object service)
        {
            _InstanceMap.Add(type, service);
        }

        /// <summary>
        /// Gets an instances of a service from the service locator.
        /// </summary>
        /// <typeparam name="T">The type of service.</typeparam>
        /// <returns>The instance of the service.</returns>
        /// <remarks>
        /// If the service is not found in the service locator, null
        /// will be returned.
        /// </remarks>
        public T GetInstance<T>()
        {
            Type serviceType = typeof(T);

            if (_InstanceMap.TryGetValue(serviceType, out object service))
                return (T)service;

            return default(T);
        }

        /// <summary>
        /// Checks to see if an instance of the specified type exists
        /// in the service locator.
        /// </summary>
        /// <typeparam name="T">The type of service.</typeparam>
        /// <returns>True if the instance exists and False if it does not.</returns>
        public bool HasInstance<T>()
        {
            Type serviceType = typeof(T);
            return _InstanceMap.ContainsKey(serviceType);
        }
    }
}
