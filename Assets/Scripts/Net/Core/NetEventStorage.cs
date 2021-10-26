using System;
using Core;

namespace Net.Core
{
    public class NetEventStorage: IDisposable
    {
        private static NetEventStorage _instance;
        public readonly IntEvent WorldInit = new IntEvent();


        public static NetEventStorage GetInstance()
        {
            return _instance ??= new NetEventStorage();
        }

        public void Dispose()
        {
            WorldInit.RemoveAllListeners();
        }
    }
}
