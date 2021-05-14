using System;
using UnityEngine;
using UnityEngine.Events;

namespace Scripts
{
    public class DamageableObject : MonoBehaviour
    {
        public float maxHealth;
       

        public bool displayDamageColor;
        public bool useEvent;
        public UnityEvent onDeathEvent;
        
        private float m_CurrentHealth;
        private MeshRenderer m_Renderer;
        private Color defaultColor;
        private void Awake()
        {
            m_CurrentHealth = maxHealth;
            m_Renderer = GetComponent<MeshRenderer>();
            defaultColor = m_Renderer.material.color;
        }


        public void TakeDamage(float damage)
        {
            m_CurrentHealth -= damage;
            if (m_CurrentHealth <= 0)
            {
                Die();
            }

            if (displayDamageColor)
                ChangeColor();
        }

        private void ChangeColor()
        {
            m_Renderer.material.color = Color.red;
        }

        private void Update()
        {
            m_Renderer.material.color = Color.Lerp(m_Renderer.material.color, defaultColor, 0.25f);
        }

        private void Die()
        {
            if (useEvent)
                onDeathEvent.Invoke();
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}