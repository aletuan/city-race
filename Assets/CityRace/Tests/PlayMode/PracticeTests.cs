using System.Collections;
using CityRace.Core;
using CityRace.Gameplay.Riding;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;
using UnityEngine.TestTools;

namespace CityRace.Tests
{
    public sealed class PracticeTests
    {
        private Scene _originalScene;
        private float _timeScale;
        private Mouse _mouse;
        private InputSettings.BackgroundBehavior _background;
        private InputSettings.EditorInputBehaviorInPlayMode _editorInput;
        [SetUp] public void SetUp()
        {
            _originalScene = SceneManager.GetActiveScene();
            _mouse = null;
            _timeScale = Time.timeScale;
            _background = InputSystem.settings.backgroundBehavior;
            _editorInput = InputSystem.settings.editorInputBehaviorInPlayMode;
            InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
            InputSystem.settings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
        }
        [UnityTearDown] public IEnumerator TearDown()
        {
            Time.timeScale = _timeScale;
            if (_mouse != null) { InputSystem.RemoveDevice(_mouse); }
            InputSystem.settings.backgroundBehavior = _background;
            InputSystem.settings.editorInputBehaviorInPlayMode = _editorInput;
            var scene = SceneManager.GetSceneByName("Practice");
            if (scene.IsValid())
            {
                SceneManager.SetActiveScene(_originalScene);
                yield return SceneManager.UnloadSceneAsync(scene);
            }
        }

        [Test]
        public void FinishRequiresAllGatesAndContinuousLowSpeedStop()
        {
            var progress = new PracticeProgress(3);
            progress.Tick(1f, false, 0f);
            Assert.That(progress.Finished, Is.False);
            progress.Tick(.02f, true, 4f);
            progress.Tick(1f, true, 2f);
            Assert.That(progress.Finished, Is.False);
            progress.Tick(.5f, true, 0f);
            progress.Tick(.1f, false, 0f);
            progress.Tick(.5f, true, 0f);
            Assert.That(progress.Finished, Is.False, "Leaving the zone must reset the dwell timer.");
            progress.Tick(.25f, true, 0f);
            Assert.That(progress.Finished, Is.True);
            var time = progress.ElapsedSeconds;
            progress.Tick(10f, true, 0f);
            Assert.That(progress.ElapsedSeconds, Is.EqualTo(time));
        }

        [Test]
        public void RecoveryCostsTimeAndCannotAdvanceOrSkipBacktracking()
        {
            var progress = new PracticeProgress(6);
            for (var i = 0; i < 4; i++) { progress.Tick(.1f, true, 4f); }
            var time = progress.ElapsedSeconds;
            Assert.That(progress.Recover(1), Is.EqualTo(1));
            Assert.That(progress.NextCheckpoint, Is.EqualTo(2));
            Assert.That(progress.ElapsedSeconds, Is.EqualTo(time + 2f).Within(.001f));
            Assert.That(progress.Recover(), Is.EqualTo(0));
            Assert.That(progress.Recover(), Is.EqualTo(0));
        }

        [UnityTest]
        public IEnumerator HoldRecoveryTriggersOnceAndStopsUntilRelease()
        {
            yield return SceneManager.LoadSceneAsync("Practice", LoadSceneMode.Additive);
            var course = Object.FindFirstObjectByType<PracticeCourse>();
            _mouse = InputSystem.AddDevice<Mouse>();
            yield return null;
            yield return null;
            var start = new Vector2(Screen.width * .4f, Screen.height * .3f);
            InputSystem.QueueStateEvent(_mouse, new MouseState { position = start }.WithButton(MouseButton.Left));
            yield return new WaitForSeconds(2f);
            Assert.That(course.Progress.Recoveries, Is.EqualTo(1));
            Assert.That(course.GetComponent<BikeMotor>().Speed, Is.LessThan(.01f));
            yield return new WaitForSeconds(1.5f);
            Assert.That(course.Progress.Recoveries, Is.EqualTo(1), "Held finger must not repeatedly reset the bike.");
            InputSystem.QueueStateEvent(_mouse, new MouseState { position = start });
            yield return null;
            yield return null;
            InputSystem.QueueStateEvent(_mouse, new MouseState { position = start }.WithButton(MouseButton.Left));
            yield return new WaitForSeconds(.5f);
            InputSystem.QueueStateEvent(_mouse, new MouseState { position = start });
            yield return new WaitForSeconds(1f);
            Assert.That(course.Progress.Recoveries, Is.EqualTo(1), "A short interrupted hold must not recover.");
        }

        [UnityTest]
        public IEnumerator AuthoredCourseCanBeDrivenToFinishAndRestarted()
        {
            yield return SceneManager.LoadSceneAsync("Practice", LoadSceneMode.Additive);
            var course = Object.FindFirstObjectByType<PracticeCourse>();
            Assert.That(course, Is.Not.Null);
            yield return null;
            var motor = course.GetComponent<BikeMotor>();
            Time.timeScale = 4f;
            for (var i = 0; i < 4000 && !course.Progress.Finished; i++)
            {
                var delta = course.NextPoint - motor.transform.position;
                var direction = new Vector2(delta.x, delta.z);
                var throttle = course.IsFinalApproach && direction.magnitude < 1.4f ? 0f : .55f;
                motor.SetCommand(direction.normalized * throttle);
                yield return new WaitForFixedUpdate();
            }
            Assert.That(course.Progress.Finished, Is.True, $"Stuck at {motor.transform.position}, gate {course.Progress.NextCheckpoint}");
            Assert.That(motor.Speed, Is.LessThan(.01f));
            Assert.That(motor.PotholeHits, Is.GreaterThanOrEqualTo(1), "The authored route must encounter a pothole; floor contact must not suppress it.");
            _mouse = InputSystem.AddDevice<Mouse>();
            var button = (RectTransform)Object.FindFirstObjectByType<Button>().transform;
            var center = RectTransformUtility.WorldToScreenPoint(null, button.TransformPoint(button.rect.center));
            InputSystem.QueueStateEvent(_mouse, new MouseState { position = center }.WithButton(MouseButton.Left));
            yield return null;
            yield return null;
            InputSystem.QueueStateEvent(_mouse, new MouseState { position = center });
            yield return null;
            yield return null;
            yield return new WaitForFixedUpdate();
            Assert.That(course.Progress.Finished, Is.False);
            Assert.That(course.Progress.NextCheckpoint, Is.EqualTo(1));
            Assert.That(motor.transform.position.z, Is.EqualTo(0f).Within(.1f));
            Assert.That(motor.Speed, Is.LessThan(.01f));
            motor.transform.position = new Vector3(15f, motor.transform.position.y, 24f);
            course.Recover();
            Assert.That(motor.Speed, Is.LessThan(.01f));
            Assert.That(course.Progress.Recoveries, Is.EqualTo(1));
        }
    }
}
