using System;
using Core;

namespace Net.Core
{
    public class NetEventStorage: IDisposable
    {
        private static NetEventStorage _instance;
        public IntEvent worldInit = new IntEvent();


        public static NetEventStorage GetInstance()
        {
            return _instance ?? (_instance = new NetEventStorage());
        }

        public void Dispose()
        {
            worldInit.RemoveAllListeners();
        }
    }
}
