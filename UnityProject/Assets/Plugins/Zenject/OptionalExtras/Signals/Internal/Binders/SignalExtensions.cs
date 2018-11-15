namespace Zenject
{
    public static class SignalExtensions
    {
        public static DeclareSignalIdRequireHandlerAsyncTickPriorityCopyBinder DeclareSignal<TSignal>(this DiContainer container)
        {
            var signalBindInfo = new SignalDeclarationBindInfo(typeof(TSignal));

            signalBindInfo.RunAsync = container.Settings.Signals.DefaultSyncMode == SignalDefaultSyncModes.Asynchronous;
            signalBindInfo.MissingHandlerResponse = container.Settings.Signals.MissingHandlerDefaultResponse;
            signalBindInfo.TickPriority = container.Settings.Signals.DefaultAsyncTickPriority;

            var bindInfo = container.Bind<SignalDeclaration>().AsCached()
                .WithArguments(signalBindInfo).WhenInjectedInto(typeof(SignalBus), typeof(SignalDeclarationAsyncInitializer)).BindInfo;

            var signalBinder = new DeclareSignalIdRequireHandlerAsyncTickPriorityCopyBinder(signalBindInfo);
            signalBinder.AddCopyBindInfo(bindInfo);
            return signalBinder;
        }

        public static BindSignalIdToBinder<TSignal> BindSignal<TSignal>(this DiContainer container)
        {
            var signalBindInfo = new SignalBindingBindInfo(typeof(TSignal));

            return new BindSignalIdToBinder<TSignal>(container, signalBindInfo);
        }
    }
}
