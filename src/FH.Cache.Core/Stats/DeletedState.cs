using System;
using System.Collections.Generic;
using System.Text;

namespace FH.Cache.Core.Stats
{
    public class DeletedState : IState
    {
        /// <summary>
        /// Represents the name of the <i>Deleted</i> state. This field is read-only.
        /// </summary>
        /// <remarks>
        /// The value of this field is <c>"Deleted"</c>.
        /// </remarks>
        public static readonly string StateName = "Deleted";

        /// <summary>
        /// Initializes a new instance of the <see cref="DeletedState"/> class.
        /// </summary>
        public DeletedState()
        {
            DeletedAt = DateTime.UtcNow;
        }

        /// <inheritdoc />
        /// <remarks>
        /// Always equals to <see cref="StateName"/> for the <see cref="DeletedState"/>.
        /// Please see the remarks section of the <see cref="IState.Name">IState.Name</see>
        /// article for the details.
        /// </remarks>
        public string Name => StateName;

        /// <inheritdoc />
        public string Reason { get; set; }

        /// <inheritdoc />
        /// <remarks>
        /// Always returns <see langword="true"/> for the <see cref="DeletedState"/>.
        /// Please refer to the <see cref="IState.IsFinal">IState.IsFinal</see> documentation
        /// for the details.
        /// </remarks>
        public bool IsFinal => true;


        /// <summary>
        /// Gets a date/time when the current state instance was created.
        /// </summary>
        public DateTime DeletedAt { get; }

 
    }
}
