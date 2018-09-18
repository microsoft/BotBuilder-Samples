// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;
using Unity;
using Unity.Exceptions;

namespace QnABot
{
    /// <summary>
    /// Resolves dependencies for dependency injection.
    /// </summary>
    /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/dependency-injection"/>
    public class UnityResolver : IDependencyResolver
    {
        private readonly IUnityContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityResolver"/> class.
        /// </summary>
        /// <param name="container">The container that resolution will be performed upon.</param>
        public UnityResolver(IUnityContainer container)
        {
            _container = container;
        }

        /// <summary>
        /// Resolves singly registered services that support arbitrary object creation.
        /// </summary>
        /// <param name="serviceType">Type of the service to resolve.</param>
        /// <returns>The requested service.</returns>
        public object GetService(Type serviceType)
        {
            try
            {
                return _container.Resolve(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return null;
            }
        }

        /// <summary>
        /// Creates a collection of objects of a specified type.
        /// </summary>
        /// <param name="serviceType">Type of the service to resolve.</param>
        /// <returns>Collection of the requested services.</returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return _container.ResolveAll(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return new List<object>();
            }
        }

        /// <summary>
        /// Starts a resolution scope.
        /// </summary>
        /// <remarks>Objects which are resolved in the given scope will belong to that scope,
        /// and when the scope is disposed, those objects are returned to the container.
        /// </remarks>
        /// <returns>The dependency scope.</returns>
        public IDependencyScope BeginScope()
        {
            var child = _container.CreateChildContainer();
            return new UnityResolver(child);
        }

        /// <summary>
        /// Performs container-associated tasks with releasing resources.
        /// </summary>
        public void Dispose()
        {
            _container.Dispose();
        }
    }
}
