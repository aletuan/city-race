using System;

namespace CityRace.Core
{
    // Offline practice rules; no Unity state, clocks or scene lookups.
    public sealed class PracticeProgress
    {
        public int NextCheckpoint { get; private set; } = 1;
        public bool Finished { get; private set; }
        public float ElapsedSeconds { get; private set; }
        public int Recoveries { get; private set; }
        private readonly int _count;
        private float _stopSeconds;

        public PracticeProgress(int checkpointCount)
        {
            if (checkpointCount < 2) { throw new ArgumentOutOfRangeException(nameof(checkpointCount)); }
            _count = checkpointCount;
        }

        public void Tick(float deltaSeconds, bool insideNextCheckpoint, float speedMetersPerSecond)
        {
            if (Finished || deltaSeconds <= 0f) { return; }
            ElapsedSeconds += deltaSeconds;
            if (NextCheckpoint < _count - 1)
            {
                if (insideNextCheckpoint) { NextCheckpoint++; }
                return;
            }
            _stopSeconds = insideNextCheckpoint && speedMetersPerSecond < 0.4f ? _stopSeconds + deltaSeconds : 0f;
            Finished = _stopSeconds >= 0.7f;
        }

        public int Recover(int maximumCheckpoint = int.MaxValue)
        {
            if (Finished) { return _count - 1; }
            // Return to an already passed point, never advance route progress.
            var checkpoint = Math.Max(0, Math.Min(NextCheckpoint - 2, maximumCheckpoint));
            NextCheckpoint = checkpoint + 1;
            _stopSeconds = 0f;
            Recoveries++;
            ElapsedSeconds += 2f;
            return checkpoint;
        }
    }
}
