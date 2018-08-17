using System;
using System.Collections.Generic;
using ModestTree;
using System.Linq;

#if ZEN_SIGNALS_ADD_UNIRX
using UniRx;
#endif

namespace Zenject
{
    public class SignalBus : ILateDisposable
    {
        readonly SignalSubscription.Pool _subscriptionPool;
        readonly Dictionary<Type, SignalDeclaration> _localDeclarationMap;
        readonly List<SignalDeclaration> _localDeclarations;
        readonly SignalBus _parentBus;
        readonly Dictionary<SignalSubscriptionId, SignalSubscription> _subscriptionMap = new Dictionary<SignalSubscriptionId, SignalSubscription>();
        readonly ZenjectSettings.SignalSettings _settings;

        public SignalBus(
            [Inject(Source = InjectSources.Local)]
            List<SignalDeclaration> signalDeclarations,
            [Inject(Source = InjectSources.Parent, Optional = true)]
            SignalBus parentBus,
            [InjectOptional]
            ZenjectSettings zenjectSettings,
            SignalSubscription.Pool subscriptionPool)
        {
            _subscriptionPool = subscriptionPool;
            zenjectSettings = zenjectSettings ?? ZenjectSettings.Default;
            _settings = zenjectSettings.Signals ?? ZenjectSettings.SignalSettings.Default;

            _localDeclarations = signalDeclarations;
            _localDeclarationMap = signalDeclarations.ToDictionary(x => x.SignalType, x => x);
            _parentBus = parentBus;
        }

        public SignalBus ParentBus
        {
            get { return _parentBus; }
        }

        public int NumSubscribers
        {
            get { return _subscriptionMap.Count; }
        }

        public void LateDispose()
        {
            if (_settings.RequireStrictUnsubscribe)
            {
                if (!_subscriptionMap.IsEmpty())
                {
                    throw Assert.CreateException(
                        "Found subscriptions for signals '{0}' in SignalBus.LateDispose!  Either add the explicit Unsubscribe or set SignalSettings.AutoUnsubscribeInDispose to true",
                        _subscriptionMap.Values.Select(x => x.SignalType.PrettyName()).Join(", "));
                }
            }
            else
            {
                foreach (var subscription in _subscriptionMap.Values)
                {
                    subscription.Dispose();
                }
            }

            for (int i = 0; i < _localDeclarations.Count; i++)
            {
                _localDeclarations[i].Dispose();
            }
        }

        public void Fire<TSignal>()
        {
            // Do this before creating the signal so that it throws if the signal was not declared
            var declaration = GetDeclaration(typeof(TSignal));

            declaration.Fire(
                (TSignal)Activator.CreateInstance(typeof(TSignal)));
        }

        public void Fire(object signal)
        {
            GetDeclaration(signal.GetType()).Fire(signal);
        }

        public void TryFire<TSignal>()
        {
            var declaration = GetDeclaration(typeof(TSignal), false);
            if (declaration != null)
            {
                declaration.Fire(
                    (TSignal)Activator.CreateInstance(typeof(TSignal)));
            }
        }

        public void TryFire(object signal)
        {
            var declaration = GetDeclaration(signal.GetType(), false);
            if (declaration != null)
            {
                declaration.Fire(signal);
            }
        }

#if ZEN_SIGNALS_ADD_UNIRX
        public IObservable<TSignal> GetStream<TSignal>()
        {
            return GetStream(typeof(TSignal)).Select(x => (TSignal)x);
        }

        public IObservable<object> GetStream(Type signalType)
        {
            return GetDeclaration(signalType).Stream;
        }
#endif

        public void Subscribe<TSignal>(Action callback)
        {
            Action<object> wrapperCallback = (args) => callback();
            SubscribeInternal(typeof(TSignal), callback, wrapperCallback);
        }

        public void Subscribe<TSignal>(Action<TSignal> callback)
        {
            Action<object> wrapperCallback = (args) => callback((TSignal)args);
            SubscribeInternal(typeof(TSignal), callback, wrapperCallback);
        }

        public void Subscribe(Type signalType, Action<object> callback)
        {
            SubscribeInternal(signalType, callback, callback);
        }

        public void Unsubscribe<TSignal>(Action callback)
        {
            Unsubscribe(typeof(TSignal), callback);
        }

        public void Unsubscribe(Type signalType, Action callback)
        {
            UnsubscribeInternal(signalType, callback, true);
        }

        public void Unsubscribe(Type signalType, Action<object> callback)
        {
            UnsubscribeInternal(signalType, callback, true);
        }

        public void Unsubscribe<TSignal>(Action<TSignal> callback)
        {
            UnsubscribeInternal(typeof(TSignal), callback, true);
        }

        public void TryUnsubscribe<TSignal>(Action callback)
        {
            UnsubscribeInternal(typeof(TSignal), callback, false);
        }

        public void TryUnsubscribe(Type signalType, Action callback)
        {
            UnsubscribeInternal(signalType, callback, false);
        }

        public void TryUnsubscribe(Type signalType, Action<object> callback)
        {
            UnsubscribeInternal(signalType, callback, false);
        }

        public void TryUnsubscribe<TSignal>(Action<TSignal> callback)
        {
            UnsubscribeInternal(typeof(TSignal), callback, false);
        }

        void UnsubscribeInternal(Type signalType, object token, bool throwIfMissing)
        {
            UnsubscribeInternal(
                new SignalSubscriptionId(signalType, token), throwIfMissing);
        }

        void UnsubscribeInternal(SignalSubscriptionId id, bool throwIfMissing)
        {
            SignalSubscription subscription;

            if (_subscriptionMap.TryGetValue(id, out subscription))
            {
                _subscriptionMap.RemoveWithConfirm(id);
                subscription.Dispose();
            }
            else
            {
                if (throwIfMissing)
                {
                    throw Assert.CreateException(
                        "Called unsubscribe for signal '{0}' but could not find corresponding subscribe.  If this is intentional, call TryUnsubscribe instead.");
                }
            }
        }

        void SubscribeInternal(Type signalType, object token, Action<object> callback)
        {
            SubscribeInternal(
                new SignalSubscriptionId(signalType, token), callback);
        }

        void SubscribeInternal(SignalSubscriptionId id, Action<object> callback)
        {
            Assert.That(!_subscriptionMap.ContainsKey(id),
                "Tried subscribing to the same signal with the same callback on Zenject.SignalBus");

            var declaration = GetDeclaration(id.SignalType);
            var subscription = _subscriptionPool.Spawn(callback, declaration);

            _subscriptionMap.Add(id, subscription);
        }

        SignalDeclaration GetDeclaration(Type signalType, bool requireDeclaration = true)
        {
            SignalDeclaration handler;

            if (_localDeclarationMap.TryGetValue(signalType, out handler))
            {
                return handler;
            }

            if (_parentBus != null)
            {
                return _parentBus.GetDeclaration(signalType, requireDeclaration);
            }

            if (requireDeclaration)
            {
                throw Assert.CreateException(
                    "Fired undeclared signal with type '{0}'!", signalType);
            }
            else
            {
                return null;
            }
        }
    }
}
