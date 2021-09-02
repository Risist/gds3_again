using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BarkSystem
{
    public class BarkController : MonoBehaviour//, IEditorUpdatable
    {
        [Header("Position")]
        public Transform target;
        public Vector3 positionOffset;

        [Header("References")]
        public TextMeshProUGUI barkText;

        [Header("Params")]
        public float alphaChangeSpeed = 1.0f;
        public float defaultAlpha;
        public float defaultAlphaMainText;

        [HideInInspector] public BarkInstance barkInstance;

        MaskableGraphic[] graphics;

        BarkRecord _currentBark;
        MinimalTimer _tDisplayBark = new MinimalTimer();

        public void CallBark(BarkRecord bark)
        {
            if(bark == null || bark.text.Equals(""))
            {
                // do not show background of the bark if there is no text
                // "" string will function as a possibility in barkSet to rand no text display
                bark = null;
                return;
            }

            _currentBark = bark;
            _tDisplayBark.Restart();
            barkText.text = bark.text;
        }

        public void FadeGraphics(float targetAlpha, float targetAlphaMainText)
        {
            foreach(var it in graphics)
            {
                if (it == barkText)
                    continue;

                var color = it.color;
                color.a = Mathf.MoveTowards(color.a, targetAlpha, alphaChangeSpeed * Time.deltaTime);
                it.color = color;
            }

            {
                var color = barkText.color;
                color.a = Mathf.MoveTowards(color.a, targetAlphaMainText, alphaChangeSpeed * Time.deltaTime);
                barkText.color = color;
            }

        }
        public void SetGraphicsAlpha(float targetAlpha, float targetAlphaMainText)
        {
            foreach (var it in graphics)
            {
                if (it == barkText)
                    continue;

                var color = it.color;
                color.a = targetAlpha;
                it.color = color;
            }

            {
                var color = barkText.color;
                color.a = targetAlphaMainText;
                barkText.color = color;
            }
        }

        public bool IsFadedOut()
        {
            foreach (var it in graphics)
            {
                if (!Mathf.Approximately(it.color.a, 0))
                    return false;
            }
            return true;
        }

        void OnSpawn()
        {
            graphics = GetComponentsInChildren<MaskableGraphic>();
            SetGraphicsAlpha(0, 0);
        }


        private void Start()
        {
            graphics = GetComponentsInChildren<MaskableGraphic>();
            SetGraphicsAlpha(0, 0);
        }

        private void LateUpdate()
        {
            if (target)
            {
                transform.position = target.position + positionOffset;
            }
            
            if (_currentBark != null)
            {
                FadeGraphics(defaultAlpha, defaultAlphaMainText);

                if (target == null || _tDisplayBark.IsReady(_currentBark.barkDisplayTime))
                {
                    _currentBark = null;
                }
            }else
            {
                FadeGraphics(0, 0);

                if (IsFadedOut())
                {
                    gameObject.SetActive(false);
                    if (barkInstance)
                    {
                        barkInstance.currentBarkController = null;
                    }
                }
            }
        }

        /*void IEditorUpdatable.OnEditorUpdate()
        {
            if (target)
            {
                transform.position = target.position + positionOffset;
            }

            graphics = GetComponentsInChildren<MaskableGraphic>();
            SetGraphicsAlpha(defaultAlpha, defaultAlphaMainText);
        }*/
    }
}
