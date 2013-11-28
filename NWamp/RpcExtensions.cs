namespace NWamp
{
    using System;
    using NWamp.Protocol.Rpc;

    /// <summary>
    /// Collection of extension methods used for <see cref="IRpcHandler"/>.
    /// </summary>
    public static class RpcExtensions
    {
        #region Actions

        /// <summary>
        /// Register a method handler delegate under specific WAMP procedure URI identifier.
        /// </summary>
        public static void RegisterAction(this IRpcHandler self, string procId, Action action)
        {
            self.RegisterRpcAction(procId, _ =>
            {
                action();
                return null;
            });
        }

        /// <summary>
        /// Register a method handler delegate under specific WAMP procedure URI identifier.
        /// </summary>
        public static void RegisterAction<T1>(this IRpcHandler self, string procId, Action<T1> action)
        {
            self.RegisterRpcAction(procId, args =>
            {
                if (args.Length < 1)
                    throw new ArgumentException(
                        "Incompatibile number of arguments provided to registered action. Procedure uri: " + procId);

                if (!(args[0] is T1)) args[0] = self.TypeResolver(args[0], args[0].GetType(), typeof(T1));

                action.DynamicInvoke(args);
                return null;
            });
        }

        /// <summary>
        /// Register a method handler delegate under specific WAMP procedure URI identifier.
        /// </summary>
        public static void RegisterAction<T1, T2>(this IRpcHandler self, string procId, Action<T1, T2> action)
        {
            self.RegisterRpcAction(procId, args =>
            {
                if (args.Length < 2)
                    throw new ArgumentException(
                        "Incompatibile number of arguments provided to registered action. Procedure uri: " + procId);

                if (!(args[0] is T1)) args[0] = self.TypeResolver(args[0], args[0].GetType(), typeof(T1));
                if (!(args[1] is T2)) args[1] = self.TypeResolver(args[1], args[1].GetType(), typeof(T2));

                action.DynamicInvoke(args);
                return null;
            });
        }

        /// <summary>
        /// Register a method handler delegate under specific WAMP procedure URI identifier.
        /// </summary>
        public static void RegisterAction<T1, T2, T3>(this IRpcHandler self, string procId, Action<T1, T2, T3> action)
        {
            self.RegisterRpcAction(procId, args =>
            {
                if (args.Length < 3)
                    throw new ArgumentException(
                        "Incompatibile number of arguments provided to registered action. Procedure uri: " + procId);

                if (!(args[0] is T1)) args[0] = self.TypeResolver(args[0], args[0].GetType(), typeof(T1));
                if (!(args[1] is T2)) args[1] = self.TypeResolver(args[1], args[1].GetType(), typeof(T2));
                if (!(args[2] is T3)) args[2] = self.TypeResolver(args[2], args[2].GetType(), typeof(T3));

                action.DynamicInvoke(args);
                return null;
            });
        }

        /// <summary>
        /// Register a method handler delegate under specific WAMP procedure URI identifier.
        /// </summary>
        public static void RegisterAction<T1, T2, T3, T4>(this IRpcHandler self, string procId, Action<T1, T2, T3, T4> action)
        {
            self.RegisterRpcAction(procId, args =>
            {
                if (args.Length < 4)
                    throw new ArgumentException(
                        "Incompatibile number of arguments provided to registered action. Procedure uri: " + procId);

                if (!(args[0] is T1)) args[0] = self.TypeResolver(args[0], args[0].GetType(), typeof(T1));
                if (!(args[1] is T2)) args[1] = self.TypeResolver(args[1], args[1].GetType(), typeof(T2));
                if (!(args[2] is T3)) args[2] = self.TypeResolver(args[2], args[2].GetType(), typeof(T3));
                if (!(args[3] is T4)) args[3] = self.TypeResolver(args[3], args[3].GetType(), typeof(T4));

                action.DynamicInvoke(args);
                return null;
            });
        }
        
         /// <summary>
        /// Register a method handler delegate under specific WAMP procedure URI identifier.
        /// </summary>
        public static void RegisterAction<T1, T2, T3, T4, T5>(this IRpcHandler self, string procId, Action<T1, T2, T3, T4, T5> action)
        {
            self.RegisterRpcAction(procId, args =>
            {
                if (args.Length < 5)
                    throw new ArgumentException(
                        "Incompatibile number of arguments provided to registered action. Procedure uri: " + procId);

                if (!(args[0] is T1)) args[0] = self.TypeResolver(args[0], args[0].GetType(), typeof(T1));
                if (!(args[1] is T2)) args[1] = self.TypeResolver(args[1], args[1].GetType(), typeof(T2));
                if (!(args[2] is T3)) args[2] = self.TypeResolver(args[2], args[2].GetType(), typeof(T3));
                if (!(args[3] is T4)) args[3] = self.TypeResolver(args[3], args[3].GetType(), typeof(T4));
                if (!(args[4] is T5)) args[4] = self.TypeResolver(args[4], args[4].GetType(), typeof(T5));

                action.DynamicInvoke(args);
                return null;
            });
        }


        #endregion

        #region Functions

        /// <summary>
        /// Register a method handler delegate under specific WAMP procedure URI identifier.
        /// </summary>
        public static void RegisterFunc<TResult>(this IRpcHandler self, string procId, Func<TResult> func)
        {
            self.RegisterRpcAction(procId, _ => func());
        }

        /// <summary>
        /// Register a method handler delegate under specific WAMP procedure URI identifier.
        /// </summary>
        public static void RegisterFunc<T1, TResult>(this IRpcHandler self, string procId, Func<T1, TResult> func)
        {
            self.RegisterRpcAction(procId, args =>
            {
                if (args.Length < 1)
                    throw new ArgumentException(
                        "Incompatibile number of arguments provided to registered action. Procedure uri: " + procId);

                if (!(args[0] is T1)) args[0] = self.TypeResolver(args[0], args[0].GetType(), typeof(T1));

                return func.DynamicInvoke(args);
            });
        }

        /// <summary>
        /// Register a method handler delegate under specific WAMP procedure URI identifier.
        /// </summary>
        public static void RegisterFunc<T1, T2, TResult>(this IRpcHandler self, string procId, Func<T1, T2, TResult> func)
        {
            self.RegisterRpcAction(procId, args =>
            {
                if (args.Length < 2)
                    throw new ArgumentException(
                        "Incompatibile number of arguments provided to registered action. Procedure uri: " + procId);

                if (!(args[0] is T1)) args[0] = self.TypeResolver(args[0], args[0].GetType(), typeof(T1));
                if (!(args[1] is T2)) args[1] = self.TypeResolver(args[1], args[1].GetType(), typeof(T2));

                return func.DynamicInvoke(args);
            });
        }

        /// <summary>
        /// Register a method handler delegate under specific WAMP procedure URI identifier.
        /// </summary>
        public static void RegisterFunc<T1, T2, T3, TResult>(this IRpcHandler self, string procId, Func<T1, T2, T3, TResult> func)
        {
            self.RegisterRpcAction(procId, args =>
            {
                if (args.Length < 3)
                    throw new ArgumentException(
                        "Incompatibile number of arguments provided to registered action. Procedure uri: " + procId);

                if (!(args[0] is T1)) args[0] = self.TypeResolver(args[0], args[0].GetType(), typeof(T1));
                if (!(args[1] is T2)) args[1] = self.TypeResolver(args[1], args[1].GetType(), typeof(T2));
                if (!(args[2] is T3)) args[2] = self.TypeResolver(args[2], args[2].GetType(), typeof(T3));

                return func.DynamicInvoke(args);
            });
        }

        /// <summary>
        /// Register a method handler delegate under specific WAMP procedure URI identifier.
        /// </summary>
        public static void RegisterFunc<T1, T2, T3, T4, TResult>(this IRpcHandler self, string procId, Func<T1, T2, T3, T4, TResult> func)
        {
            self.RegisterRpcAction(procId, args =>
            {
                if (args.Length < 4)
                    throw new ArgumentException(
                        "Incompatibile number of arguments provided to registered action. Procedure uri: " + procId);

                if (!(args[0] is T1)) args[0] = self.TypeResolver(args[0], args[0].GetType(), typeof(T1));
                if (!(args[1] is T2)) args[1] = self.TypeResolver(args[1], args[1].GetType(), typeof(T2));
                if (!(args[2] is T3)) args[2] = self.TypeResolver(args[2], args[2].GetType(), typeof(T3));
                if (!(args[3] is T4)) args[3] = self.TypeResolver(args[3], args[3].GetType(), typeof(T4));

                return func.DynamicInvoke(args);
            });
        }

        #endregion
    }
}