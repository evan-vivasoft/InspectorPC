/*
//===============================================================================
// Copyright Kamstrup
// All rights reserved.
//===============================================================================
*/



namespace Inspector.Infra.Ioc
{
    /// <summary>
    /// Interface for exposing the application context.
    /// </summary>
    public interface IApplicationContext
    {
        /// <summary>
        /// Returns a component instance by the interface
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        T Resolve<T>();

        /// <summary>
        /// Resolves the specified component id.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arguments">The arguments.</param>
        /// <returns></returns>
        T Resolve<T>(object[] arguments);


        /// <summary>
        /// Returns a component instance by the key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="compenentId"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        T Resolve<T>(string compenentId);

        /// <summary>
        /// Resolves the specified component id.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="componentId">The component id.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns></returns>
        T Resolve<T>(string componentId, object[] arguments);


        /// <summary>
        /// Releases a component instance
        /// </summary>
        /// <param name="instance"></param>
        void Release(object instance);

        /// <summary>
        /// Configures this instance.
        /// </summary>
        void Configure();
    }

    #region Spring Abstraction
    /// <summary>
    /// SpringApplicationContext
    /// </summary>
    class SpringApplicationContext : IApplicationContext
    {

        /// <summary>
        /// Configures this instance.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "ctx")]
        public void Configure()
        {
            Spring.Context.IApplicationContext ctx = Spring.Context.Support.ContextRegistry.GetContext();
        }

        /// <summary>
        /// Returns a component instance by the interface
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Resolve<T>()
        {
            Spring.Context.IApplicationContext ctx = Spring.Context.Support.ContextRegistry.GetContext();
            return (T)ctx.GetObject(typeof(T).Name);
        }

        /// <summary>
        /// Resolves the specified component id.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arguments">The arguments.</param>
        /// <returns></returns>
        public T Resolve<T>(object[] arguments)
        {
            Spring.Context.IApplicationContext ctx = Spring.Context.Support.ContextRegistry.GetContext();
            return (T)ctx.GetObject(typeof(T).Name, typeof(T), arguments);
        }


        /// <summary>
        /// Returns a component instance by the key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="compenentId"></param>
        /// <returns></returns>
        public T Resolve<T>(string compenentId)
        {
            Spring.Context.IApplicationContext ctx = Spring.Context.Support.ContextRegistry.GetContext();
            return (T)ctx.GetObject(compenentId, typeof(T));
        }

        /// <summary>
        /// Resolves the specified component id.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="componentId">The component id.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns></returns>
        public T Resolve<T>(string componentId, object[] arguments)
        {
            Spring.Context.IApplicationContext ctx = Spring.Context.Support.ContextRegistry.GetContext();
            return (T)ctx.GetObject(componentId, typeof(T), arguments);
        }

        /// <summary>
        /// Releases a component instance
        /// </summary>
        /// <param name="instance"></param>
        public void Release(object instance)
        {
        }
    }
    #endregion

    /// <summary>
    /// ContextRegistry: Provides access to a central component registry
    /// <example>
    /// <![CDATA[
    /// IOrder order = ContextRegistry.GetContext().Resolve<IOrder>();
    ///   or
    /// IOrder order = ContextRegistry.GetContext().Resolve<IOrder>("my.order");
    /// ]]>
    /// </example>
    /// </summary>
    public sealed class ContextRegistry
    {
        /// <summary>
        /// Private constructor.
        /// </summary>
        private ContextRegistry()
        {
        }

        private static IApplicationContext appContext;

        /// <summary>
        /// Returns a reference to the IOC container
        /// </summary>
        /// <value>The container.</value>
        public static IApplicationContext Context
        {
            get
            {
                if (appContext == null)
                {
                    // Spring
                    appContext = new SpringApplicationContext();
                }
                return appContext;
            }
        }
    }
}
