using System;
using System.Collections.Generic;

namespace Zenject
{
    // Zero parameters
    public class Factory<TValue> : PlaceholderFactory<TValue>, IFactory<TValue>
    {
        // If you were hoping to override this method, use BindFactory<>.ToFactory instead
        public TValue Create()
        {
            return CreateInternal(new List<TypeValuePair>());
        }

        protected sealed override IEnumerable<Type> ParamTypes
        {
            get { yield break; }
        }
    }

    // One parameter
    public class Factory<TParam1, TValue>
        : PlaceholderFactory<TValue>, IFactory<TParam1, TValue>
    {
        // If you were hoping to override this method, use BindFactory<>.ToFactory instead
        public TValue Create(TParam1 param)
        {
            return CreateInternal(
                new List<TypeValuePair>()
                {
                    new TypeValuePair(typeof(TParam1), param),
                });
        }

        protected sealed override IEnumerable<Type> ParamTypes
        {
            get { yield return typeof(TParam1); }
        }
    }

    // Two parameters
    public class Factory<TParam1, TParam2, TValue>
        : PlaceholderFactory<TValue>, IFactory<TParam1, TParam2, TValue>
    {
        // If you were hoping to override this method, use BindFactory<>.ToFactory instead
        public TValue Create(TParam1 param1, TParam2 param2)
        {
            return CreateInternal(
                new List<TypeValuePair>()
                {
                    new TypeValuePair(typeof(TParam1), param1),
                    new TypeValuePair(typeof(TParam2), param2),
                });
        }

        protected sealed override IEnumerable<Type> ParamTypes
        {
            get
            {
                yield return typeof(TParam1);
                yield return typeof(TParam2);
            }
        }
    }

    // Three parameters
    public class Factory<TParam1, TParam2, TParam3, TValue>
        : PlaceholderFactory<TValue>, IFactory<TParam1, TParam2, TParam3, TValue>
    {
        // If you were hoping to override this method, use BindFactory<>.ToFactory instead
        public TValue Create(TParam1 param1, TParam2 param2, TParam3 param3)
        {
            return CreateInternal(
                new List<TypeValuePair>()
                {
                    new TypeValuePair(typeof(TParam1), param1),
                    new TypeValuePair(typeof(TParam2), param2),
                    new TypeValuePair(typeof(TParam3), param3),
                });
        }

        protected sealed override IEnumerable<Type> ParamTypes
        {
            get
            {
                yield return typeof(TParam1);
                yield return typeof(TParam2);
                yield return typeof(TParam3);
            }
        }
    }

    // Four parameters
    public class Factory<TParam1, TParam2, TParam3, TParam4, TValue>
        : PlaceholderFactory<TValue>, IFactory<TParam1, TParam2, TParam3, TParam4, TValue>
    {
        // If you were hoping to override this method, use BindFactory<>.ToFactory instead
        public TValue Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
        {
            return CreateInternal(
                new List<TypeValuePair>()
                {
                    new TypeValuePair(typeof(TParam1), param1),
                    new TypeValuePair(typeof(TParam2), param2),
                    new TypeValuePair(typeof(TParam3), param3),
                    new TypeValuePair(typeof(TParam4), param4),
                });
        }

        protected sealed override IEnumerable<Type> ParamTypes
        {
            get
            {
                yield return typeof(TParam1);
                yield return typeof(TParam2);
                yield return typeof(TParam3);
                yield return typeof(TParam4);
            }
        }
    }

    // Five parameters
    public class Factory<TParam1, TParam2, TParam3, TParam4, TParam5, TValue>
        : PlaceholderFactory<TValue>, IFactory<TParam1, TParam2, TParam3, TParam4, TParam5, TValue>
    {
        // If you were hoping to override this method, use BindFactory<>.ToFactory instead
        public TValue Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5)
        {
            return CreateInternal(
                new List<TypeValuePair>()
                {
                    new TypeValuePair(typeof(TParam1), param1),
                    new TypeValuePair(typeof(TParam2), param2),
                    new TypeValuePair(typeof(TParam3), param3),
                    new TypeValuePair(typeof(TParam4), param4),
                    new TypeValuePair(typeof(TParam5), param5),
                });
        }

        protected sealed override IEnumerable<Type> ParamTypes
        {
            get
            {
                yield return typeof(TParam1);
                yield return typeof(TParam2);
                yield return typeof(TParam3);
                yield return typeof(TParam4);
                yield return typeof(TParam5);
            }
        }
    }
}
