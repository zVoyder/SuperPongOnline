namespace Code.Scripts.Game.Level.Goal.UI
{
    using UnityEngine;
    using TMPro;

    [RequireComponent(typeof(TMP_Text))]
    public class UIGoalScore : MonoBehaviour
    {
        [SerializeField]
        private Goal _relatedGoal;
        
        private TMP_Text _scoreText;

        private void Awake()
        {
            TryGetComponent(out _scoreText);
            SetText("0");
        }

        private void OnEnable()
        {
            _relatedGoal.OnScoreValueChangedHookReceived += OnGoalScored;
        }
        
        private void OnDisable()
        {
            _relatedGoal.OnScoreValueChangedHookReceived -= OnGoalScored;
        }
        
        private void OnGoalScored(int score)
        {
            SetText(score.ToString());
        }
        
        private void SetText(string text)
        {
            _scoreText.text = text;
        }
    }
}