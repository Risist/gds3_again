using UnityEngine;
using BarkSystem;
using NaughtyAttributes;

namespace Ai
{
    [CreateAssetMenu(fileName = "BehaviourPackBark", menuName = "Ris/Ai/BehaviourPack/Bark", order = 0)]
    public class BehaviourPackBark : BehaviourPack
    {
        public EStimuliFilterType filterType = EStimuliFilterType.EEnemy;
        public EAwarenessLevel awarenessLevel = EAwarenessLevel.EInaccurate;

        [Expandable, SerializeField] BarkSet enterBark;
        [Expandable, SerializeField] BarkSet updateBark;
        [Expandable, SerializeField] BarkSet exitBark;
        
        protected override void DefineBehaviours_Impl()
        {
            // references
            var monoAttentionSelector = controller.GetComponentInChildren<MonoAttentionSelector>();
            var barkInstance = controller.GetComponent<BarkInstance>();

            // Attention mode aliases
            AttentionMode mode = monoAttentionSelector.GetAttentionMode(filterType, awarenessLevel);

            if (enterBark)
            {
                mode.AddOnEnter(() => BarkManager.Instance.CallBark(transform, enterBark));
            }

            if (exitBark)
            {
                mode.AddOnExit(() => BarkManager.Instance.CallBark(transform, exitBark));
            }

            if (barkInstance && updateBark)
            {
                mode.AddOnUpdate(() =>
                {
                    BarkManager.Instance.CallBark(transform, updateBark);
                });
            }
        }

    }
}
