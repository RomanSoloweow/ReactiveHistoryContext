using System;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;

namespace ReactiveHistory
{
    public class ReactiveHistoryContext:IDisposable
    {
        public IHistory History { get; }
        public ReactiveHistoryContext(IHistory history)
        {
            History = history;
            
            var CanUndo = CanSnapshot
                .CombineLatest(history.CanUndo, (recordable, executable) => recordable && executable);

            Undo = ReactiveCommand.Create(history.Undo, CanUndo);

            var CanRedo = CanSnapshot
                .CombineLatest(history.CanRedo, (recordable, executable) => recordable && executable);

            Redo = ReactiveCommand.Create(history.Redo, CanRedo);
            Clear = ReactiveCommand.Create(history.Clear, history.CanClear);
        }
        
        public IObservable<bool> CanSnapshot => History.CanSnapshot;
        public ReactiveCommand<Unit, Unit> Undo { get; }
        
        public ReactiveCommand<Unit, Unit> Redo { get; }
        
        public ReactiveCommand<Unit, Unit> Clear { get; }
        

        public virtual void Dispose()
        {
            Undo?.Dispose();
            Redo?.Dispose();
            Clear?.Dispose();
        }
    }
}
