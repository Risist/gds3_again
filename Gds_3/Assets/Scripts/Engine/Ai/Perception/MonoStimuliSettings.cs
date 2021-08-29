using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using NaughtyAttributes;

namespace Ai
{

    public enum EStimuliFilterType
    {
        EIdle,
        EEnemy,
        EAlly,
        ENeutral,
        ENoise,
        EPain,
        ETouch,
        EHerd,
    }

    public class MonoStimuliSettings : MonoBehaviour
    {
        [Serializable]
        public class StorageSettings
        {
            [BoxGroup(AttributeHelper.HEADER_GENERAL)]
            [Tooltip("How many events will be stored")]
            public int nEvents = 5;

            [BoxGroup(AttributeHelper.HEADER_GENERAL)]
            public float maxEventLifeTime = 1.0f;

            [BoxGroup(AttributeHelper.HEADER_GENERAL)]
            public float velocityPredictionScale = 1.0f;

            [BoxGroup(AttributeHelper.HEADER_DEBUG), ReadOnly] public StimuliStorage storage;
        }



        BehaviourController _controller;
        public BehaviourController controller
        {
            get
            {
                _controller = _controller ?? GetComponentInParent<BehaviourController>();
                return _controller;
            }
        }
        public StateMachine stateMachine => controller.stateMachine;
        public GenericBlackboard blackboard => controller.blackboard;

        /// sense type id's to reference blackboard content
        public const string enemyId     = nameof(enemyId);
        public const string allyId      = nameof(allyId);
        public const string neutralId   = nameof(neutralId);
        public const string noiseId     = nameof(noiseId);
        public const string painId      = nameof(painId);
        public const string touchId     = nameof(touchId);
        public const string herdId      = nameof(herdId);

        public StorageSettings enemyStorageSettings;
        public StorageSettings allyStorageSettings;
        public StorageSettings neutralStorageSettings;
        public StorageSettings touchStorageSettings;
        public StorageSettings noiseStorageSettings;
        public StorageSettings painStorageSettings;

        public StimuliStorage enemyStorage => enemyStorageSettings.storage;
        public StimuliStorage allyStorage => allyStorageSettings.storage;
        public StimuliStorage neutralStorage => neutralStorageSettings.storage;
        public StimuliStorage touchStorage => touchStorageSettings.storage;
        public StimuliStorage noiseStorage => noiseStorageSettings.storage;
        public StimuliStorage painStorage => painStorageSettings.storage;

        protected void Awake()
        {
            enemyStorageSettings.storage = RegisterSenseInBlackboard(enemyId, enemyStorageSettings);
            allyStorageSettings.storage = RegisterSenseInBlackboard(allyId, allyStorageSettings);
            neutralStorageSettings.storage = RegisterSenseInBlackboard(neutralId, neutralStorageSettings);

            touchStorageSettings.storage = RegisterSenseInBlackboard(touchId, touchStorageSettings);
            noiseStorageSettings.storage = RegisterSenseInBlackboard(noiseId, noiseStorageSettings);
            painStorageSettings.storage = RegisterSenseInBlackboard(painId, painStorageSettings);
        }


        public StimuliFilter GetFilter(EStimuliFilterType type)
        {
            switch(type)
            {
                case EStimuliFilterType.EIdle:
                    return null;
                case EStimuliFilterType.EEnemy:
                    return GetEnemyFilter();
                case EStimuliFilterType.EAlly:
                    return GetAllyFilter();
                case EStimuliFilterType.ENeutral:
                    return GetNeutralFilter();
                case EStimuliFilterType.ENoise:
                    return GetNoiseFilter();
                case EStimuliFilterType.EPain:
                    return GetPainFilter();
                case EStimuliFilterType.ETouch:
                    return GetTouchFilter();
                case EStimuliFilterType.EHerd:
                    return GetHerdFilter(); 
            }

            Debug.LogWarning("Used unsupported filter type");
            return null;
        }

        public StimuliFilter GetEnemyFilter()
        {
            var storage = blackboard.GetValue<StimuliStorage>(enemyId).value;
            return blackboard.InitValue<StimuliFilter>(enemyId,
                () => new StimuliFilter(storage, controller.transform)).value;
        }
        public StimuliFilter GetAllyFilter()
        {
            var storage = blackboard.GetValue<StimuliStorage>(allyId).value;
            return blackboard.InitValue<StimuliFilter>(allyId,
                () => new StimuliFilter(storage, controller.transform)).value;
        }
        public StimuliFilter GetNeutralFilter()
        {
            var storage = blackboard.GetValue<StimuliStorage>(neutralId).value;
            return blackboard.InitValue<StimuliFilter>(neutralId,
                () => new StimuliFilter(storage, controller.transform)).value;
        }
        public StimuliFilter GetNoiseFilter()
        {
            var storage = blackboard.GetValue<StimuliStorage>(noiseId).value;
            return blackboard.InitValue<StimuliFilter>(noiseId,
                () => new StimuliFilter(storage, controller.transform)).value;
        }
        public StimuliFilter GetPainFilter()
        {
            var storage = blackboard.GetValue<StimuliStorage>(painId).value;
            return blackboard.InitValue<StimuliFilter>(painId,
                () => new StimuliFilter(storage, controller.transform)).value;
        }
        public StimuliFilter GetTouchFilter()
        {
            var storage = blackboard.GetValue<StimuliStorage>(touchId).value;
            return blackboard.InitValue<StimuliFilter>(touchId,
                () => new StimuliFilter(storage, controller.transform)).value;
        }
        public StimuliFilter GetHerdFilter()
        {
            var storage = blackboard.GetValue<StimuliStorage>(herdId).value;
            return blackboard.InitValue<StimuliFilter>(herdId,
                () => new StimuliFilter(storage, controller.transform)).value;
        }


        // registers event
        protected StimuliStorage RegisterSenseInBlackboard(string blackboardName, StorageSettings settings)
        {
            Debug.Assert(controller);
            var storage = controller.blackboard.InitValue(blackboardName,
                () => new StimuliStorage(settings.maxEventLifeTime, settings.nEvents));

            controller.RegisterSense(storage.value);
            return storage.value;
        }

        protected StimuliStorage GetStorage(string blackboardName)
        {
            Debug.Assert(controller);
            var storage = controller.blackboard.GetValue<StimuliStorage>(blackboardName);
            return storage.value;
        }
    }
}
