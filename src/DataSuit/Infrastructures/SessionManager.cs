using System;
using System.Collections.Concurrent;
using System.Threading;

namespace DataSuit.Infrastructures
{
    public class SessionManager : ISessionManager
    {
        private readonly ConcurrentDictionary<string, int> _cdInteger;

        public SessionManager() // maybe an id
        {
            _cdInteger = new ConcurrentDictionary<string, int>();
        }

        public int IncreaseInteger(string prop)
        {
            if (_cdInteger.TryGetValue(prop, out int intValue))
            {
                Interlocked.Increment(ref intValue);
                _cdInteger[prop] = intValue;

                return intValue;
            }
            else
            {
                if (!_cdInteger.TryAdd(prop, 1))
                {
                    throw new Exception("IncreaseInteger");
                }
                
                return 1;
            }
        }
    }
}