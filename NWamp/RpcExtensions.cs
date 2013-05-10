using System;
using NWamp.Protocol.Rpc;

namespace NWamp
{
    /// <summary>
    /// Collection of extension methods used for <see cref="IRpcHandler"/>.
    /// </summary>
    public static class RpcExtensions
    {
        #region Actions

        public static void RegisterAction(this IRpcHandler self, string procId, Action action)
        {
            self.RegisterRpcAction(procId, _ =>
            {
                action();
                return null;
            });
        }

        public static void RegisterAction<T1>(this IRpcHandler self, string procId, Action<T1> action)
        {
            self.RegisterRpcAction(procId, args =>
            {
                if (args.Length < 1)
                    throw new ArgumentException(
                        "Incompatibile number of arguments provided to registered action. Procedure uri: " + procId);

                if (!(args[0] is T1))
                    throw new ArgumentException(
                        "One or more of the arguments provided are not compatibile with registered RPC action. Procedure Uri: " +
                        procId);

                var a = (T1)args[0];
                action(a);
                return null;
            });
        }

        public static void RegisterAction<T1, T2>(this IRpcHandler self, string procId, Action<T1, T2> action)
        {
            self.RegisterRpcAction(procId, args =>
            {
                if (args.Length < 2)
                    throw new ArgumentException(
                        "Incompatibile number of arguments provided to registered action. Procedure uri: " + procId);

                if (!(args[0] is T1 && args[1] is T2))
                    throw new ArgumentException(
                        "One or more of the arguments provided are not compatibile with registered RPC action. Procedure Uri: " +
                        procId);

                action((T1)args[0], (T2)args[1]);
                return null;
            });
        }

        public static void RegisterAction<T1, T2, T3>(this IRpcHandler self, string procId, Action<T1, T2, T3> action)
        {
            self.RegisterRpcAction(procId, args =>
            {
                if (args.Length < 3)
                    throw new ArgumentException(
                        "Incompatibile number of arguments provided to registered action. Procedure uri: " + procId);

                if (!(args[0] is T1 && args[1] is T2 && args[2] is T3))
                    throw new ArgumentException(
                        "One or more of the arguments provided are not compatibile with registered RPC action. Procedure Uri: " +
                        procId);

                action((T1)args[0], (T2)args[1], (T3)args[2]);
                return null;
            });
        }

        public static void RegisterAction<T1, T2, T3, T4>(this IRpcHandler self, string procId, Action<T1, T2, T3, T4> action)
        {
            self.RegisterRpcAction(procId, args =>
            {
                if (args.Length < 4)
                    throw new ArgumentException(
                        "Incompatibile number of arguments provided to registered action. Procedure uri: " + procId);

                if (!(args[0] is T1 && args[1] is T2 && args[2] is T3 && args[3] is T4))
                    throw new ArgumentException(
                        "One or more of the arguments provided are not compatibile with registered RPC action. Procedure Uri: " +
                        procId);

                action((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3]);
                return null;
            });
        }

        #endregion

        #region Functions

        public static void RegisterFunc<TResult>(this IRpcHandler self, string procId, Func<TResult> func)
        {
            self.RegisterRpcAction(procId, _ => func());
        }

        public static void RegisterFunc<T1, TResult>(this IRpcHandler self, string procId, Func<T1, TResult> func)
        {
            self.RegisterRpcAction(procId, args =>
            {
                if (args.Length < 1)
                    throw new ArgumentException(
                        "Incompatibile number of arguments provided to registered action. Procedure uri: " + procId);

                if (!(args[0] is T1))
                    throw new ArgumentException(string.Format(
                        "One or more of the arguments provided are not compatibile with registered RPC action.\n Procedure Uri: {0}.\n Desired type: {1}.\n Argument type: {2}", procId, typeof(T1).ToString(), args[0].GetType().ToString()));

                return func((T1)args[0]);
            });
        }

        public static void RegisterFunc<T1, T2, TResult>(this IRpcHandler self, string procId, Func<T1, T2, TResult> func)
        {
            self.RegisterRpcAction(procId, args =>
            {
                if (args.Length < 2)
                    throw new ArgumentException(
                        "Incompatibile number of arguments provided to registered action. Procedure uri: " + procId);

                if (!(args[0] is T1 && args[1] is T2))
                    throw new ArgumentException(
                        "One or more of the arguments provided are not compatibile with registered RPC action. Procedure Uri: " +
                        procId);

                return func((T1)args[0], (T2)args[1]);
            });
        }

        public static void RegisterFunc<T1, T2, T3, TResult>(this IRpcHandler self, string procId, Func<T1, T2, T3, TResult> func)
        {
            self.RegisterRpcAction(procId, args =>
            {
                if (args.Length < 3)
                    throw new ArgumentException(
                        "Incompatibile number of arguments provided to registered action. Procedure uri: " + procId);

                if (!(args[0] is T1 && args[1] is T2 && args[2] is T3))
                    throw new ArgumentException(
                        "One or more of the arguments provided are not compatibile with registered RPC action. Procedure Uri: " +
                        procId);

                return func((T1)args[0], (T2)args[1], (T3)args[2]);
            });
        }

        public static void RegisterFunc<T1, T2, T3, T4, TResult>(this IRpcHandler self, string procId, Func<T1, T2, T3, T4, TResult> func)
        {
            self.RegisterRpcAction(procId, args =>
            {
                if (args.Length < 4)
                    throw new ArgumentException(
                        "Incompatibile number of arguments provided to registered action. Procedure uri: " + procId);

                if (!(args[0] is T1 && args[1] is T2 && args[2] is T3 && args[3] is T4))
                    throw new ArgumentException(
                        "One or more of the arguments provided are not compatibile with registered RPC action. Procedure Uri: " +
                        procId);

                return func((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3]);
            });
        }

        #endregion
    }
}