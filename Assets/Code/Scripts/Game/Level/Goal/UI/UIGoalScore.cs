namespace Code.Scripts.Game.Level.Goal.UI
{
    using UnityEngine;
    using TMPro;

    [RequireComponent(typeof(TMP_Text))]
    public class UIGoalScore : MonoBehaviour
    {
        [Header("Game Elements")]
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
        
        /// <summary>
        /// Event handler for when a goal is scored.
        /// </summary>
        /// <param name="score">The total score of the goal.</param>
        private void OnGoalScored(int score)
        {
            SetText(score.ToString());
        }
        
        /// <summary>
        /// Sets the text of the score.
        /// </summary>
        /// <param name="text">The text to set.</param>
        private void SetText(string text)
        {
            _scoreText.text = text;
        }
    }
}