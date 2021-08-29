using UnityEngine;
using BarkSystem;

namespace Ai
{
    [CreateAssetMenu(fileName = "BehaviourPackBark", menuName = "Ris/Ai/BehaviourPack/Bark", order = 0)]
    public class BehaviourPackBark : BehaviourPack
    {
        public EStimuliFilterType filterType = EStimuliFilterType.EEnemy;
        public EAwarenessLevel awarenessLevel = EAwarenessLevel.EInaccurate;

        public BarkSet enterBark;
        public BarkSet updateBark;
        public BarkSet exitBark;
        
        protected override void DefineBehaviours_Impl()
        {
            // references
            var monoAttentionSelector = controller.GetComponentInChildren<MonoAttentionSelector>();
            var barkInstance = controller.GetComponent<BarkInstance>();

            // Attention mode aliases
            AttentionMode modeEnemyLost = monoAttentionSelector.GetAttentionMode(filterType, awarenessLevel);

            if (enterBark)
            {
                modeEnemyLost.AddOnEnter(() => BarkManager.Instance.CallBark(transform, enterBark));
            }

            if (exitBark)
            {
                modeEnemyLost.AddOnExit(() => BarkManager.Instance.CallBark(transform, exitBark));
            }

            if (barkInstance && updateBark)
            {
                modeEnemyLost.AddOnUpdate(() =>
                {
                    BarkManager.Instance.CallBark(transform, updateBark);
                });
            }
        }

    }
}
