using System.Collections;
using CityRace.Core;
using CityRace.Gameplay.Riding;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace CityRace.Tests
{
    public sealed class PotholeTests
    {
        private GameObject _bike;
        private GameObject _hole;
        private BikeMotor _motor;
        private Rigidbody _body;

        [SetUp]
        public void SetUp()
        {
            _bike = new GameObject("Pothole test bike");
            _body = _bike.AddComponent<Rigidbody>();
            _body.useGravity = false;
            _body.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            _bike.AddComponent<BoxCollider>().size = new Vector3(.65f,.7f,1.4f);
            _motor = _bike.AddComponent<BikeMotor>();
            _hole = new GameObject("Test pothole");
            _hole.transform.position = new Vector3(0,0,6);
            var trigger = _hole.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.size = new Vector3(1.35f,1f,.9f);
            _hole.AddComponent<Pothole>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_bike);
            Object.DestroyImmediate(_hole);
        }

        [Test]
        public void ThresholdAndProtectionHaveExplicitBoundaries()
        {
            Assert.That(PotholeResponse.ShouldSlow(3f,0f), Is.False);
            Assert.That(PotholeResponse.ShouldSlow(3.01f,0f), Is.True);
            Assert.That(PotholeResponse.ShouldSlow(8f,.01f), Is.False);
        }

        [UnityTest]
        public IEnumerator FastCrossingSlowsOnceButAllowsSteeringAndRecovers()
        {
            _motor.SetCommand(Vector2.up);
            for (var i = 0; i < 150 && _motor.PotholeHits == 0; i++) { yield return new WaitForFixedUpdate(); }
            Assert.That(_motor.PotholeHits, Is.EqualTo(1), "Real trigger entry must apply the impact.");
            Assert.That(_motor.Speed, Is.InRange(2f,5f));
            Assert.That(_motor.TryHitPothole(), Is.False, "Overlapping contacts must not compound the penalty.");
            var yaw = _body.rotation;
            _motor.SetCommand(new Vector2(.5f,1f));
            for (var i = 0; i < 8; i++) { yield return new WaitForFixedUpdate(); }
            Assert.That(Quaternion.Angle(yaw,_body.rotation), Is.GreaterThan(5f));
            Assert.That(_motor.PotholeWobble, Is.GreaterThan(0f));
            _motor.SetCommand(Vector2.up);
            for (var i = 0; i < 100; i++) { yield return new WaitForFixedUpdate(); }
            Assert.That(_motor.Speed, Is.GreaterThan(7.5f));
            Assert.That(_motor.PotholeWobble, Is.EqualTo(0f));
            Assert.That(_motor.PotholeHits, Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator SlowCrossingHasNoPenalty()
        {
            _motor.SetCommand(Vector2.up * .25f);
            for (var i = 0; i < 250; i++) { yield return new WaitForFixedUpdate(); }
            Assert.That(_body.position.z, Is.GreaterThan(8f));
            Assert.That(_motor.PotholeHits, Is.Zero);
            Assert.That(_motor.Speed, Is.EqualTo(2f).Within(.01f));
        }

        [UnityTest]
        public IEnumerator RidingBesideHoleAvoidsPenalty()
        {
            _motor.ResetPose(new Vector3(2,0,0),Quaternion.identity);
            _motor.SetCommand(Vector2.up);
            for (var i = 0; i < 150; i++) { yield return new WaitForFixedUpdate(); }
            Assert.That(_body.position.z, Is.GreaterThan(8f));
            Assert.That(_motor.PotholeHits, Is.Zero);
            Assert.That(_motor.Speed, Is.GreaterThan(7.5f));
        }

        [UnityTest]
        public IEnumerator ResetClearsImpactAndRequiresNewThrottle()
        {
            _motor.SetCommand(Vector2.up);
            for (var i = 0; i < 150 && _motor.PotholeHits == 0; i++) { yield return new WaitForFixedUpdate(); }
            Assert.That(_motor.PotholeHits, Is.EqualTo(1));
            _motor.ResetPose(Vector3.zero,Quaternion.identity);
            yield return new WaitForFixedUpdate();
            Assert.That(_motor.Speed, Is.Zero);
            Assert.That(_motor.PotholeWobble, Is.Zero);
            Assert.That(_motor.PotholeFeedbackSeconds, Is.Zero);
            Assert.That(_motor.PotholeHits, Is.Zero);
        }
    }
}
