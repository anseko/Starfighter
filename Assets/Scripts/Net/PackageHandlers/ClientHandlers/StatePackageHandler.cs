using System;
using Net.Interfaces;
using Net.Packages;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

namespace Net.PackageHandlers.ClientHandlers
{
    public class StatePackageHandler : IPackageHandler
    {
        public async Task Handle(AbstractPackage pack)
        {
            var statePack = pack as StatePackage;
            try
            {
            }
            catch (Exception ex)
            {
                Debug.unityLogger.LogException(ex);
                return;
            }
        }
    }
}