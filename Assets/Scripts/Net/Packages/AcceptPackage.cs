﻿using System;
using Net.PackageData;
using Net.Utils;

namespace Net.Packages
{
    [Serializable]
    public class AcceptPackage : AbstractPackage
    {
        public new AcceptData data
        {
            get => base.data as AcceptData; 
            private set => base.data = value;
        }
        public AcceptPackage(AcceptData data) : base(data, PackageType.AcceptPackage)
        { }
    }
}