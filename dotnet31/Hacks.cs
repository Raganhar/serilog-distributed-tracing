
using System;
using System.Collections.Generic;

class NullObserver : IObserver<KeyValuePair<string, object>>
{
    public void OnCompleted()
    {
    }

    public void OnError(Exception error)
    {
    }

    public void OnNext(KeyValuePair<string, object> value)
    {
    }
}