namespace Client.Util
{
    public class ShipDamageCalculationUtil
    { 
        public float CalculateDamage(float speed, float maxSpeed, float maxPossibleDamageHp)
        {
            var oldSpeed = speed;
            var oldMinSpeed = 0;
            var oldMaxSpeed = maxSpeed;
            var minPossibleDamageHp = 0;
            
            var speedRange = oldMaxSpeed - oldMinSpeed;
            var possibleDamageHpRange = maxPossibleDamageHp - minPossibleDamageHp;
            var damagePercentage = (oldSpeed - oldMinSpeed) * possibleDamageHpRange / speedRange + minPossibleDamageHp;

            return damagePercentage * speed / 100;
        }
    }
}