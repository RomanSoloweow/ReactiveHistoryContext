using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ReactiveHistory
{
    /// <summary>
    /// Undo/redo stack based action history.
    /// </summary>
    public class StackHistory : IHistory, IDisposable
    {
        private readonly Subject<bool> _canUndo = new();
        private readonly Subject<bool> _canRedo = new();
        private readonly Subject<bool> _canClear = new();
        private readonly Subject<bool> _canSnapshot  = new();

        /// <summary>
        /// Gets or sets undo states stack.
        /// </summary>
        public Stack<State> Undos { get; set; } = new();

        /// <summary>
        /// Gets or sets redo states stack.
        /// </summary>
        public Stack<State> Redos { get; set; } = new();
        
        /// <inheritdoc/>
        public IObservable<bool> CanUndo => _canUndo.AsObservable().DistinctUntilChanged();

        /// <inheritdoc/>
        public IObservable<bool> CanRedo => _canRedo.AsObservable().DistinctUntilChanged();

        /// <inheritdoc/>
        public IObservable<bool> CanClear => CanClear.AsObservable().DistinctUntilChanged();

        /// <inheritdoc/>
        public IObservable<bool> CanSnapshot => CanSnapshot.AsObservable().DistinctUntilChanged();
        
        /// <inheritdoc/>
        public void Snapshot(Action undo, Action redo)
        {
            if (undo == null)
                throw new ArgumentNullException(nameof(undo));

            if (redo == null)
                throw new ArgumentNullException(nameof(redo));
            
            UpdateSubjects(true);
            Redos.Clear();
            Undos.Push(new State(undo, redo));
            UpdateSubjects();
        }

        /// <inheritdoc/>
        public void Undo()
        {
            if (Undos.Count == 0)
                throw new Exception();

            UpdateSubjects(true);
            var state = Undos.Pop();
            state.Undo.Invoke();
            Redos.Push(state);
            UpdateSubjects();
        }

        /// <inheritdoc/>
        public void Redo()
        {
            if (Redos.Count == 0)
                throw new Exception();
            
            UpdateSubjects(true);
            var state = Redos.Pop();
            state.Redo.Invoke();
            Undos.Push(state);
            UpdateSubjects();
        }

        /// <inheritdoc/>
        public void Clear()
        {
            UpdateSubjects(true);
            Undos.Clear();
            Redos.Clear();
            UpdateSubjects();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Undos.Clear();
            Redos.Clear();
            _canUndo.Dispose();
            _canRedo.Dispose();
            _canClear.Dispose();
            _canSnapshot.Dispose();
        }
        
        private void UpdateSubjects(bool disableAll = false)
        {
            if (disableAll)
            {
                _canUndo.OnNext(false);
                _canRedo.OnNext(false);
                _canClear.OnNext(false);
                _canSnapshot.OnNext(false);
            }
            else
            {
                var hasUndoEntries = Undos.Any(); 
                var hasRedoEntries = Redos.Any();
                
                _canUndo.OnNext(hasUndoEntries);
                _canRedo.OnNext(hasRedoEntries);
                _canClear.OnNext(hasUndoEntries || hasRedoEntries);
                _canSnapshot.OnNext(true);
            }
        }
    }
}
