using System;
using Core;

namespace Client.Core
{
    public class ClientEventStorage: IDisposable
    {
        private static ClientEventStorage _instance;

        public readonly CoreEvent DockingAvailable = new CoreEvent(); //green
        public readonly CoreEvent DockableUnitsInRange = new CoreEvent(); //yellow
        public readonly CoreEvent IsDocked = new CoreEvent(); //blue
        public readonly CoreEvent NoOneToDock = new CoreEvent(); //clear
        public readonly CoreEvent DockIndicatorStateRequest = new CoreEvent();

        public static ClientEventStorage GetInstance()
        {
            return _instance ??= new ClientEventStorage();
        }
        
        public void Dispose()
        {
            DockingAvailable.RemoveAllListeners();
            DockableUnitsInRange.RemoveAllListeners();
            IsDocked.RemoveAllListeners();
            NoOneToDock.RemoveAllListeners();
            DockIndicatorStateRequest.RemoveAllListeners();
        }
    }
}