using System;
using DG.Tweening;
using UnityEngine;

namespace Effects
{
    public class Animation_ButtonPress : MonoBehaviour
    {
        // Start is called before the first frame update
        private Vector3 _ogPos;
        private Vector3 _pushPos;
        private bool _triggerFlag;
        private Tweener _buttonPush;
        private Tweener _buttonReset;

        private void Awake()
        {
            _ogPos = transform.position;
            _pushPos = transform.position - transform.up * .025f;
        }

        public void TriggerButtonAnimation()
        {
            _triggerFlag = true;
            _buttonReset.Pause();
            _buttonPush = transform.DOMove(_pushPos, 0.3f).OnComplete(() =>
            {
                if (!_triggerFlag)
                {
                    _buttonReset = transform.DOMove(_ogPos, 0.3f);
                }
            });

            // transform.DOMove(transform.position + transform.up * 20f, 0.3f)
            //     .OnComplete(() => transform.DOMove(_ogPos, 0.3f));

            _triggerFlag = false;
        }

        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}