using System;

namespace CityRace.Core
{
    // Plain-geometry checks for the practice course, expressed as offsets from a checkpoint centre (metres, X/Z plane).
    public static class PracticeZones
    {
        // Roads are 6 m wide and enclosed by solid kerbs, so a generous gate cannot become a shortcut,
        // while riders hugging the inside of a corner (up to ~4.2 m from its centre) still register it.
        public const float GateRadius = 4.5f;
        // Matches the painted 3 x 3 m company square, plus a small allowance for the bike body.
        public const float FinishHalfExtent = 1.5f + 0.35f;

        public static bool ReachesGate(float dx, float dz) => dx * dx + dz * dz <= GateRadius * GateRadius;

        public static bool InsideFinish(float dx, float dz) =>
            Math.Abs(dx) <= FinishHalfExtent && Math.Abs(dz) <= FinishHalfExtent;
    }
}
