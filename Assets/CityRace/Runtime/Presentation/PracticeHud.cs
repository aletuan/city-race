using CityRace.Gameplay.Riding;
using UnityEngine;
using UnityEngine.UI;

namespace CityRace.Presentation
{
    public sealed class PracticeHud : MonoBehaviour
    {
        [SerializeField] private PracticeCourse _course;
        [SerializeField] private Text _status;
        [SerializeField] private Text _hint;
        [SerializeField] private Button _restart;
        private float _nextRefresh;
        private bool Vietnamese => Application.systemLanguage == SystemLanguage.Vietnamese;

        public void Configure(PracticeCourse course, Text status, Text hint, Button restart)
        {
            _course = course;
            _status = status;
            _hint = hint;
            _restart = restart;
        }
        private void Start()
        {
            _restart.onClick.AddListener(Restart);
            _restart.GetComponentInChildren<Text>().text = Vietnamese ? "Chơi lại" : "Restart";
        }
        private void OnDestroy() { if (_restart != null) { _restart.onClick.RemoveListener(Restart); } }
        private void Restart() { _course.Restart(); }
        private void Update()
        {
            if (_course.Progress == null || Time.unscaledTime < _nextRefresh) { return; }
            _nextRefresh = Time.unscaledTime + 0.1f;
            var progress = _course.Progress;
            if (progress.Finished)
            {
                _status.text = (Vietnamese ? "Đã đến công ty!" : "Arrived at work!") + $"  {progress.ElapsedSeconds:0.0}s";
                _hint.text = Vietnamese ? "Chọn Chơi lại để thử lượt mới" : "Tap Restart for another ride";
                return;
            }
            var direction = _course.NextPoint - _course.transform.position;
            // Course component is on the bike, so the compass stays consistent with drag input.
            var arrow = Mathf.Abs(direction.x) > Mathf.Abs(direction.z)
                ? (direction.x > 0 ? "→" : "←") : (direction.z > 0 ? "↑" : "↓");
            if (_course.IsFinalApproach)
            {
                _status.text = (Vietnamese ? "Dừng trong ô vàng" : "Stop in the yellow square") + $"  {progress.ElapsedSeconds:0.0}s";
            }
            else { _status.text = $"{arrow}  {(Vietnamese ? "Đến công ty" : "To work")}  •  {progress.ElapsedSeconds:0.0}s"; }
            _hint.text = _course.RecoveryFraction > 0.15f
                ? (Vietnamese ? "Đang thoát kẹt… " : "Recovering… ") + $"{_course.RecoveryFraction * 100:0}%"
                : (Vietnamese ? "Kéo để lái • Nhả để phanh\nKẹt? Nhả rồi giữ yên 1,2 giây (+2s)" : "Drag to ride • Release to brake\nStuck? Release, then hold still 1.2s (+2s)");
        }
    }
}
