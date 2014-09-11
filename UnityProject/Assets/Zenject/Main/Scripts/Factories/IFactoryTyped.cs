namespace ModestTree.Zenject.Api.Factories
{
    /// <summary>
    ///     Four parameters
    /// </summary>
    public interface IFactoryTyped<in TParam1, in TParam2, in TParam3, in TParam4, out TValue> : IValidatable
    {
        TValue Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4);
    }

    /// <summary>
    ///     Three parameters
    /// </summary>
    public interface IFactoryTyped<in TParam1, in TParam2, in TParam3, out TValue> : IValidatable
    {
        TValue Create(TParam1 param1, TParam2 param2, TParam3 param3);
    }

    /// <summary>
    ///     Two parameters
    /// </summary>
    public interface IFactoryTyped<in TParam1, in TParam2, out TValue> : IValidatable
    {
        TValue Create(TParam1 param1, TParam2 param2);
    }

    /// <summary>
    ///     One parameter
    /// </summary>
    public interface IFactoryTyped<in TParam1, out TValue> : IValidatable
    {
        TValue Create(TParam1 param);
    }

    /// <summary>
    ///     Zero parameters
    /// </summary>
    public interface IFactoryTyped<out TValue> : IValidatable
    {
        TValue Create();
    }
}