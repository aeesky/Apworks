﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace Apworks.ObjectContainers.Unity
{
    /// <summary>
    /// Represents the object container that utilizes the Microsoft Unity as
    /// IoC/DI containers.
    /// </summary>
    public class UnityObjectContainer : ObjectContainer
    {
        #region Private Fields
        private readonly IUnityContainer container;
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>UnityObjectContainer</c> class.
        /// </summary>
        public UnityObjectContainer()
        {
            container = new Microsoft.Practices.Unity.UnityContainer();
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>A service object of type serviceType.-or- null if there is no service object
        /// of type serviceType.</returns>
        protected override object DoGetService(Type serviceType)
        {
            return container.Resolve(serviceType);
        }
        /// <summary>
        /// Gets the service object of the specified type, with overrided
        /// arguments provided.
        /// </summary>
        /// <param name="serviceType">The type of the service to get.</param>
        /// <param name="overridedArguments">The overrided arguments to be used when getting the service.</param>
        /// <returns>The instance of the service object.</returns>
        protected override object DoGetService(Type serviceType, object overridedArguments)
        {
            List<ParameterOverride> overrides = new List<ParameterOverride>();
            Type argumentsType = overridedArguments.GetType();
            argumentsType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToList()
                .ForEach(property =>
                {
                    var propertyValue = property.GetValue(overridedArguments, null);
                    var propertyName = property.Name;
                    overrides.Add(new ParameterOverride(propertyName, propertyValue));
                });
            return container.Resolve(serviceType, overrides.ToArray());
        }
        /// <summary>
        /// Resolves all the objects from the specified type.
        /// </summary>
        /// <param name="serviceType">The type of the objects to be resolved.</param>
        /// <returns>A <see cref="System.Array"/> object which contains all the objects resolved.</returns>
        protected override Array DoResolveAll(Type serviceType)
        {
            return container.ResolveAll(serviceType).ToArray();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Initializes the object container from the configuration file.
        /// </summary>
        public void InitializeFromConfigFile()
        {
            InitializeFromConfigFile(UnityConfigurationSection.SectionName);
        }
        /// <summary>
        /// Initializes the object container from the configuration file, specifying
        /// the name of the configuration section.
        /// </summary>
        /// <param name="configSectionName">The name of the configuration section.</param>
        public override void InitializeFromConfigFile(string configSectionName)
        {
            UnityConfigurationSection section = (UnityConfigurationSection)ConfigurationManager.GetSection(configSectionName);
            section.Configure(container);
        }
        /// <summary>
        /// Gets the wrapped container instance.
        /// </summary>
        /// <typeparam name="T">The type of the wrapped container.</typeparam>
        /// <returns>The instance of the wrapped container.</returns>
        public override T GetWrappedContainer<T>()
        {
            if (typeof(T).Equals(typeof(UnityContainer)))
                return (T)this.container;
            throw new InfrastructureException("The wrapped container type provided by the current object container should be '{0}'.", typeof(UnityContainer));
        }
        
        #endregion
    }
}
