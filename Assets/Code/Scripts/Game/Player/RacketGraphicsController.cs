namespace SPO.Player
{
    using UnityEngine;
    using VUDK.Patterns.Initialization.Interfaces;

    public class RacketGraphicsController : MonoBehaviour, IInit<SpriteRenderer>
    {
        private SpriteRenderer _spriteRenderer;

        /// <summary>
        /// Initializes the racket graphics controller.
        /// </summary>
        /// <param name="arg1">The sprite renderer.</param>
        public void Init(SpriteRenderer arg1)
        {
            _spriteRenderer = arg1;
        }

        /// <summary>
        /// Checks if the racket graphics controller is correctly initialized.
        /// </summary>
        /// <returns>True if the racket graphics controller is correctly initialized, false otherwise.</returns>
        public bool Check()
        {
            return _spriteRenderer != null;
        }
        
        /// <summary>
        /// Assigns a color to the player racket.
        /// </summary>
        /// <param name="color">The color to assign.</param>
        public void AssignColor(Color color)
        {
            if (!Check()) return;
            
            _spriteRenderer.color = color;
        }
    }
}