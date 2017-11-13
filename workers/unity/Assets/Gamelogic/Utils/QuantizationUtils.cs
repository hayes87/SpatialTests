using Assets.Gamelogic.Core;

namespace Assets.Gamelogic.Utils
{
    public static class QuantizationUtils
    {
        public static uint QuantizeAngle(float angle)
        {
            return (uint)(angle *2f);
        }

        public static float DequantizeAngle(uint angle)
        {
            return angle /2;
        }
    }
}
