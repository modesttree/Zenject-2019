using System;
namespace Zenject
{
    public static class SignalExtensions
    {
        public static SignalDeclarationBindInfo CreateDefaultSignalDeclarationBindInfo(DiContainer container, Type signalType)
        {
            return new SignalDeclarationBindInfo(signalType)
            {
                RunAsync = container.Settings.Signals.DefaultSyncMode == SignalDefaultSyncModes.Asynchronous,
                MissingHandlerResponse = container.Settings.Signals.MissingHandlerDefaultResponse,
                TickPriority = container.Settings.Signals.DefaultAsyncTickPriority
            };
        }

        public static DeclareSignalIdRequireHandlerAsyncTickPriorityCopyBinder DeclareSignal(this DiContainer container, Type type)
        {
            var signalBindInfo = CreateDefaultSignalDeclarationBindInfo(container, type);

            var bindInfo = container.Bind<SignalDeclaration>().AsCached()
                .WithArguments(signalBindInfo).WhenInjectedInto(typeof(SignalBus), typeof(SignalDeclarationAsyncInitializer)).BindInfo;

            var signalBinder = new DeclareSignalIdRequireHandlerAsyncTickPriorityCopyBinder(signalBindInfo);
            signalBinder.AddCopyBindInfo(bindInfo);
            return signalBinder;
        }

        public static DeclareSignalIdRequireHandlerAsyncTickPriorityCopyBinder DeclareSignal<TSignal>(this DiContainer container)
        {
            return container.DeclareSignal(typeof(TSignal));
        }

        public static BindSignalIdToBinder<TSignal> BindSignal<TSignal>(this DiContainer container)
        {
            var signalBindInfo = new SignalBindingBindInfo(typeof(TSignal));

            return new BindSignalIdToBinder<TSignal>(container, signalBindInfo);
        }
    }
}

