using NUnit.Framework;
using Client.Util;

namespace Scenes.Tests.Client.Util
{
    public class ShipDamageCalculationUtilTest
    {
        private ShipDamageCalculationUtil _util = new ShipDamageCalculationUtil();
        
        [Test]
        public void test_ShouldReturnResultDamage()
        {
            float maxPossibleDamage = 98;
            float maxSpeed = 90;

            float firstSpeed = 45;
            float secondSpeed = 60;
            float thirdSpeed = 89;

            var firstDamage = _util.CalculateDamage(firstSpeed, maxSpeed, maxPossibleDamage);
            var secondDamage = _util.CalculateDamage(secondSpeed, maxSpeed, maxPossibleDamage);
            var thirdDamage = _util.CalculateDamage(thirdSpeed, maxSpeed, maxPossibleDamage);
            
            Assert.True(secondDamage >= firstDamage);
            Assert.True(thirdDamage >= secondDamage);
        }
    }
}