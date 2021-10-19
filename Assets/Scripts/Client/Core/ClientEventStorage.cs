using System;
using Core;

namespace Client.Core
{
    public class ClientEventStorage: IDisposable
    {
        private static ClientEventStorage _instance;

        public CoreEvent DockingAvailable = new CoreEvent(); //green
        public CoreEvent DockableUnitsInRange = new CoreEvent(); //yellow
        public CoreEvent IsDocked = new CoreEvent(); //blue
        public CoreEvent NoOneToDock = new CoreEvent(); //clear

        public static ClientEventStorage GetInstance()
        {
            return _instance ?? (_instance = new ClientEventStorage());
        }
        
        public void Dispose()
        {
            DockingAvailable.RemoveAllListeners();
            DockableUnitsInRange.RemoveAllListeners();
            IsDocked.RemoveAllListeners();
            NoOneToDock.RemoveAllListeners();
        }
    }
}