namespace CityRace.Core
{
    public static class PotholeResponse
    {
        public const float SafeSpeedMetersPerSecond = 3f;
        public const float RetainedSpeedFraction = 0.6f;
        public const float SlowSeconds = 0.45f;
        public const float ProtectionSeconds = 1.2f;

        public static bool ShouldSlow(float speedMetersPerSecond, float protectionSeconds)
        {
            return speedMetersPerSecond > SafeSpeedMetersPerSecond && protectionSeconds <= 0f;
        }
    }
}
