using System.Collections;
using CityRace.Gameplay.Riding;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;

namespace CityRace.Tests
{
    public sealed class RidingTests
    {
        private GameObject _bike;
        private GameObject _wall;
        private Mouse _mouse;
        private Touchscreen _touch;
        private Rigidbody _body;
        private BikeMotor _motor;
        private InputSettings.BackgroundBehavior _background;
        private InputSettings.EditorInputBehaviorInPlayMode _editorInput;

        [SetUp]
        public void SetUp()
        {
            _background = InputSystem.settings.backgroundBehavior;
            _editorInput = InputSystem.settings.editorInputBehaviorInPlayMode;
            InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
            InputSystem.settings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
            _bike = new GameObject("Test bike");
            _body = _bike.AddComponent<Rigidbody>();
            _body.useGravity = false;
            _body.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            _bike.AddComponent<BoxCollider>().size = new Vector3(.65f, .7f, 1.4f);
            _motor = _bike.AddComponent<BikeMotor>();
        }

        [TearDown]
        public void TearDown()
        {
            InputSystem.settings.backgroundBehavior = _background;
            InputSystem.settings.editorInputBehaviorInPlayMode = _editorInput;
            Object.DestroyImmediate(_bike);
            if (_wall != null) { Object.DestroyImmediate(_wall); }
            if (_mouse != null) { InputSystem.RemoveDevice(_mouse); }
            if (_touch != null) { InputSystem.RemoveDevice(_touch); }
        }

        [UnityTest]
        public IEnumerator AccelerationIsCappedAndReleaseBrakes()
        {
            _motor.SetCommand(Vector2.up);
            for (var i = 0; i < 100; i++) { yield return new WaitForFixedUpdate(); }
            Assert.That(_motor.Speed, Is.InRange(7.8f, 8.01f));
            Assert.That(_body.position.z, Is.GreaterThan(5f));
            _motor.SetCommand(Vector2.zero);
            for (var i = 0; i < 30; i++) { yield return new WaitForFixedUpdate(); }
            Assert.That(_motor.Speed, Is.LessThan(.01f));
        }

        [UnityTest]
        public IEnumerator SteeringFollowsAnArcAndCannotSnap()
        {
            _motor.SetCommand(Vector2.right);
            yield return new WaitForFixedUpdate();
            Assert.That(Quaternion.Angle(Quaternion.identity, _body.rotation), Is.LessThan(3f));
            for (var i = 0; i < 50; i++) { yield return new WaitForFixedUpdate(); }
            Assert.That(_body.position.z, Is.GreaterThan(.2f));
            Assert.That(_body.position.x, Is.GreaterThan(.2f));
            Assert.That(Quaternion.Angle(_body.rotation, Quaternion.Euler(0, 90, 0)), Is.LessThan(15f));
        }

        [UnityTest]
        public IEnumerator SharpTurnReducesSpeedBeforeChangingDirection()
        {
            _motor.SetCommand(Vector2.up);
            for (var i = 0; i < 100; i++) { yield return new WaitForFixedUpdate(); }
            var speed = _motor.Speed;
            _motor.SetCommand(Vector2.right);
            for (var i = 0; i < 5; i++) { yield return new WaitForFixedUpdate(); }
            Assert.That(_motor.Speed, Is.LessThan(speed - .5f));
            Assert.That(Quaternion.Angle(Quaternion.identity, _body.rotation), Is.LessThan(25f));
        }

        [UnityTest]
        public IEnumerator SolidBoundaryStopsBike()
        {
            _wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _wall.transform.position = new Vector3(0, 0, 3);
            _wall.transform.localScale = new Vector3(8, 2, .5f);
            _motor.SetCommand(Vector2.up);
            for (var i = 0; i < 150; i++) { yield return new WaitForFixedUpdate(); }
            Assert.That(_body.position.z, Is.LessThan(2.2f));
            Assert.That(_body.position.z, Is.GreaterThan(1.5f));
        }

        [UnityTest]
        public IEnumerator MouseDragReleaseAndCancelRequireNewPress()
        {
            _mouse = InputSystem.AddDevice<Mouse>();
            var input = _bike.AddComponent<DragRideInput>();
            yield return null;
            yield return null;
            var start = new Vector2(Screen.width * .5f, Screen.height * .25f);
            InputSystem.QueueStateEvent(_mouse, new MouseState { position = start }.WithButton(MouseButton.Left));
            yield return null;
            yield return null;
            InputSystem.QueueStateEvent(_mouse, new MouseState { position = start + Vector2.up * input.Radius }.WithButton(MouseButton.Left));
            yield return null;
            yield return null;
            Assert.That(input.Command.y, Is.GreaterThan(.95f), $"screen={Screen.width}x{Screen.height}, safe={Screen.safeArea}, tracking={input.IsTracking}, origin={input.Origin}, mouse={Mouse.current?.position.ReadValue()}, touch={Touchscreen.current?.primaryTouch.position.ReadValue()}, mouseDown={Mouse.current?.leftButton.isPressed}, touchDown={Touchscreen.current?.primaryTouch.press.isPressed}");
            input.Cancel();
            yield return null;
            yield return null;
            Assert.That(input.Command, Is.EqualTo(Vector2.zero));
            InputSystem.QueueStateEvent(_mouse, new MouseState { position = start });
            yield return null;
            yield return null;
            InputSystem.QueueStateEvent(_mouse, new MouseState { position = start }.WithButton(MouseButton.Left));
            yield return null;
            yield return null;
            InputSystem.QueueStateEvent(_mouse, new MouseState { position = start + Vector2.right * input.Radius }.WithButton(MouseButton.Left));
            yield return null;
            yield return null;
            Assert.That(input.Command.x, Is.GreaterThan(.95f));
            InputSystem.QueueStateEvent(_mouse, new MouseState { position = start });
            yield return null;
            yield return null;
            Assert.That(input.Command, Is.EqualTo(Vector2.zero));
        }

        [UnityTest]
        public IEnumerator SecondFingerCannotKeepThrottleAfterFirstFingerLifts()
        {
            _touch = InputSystem.AddDevice<Touchscreen>();
            var input = _bike.AddComponent<DragRideInput>();
            yield return null;
            yield return null;
            var start = new Vector2(Screen.width * .5f, Screen.height * .25f);
            InputSystem.QueueStateEvent(_touch, new TouchState { touchId = 1, phase = UnityEngine.InputSystem.TouchPhase.Began, position = start });
            yield return null;
            yield return null;
            InputSystem.QueueStateEvent(_touch, new TouchState { touchId = 1, phase = UnityEngine.InputSystem.TouchPhase.Moved, position = start + Vector2.up * input.Radius });
            yield return null;
            yield return null;
            Assert.That(input.Command.y, Is.GreaterThan(.95f));
            InputSystem.QueueStateEvent(_touch, new TouchState { touchId = 2, phase = UnityEngine.InputSystem.TouchPhase.Began, position = start + Vector2.right * 10f });
            yield return null;
            yield return null;
            Assert.That(input.Command.y, Is.GreaterThan(.95f));
            InputSystem.QueueStateEvent(_touch, new TouchState { touchId = 1, phase = UnityEngine.InputSystem.TouchPhase.Ended, position = start });
            yield return null;
            yield return null;
            Assert.That(input.Command, Is.EqualTo(Vector2.zero));
            InputSystem.QueueStateEvent(_touch, new TouchState { touchId = 2, phase = UnityEngine.InputSystem.TouchPhase.Moved, position = start + Vector2.up * input.Radius });
            yield return null;
            yield return null;
            Assert.That(input.Command, Is.EqualTo(Vector2.zero));
        }

        [UnityTest]
        public IEnumerator TouchDragHasSameNormalizedThrottleAndRelease()
        {
            _touch = InputSystem.AddDevice<Touchscreen>();
            var input = _bike.AddComponent<DragRideInput>();
            yield return null;
            yield return null;
            var start = new Vector2(Screen.width * .5f, Screen.height * .25f);
            InputSystem.QueueStateEvent(_touch, new TouchState { touchId = 1, phase = UnityEngine.InputSystem.TouchPhase.Began, position = start });
            yield return null;
            yield return null;
            InputSystem.QueueStateEvent(_touch, new TouchState { touchId = 1, phase = UnityEngine.InputSystem.TouchPhase.Moved, position = start + Vector2.up * input.Radius });
            yield return null;
            yield return null;
            Assert.That(input.Command.y, Is.GreaterThan(.95f), $"screen={Screen.width}x{Screen.height}, safe={Screen.safeArea}, tracking={input.IsTracking}, origin={input.Origin}, mouse={Mouse.current?.position.ReadValue()}, touch={Touchscreen.current?.primaryTouch.position.ReadValue()}, mouseDown={Mouse.current?.leftButton.isPressed}, touchDown={Touchscreen.current?.primaryTouch.press.isPressed}");
            InputSystem.QueueStateEvent(_touch, new TouchState { touchId = 1, phase = UnityEngine.InputSystem.TouchPhase.Ended, position = start });
            yield return null;
            yield return null;
            Assert.That(input.Command, Is.EqualTo(Vector2.zero));
        }
    }
}
