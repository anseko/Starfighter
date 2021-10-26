using System;

namespace Core
{
    public class CoreEventStorage: IDisposable
    {
        private static CoreEventStorage _instance;

        public readonly AxisValueEvent AxisValueChanged = new AxisValueEvent();
        public readonly KeyCodeEvent ActionKeyPressed = new KeyCodeEvent(); 
        
        public static CoreEventStorage GetInstance()
        {
            return _instance ??= new CoreEventStorage();
        }
        
        public void Dispose()
        {
            
        }
    }
}