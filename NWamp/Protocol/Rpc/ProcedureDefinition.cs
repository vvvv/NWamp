using System;

namespace NWamp.Protocol.Rpc
{
    /// <summary>
    /// Class defining remote procedure registered in <see cref="IRpcHandler"/>.
    /// </summary>
    public class ProcedureDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcedureDefinition"/> class.
        /// </summary>
        public ProcedureDefinition(string procUri, Func<object[],object> procedure)
        {
            this.ProcUri = procUri;
            this.Procedure = procedure;
        }

        /// <summary>
        /// Gets URI identifier of the remote procedure.
        /// </summary>
        public string ProcUri { get; protected set; }

        /// <summary>
        /// Gets function handler of the remote procedure.
        /// </summary>
        public Func<object[], object> Procedure { get; protected set; }
    }
}
