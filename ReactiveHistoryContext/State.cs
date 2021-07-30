using System;

namespace ReactiveHistory
{
    /// <summary>
    /// Undo/redo action pair.
    /// </summary>
    public struct State
    {
        /// <summary>
        /// The undo state action.
        /// </summary>
        public readonly Action Undo;

        /// <summary>
        /// The redo state action.
        /// </summary>
        public readonly Action Redo;

        /// <summary>
        /// Initializes a new <see cref="State"/> instance.
        /// </summary>
        /// <param name="undo">The undo state action.</param>
        /// <param name="redo">The redo state action.</param>
        /// <param name="undoName">The undo state name.</param>
        /// <param name="redoName">The redo state name.</param>
        public State(Action undo, Action redo)
        {
            Undo = undo;
            Redo = redo;
        }
    }
}
