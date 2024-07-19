namespace SPO.Player
{
    using UnityEngine;
    using VUDK.Patterns.Initialization.Interfaces;

    public class RacketGraphicsController : MonoBehaviour, IInit<SpriteRenderer>
    {
        private SpriteRenderer _spriteRenderer;

        public void Init(SpriteRenderer arg1)
        {
            _spriteRenderer = arg1;
        }

        public bool Check()
        {
            return _spriteRenderer != null;
        }
        
        public void AssignColor(Color color)
        {
            if (!Check()) return;
            
            _spriteRenderer.color = color;
        }
    }
}