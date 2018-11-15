using System;
using System.Collections.Generic;

namespace Zenject
{
    public interface IPoolable
    {
        void OnDespawned();
        void OnSpawned();
    }

    public interface IPoolable<TParam1>
    {
        void OnDespawned();
        void OnSpawned(TParam1 p1);
    }

    public interface IPoolable<TParam1, TParam2>
    {
        void OnDespawned();
        void OnSpawned(TParam1 p1, TParam2 p2);
    }

    public interface IPoolable<TParam1, TParam2, TParam3>
    {
        void OnDespawned();
        void OnSpawned(TParam1 p1, TParam2 p2, TParam3 p3);
    }

    public interface IPoolable<TParam1, TParam2, TParam3, TParam4>
    {
        void OnDespawned();
        void OnSpawned(TParam1 p1, TParam2 p2, TParam3 p3, TParam4 p4);
    }

    public interface IPoolable<TParam1, TParam2, TParam3, TParam4, TParam5>
    {
        void OnDespawned();
        void OnSpawned(TParam1 p1, TParam2 p2, TParam3 p3, TParam4 p4, TParam5 p5);
    }

    public interface IPoolable<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>
    {
        void OnDespawned();
        void OnSpawned(TParam1 p1, TParam2 p2, TParam3 p3, TParam4 p4, TParam5 p5, TParam6 p6);
    }

    public interface IPoolable<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7>
    {
        void OnDespawned();
        void OnSpawned(TParam1 p1, TParam2 p2, TParam3 p3, TParam4 p4, TParam5 p5, TParam6 p6, TParam7 p7);
    }

    public interface IPoolable<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8>
    {
        void OnDespawned();
        void OnSpawned(TParam1 p1, TParam2 p2, TParam3 p3, TParam4 p4, TParam5 p5, TParam6 p6, TParam7 p7, TParam8 p8);
    }

    public interface IPoolable<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9>
    {
        void OnDespawned();
        void OnSpawned(TParam1 p1, TParam2 p2, TParam3 p3, TParam4 p4, TParam5 p5, TParam6 p6, TParam7 p7, TParam8 p8, TParam9 p9);
    }

    public interface IPoolable<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10>
    {
        void OnDespawned();
        void OnSpawned(TParam1 p1, TParam2 p2, TParam3 p3, TParam4 p4, TParam5 p5, TParam6 p6, TParam7 p7, TParam8 p8, TParam9 p9, TParam10 p10);
    }

    public interface IPoolable<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11>
    {
        void OnDespawned();
        void OnSpawned(TParam1 p1, TParam2 p2, TParam3 p3, TParam4 p4, TParam5 p5, TParam6 p6, TParam7 p7, TParam8 p8, TParam9 p9, TParam10 p10, TParam11 p11);
    }
}
