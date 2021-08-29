using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Ai
{
    /*
     *
     * OverlapCircle check by memorableMask
     * to all hit targets which have AiPerceiveUnit and are in given cone cast ray
     * The ray will check if there is no obstacle at the way from perceiver to target
     *
     * All targets that pass the test will be pushed into special AiFocus type
     *      that will store targets and sort them when needed
     *
     */

    [RequireComponent(typeof(SphereCollider))]
    public class SenseSight : SenseBase
    {
        public const string HEADER_VISION = "VisionSettings";
        public const string HEADER_SHAPE = "Shape";

        [BoxGroup(HEADER_SHAPE)] public List<VisionAreaRecord> visionAreas;

        [BoxGroup(AttributeHelper.HEADER_DEBUG)] public bool drawRayGizmos;

        TimedDetection timedDetection = new TimedDetection();
        Transform cachedTransform;


        float searchDistance = 5.0f;
        Vector2 offset;

        [Serializable]
        public class VisionAreaRecord
        {
            [BoxGroup(AttributeHelper.HEADER_DEBUG)] public string DegugName;
            [BoxGroup(AttributeHelper.HEADER_DEBUG)] public Color gizmoColor = new Color(1, 0, 0, 0.25f);

            [Space]
            [BoxGroup(HEADER_VISION)] public bool trackEnemy = true;
            [BoxGroup(HEADER_VISION)] public bool trackAlly = false;
            [BoxGroup(HEADER_VISION)] public bool trackNeutrals = false;
            [BoxGroup(HEADER_VISION)] public float timeToBeDetected = 1;
            [Space]
            [BoxGroup(HEADER_SHAPE)] public float coneAngle = 170.0f;
            [BoxGroup(HEADER_SHAPE)] public float searchDistance = 5.0f;

            [Space]
            [BoxGroup(HEADER_SHAPE)] public Vector2 scale;
            [BoxGroup(HEADER_SHAPE)] public Vector2 offset;
            [BoxGroup(HEADER_SHAPE)] public bool onlyInFront;

            [HideInInspector] public bool detected;

        }

        public bool CheckPointCollision(VisionAreaRecord visionRecord, Vector3 pointToCheck)
        {
            Vector3 pointToCheckWithOffset = pointToCheck - transform.rotation * visionRecord.offset.To3D();

            Vector2 toIt = (pointToCheckWithOffset - transform.position).To2D();

            toIt.x /= visionRecord.scale.x;
            toIt.y /= visionRecord.scale.y;

            if (toIt.sqrMagnitude > visionRecord.searchDistance.Sq())
                return false;

            float cosAngle = Vector2.Dot(toIt.normalized, transform.forward.To2D());
            float angle = Mathf.Acos(cosAngle) * 180 / Mathf.PI;
            //Debug.Log(angle);
            bool bProperAngle = angle < visionRecord.coneAngle * 0.5f;
            if (!bProperAngle)
                return false;

            if (visionRecord.onlyInFront)
            {
                Vector2 toPoint = (pointToCheckWithOffset - transform.position).To2D();

                float cosAnglePoint = Vector2.Dot(toPoint.normalized, transform.forward.To2D());
                if (cosAnglePoint < 0)
                    return false;
            }

            return true;
        }


        // insert given event to the storage specified by an attitiude
        bool InsertEvent(MemoryEvent ev, Fraction.EAttitude attitude, VisionAreaRecord visionArea)
        {
            switch (attitude)
            {
                case Fraction.EAttitude.EEnemy:
                    if (visionArea.trackEnemy)
                    {
                        stimuliSettings.enemyStorage.PerceiveEvent(ev);
                        return true;
                    }
                    break;
                case Fraction.EAttitude.EFriendly:
                    if (visionArea.trackAlly)
                    {
                        stimuliSettings.allyStorage.PerceiveEvent(ev);
                        return true;
                    }
                    break;
                case Fraction.EAttitude.ENeutral:
                    if (visionArea.trackNeutrals)
                    {
                        stimuliSettings.neutralStorage.PerceiveEvent(ev);
                        return true;
                    }
                    break;
            }
            return false;
        }
        MonoStimuliSettings.StorageSettings GetStorageSettings(Fraction.EAttitude attitude)
        {
            switch (attitude)
            {
                case Fraction.EAttitude.EEnemy:
                    return stimuliSettings.enemyStorageSettings;
                case Fraction.EAttitude.EFriendly:
                    return stimuliSettings.allyStorageSettings;
                case Fraction.EAttitude.ENeutral:
                    return stimuliSettings.neutralStorageSettings;
            }
            return null;
        }

        new void Awake()
        {
            base.Awake();

            // cache transform
            cachedTransform = base.transform;

            var collider = GetComponent<SphereCollider>();
            searchDistance = collider.radius;
            offset = collider.center.To2D();
        }

        #region Search

        private void OnTriggerEnter(Collider other)
        {
            bool b = ((1 << other.gameObject.layer) & SightManager.Instance.memorableMask) != 0;
            if (b)
            {
                PerceiveUnit perceiveUnit = other.GetComponent<PerceiveUnit>();
                if (perceiveUnit == myUnit)
                    // oh, come on do not look at yourself... don't be soo narcissistic
                    return;

                if (!perceiveUnit)
                    // no perceive unit, this target is invisible to us
                    return;

                Fraction itFraction = perceiveUnit.fraction;
                if (!itFraction)
                    // the same as above,
                    return;

                timedDetection.AddRecord(perceiveUnit);
            }
        }
        private void OnTriggerExit(Collider other)
        {

            var unit = other.GetComponent<PerceiveUnit>();
            if (unit)
            {
                timedDetection.RemoveRecord(unit);
            }
        }

        Timer tUpdate = new Timer(0.1f);

        private void FixedUpdate()
        {
            if(tUpdate.IsReadyRestart())
            timedDetection.IterateOver((record) =>
            {
                var perceiveUnit = record.unit;
                Fraction myFraction = myUnit.fraction;
                if (!myFraction)
                {
#if UNITY_EDITOR
                    Debug.LogWarning("No fraction in perceive unit but trying to use sight");
#endif
                    // there's no way to determine where to put events
                    return;
                }

                Transform itTransform = perceiveUnit.transform;

                // ok, now check if it has PerceiveUnit component
                // we need it's fraction to determine our attitude
                if (perceiveUnit == myUnit)
                    // oh, come on do not look at yourself... don't be soo narcissistic
                    return;

                if (!perceiveUnit)
                    // no perceive unit, this target is invisible to us
                    return;

                Fraction itFraction = perceiveUnit.fraction;
                if (!itFraction)
                    // the same as above,
                    return;

                var rb = perceiveUnit.GetComponent<Rigidbody>();
                Debug.Assert(rb);

                //// determine attitude
                Fraction.EAttitude attitude = myFraction.GetAttitude(itFraction);

                //// Check if obstacles blocks vision
                if (DoObstaclesBlockVision(itTransform.position))
                {
                    record.tSeen.Restart();
                    return;
                }
                bool anyDetected = false;
                foreach (var itVisionArea in visionAreas)
                {
                    bool bCheck = CheckPointCollision(itVisionArea, itTransform.position);

                    anyDetected |= bCheck;

                    if (!record.tSeen.IsReady(itVisionArea.timeToBeDetected))
                        continue;

                    if (bCheck)
                    {
                        MemoryEvent ev = new MemoryEvent
                        {
                            exactPosition = itTransform.position,
                            forward = itTransform.forward,

                            // if collider has rigidbody then take its velocity
                            // otherwise there is no simple way to determine event velocity
                            velocity = rb ? rb.velocity * GetStorageSettings(attitude).velocityPredictionScale : Vector3.zero,
                            // set up agent responsible for this event
                            perceiveUnit = perceiveUnit
                        };


                        // ensure event will tick from now on
                        ev.lifetimeTimer.Restart();

                        if (drawRayGizmos)
                        {
                            Debug.DrawRay(itTransform.position, Vector3.up, Color.blue, SightManager.Instance.searchTime * GetStorageSettings(attitude).nEvents);
                            Debug.DrawRay(itTransform.position, ev.velocity * SightManager.Instance.searchTime, Color.gray, SightManager.Instance.searchTime);
                        }


                        if (InsertEvent(ev, attitude, itVisionArea))
                        {
                            itVisionArea.detected = true;
                            return;
                        }
                    }
                }

                if (!anyDetected)
                {
                    record.tSeen.Restart();
                }
            });
        }

        bool DoObstaclesBlockVision(Vector3 target)
        {
            // we will change searchDistance based on visibility of obstacles;
            float localSearchDistance = this.searchDistance;

            Vector3 toTarget = target - cachedTransform.position;
            float toTargetSq = toTarget.ToPlane().sqrMagnitude;


            int n = Physics.RaycastNonAlloc(cachedTransform.position, toTarget, StaticCacheLists.raycastHitCache,
                toTarget.magnitude, SightManager.Instance.obstacleMask);

            bool bObstaclesBlocksVision = false;
            for (int i = 0; i < n; ++i)
            {
                var it = StaticCacheLists.raycastHitCache[i];

                PerceiveUnit unit = it.collider.GetComponent<PerceiveUnit>();
                if (!unit)
                {
                    // we assume objects that do not have perceive unit will behave as non transparent
                    // so we can't see our target
                    bObstaclesBlocksVision = true;
                    break;
                }

                if (unit == myUnit)
                    // well, i'm not that fat ... i guess
                    continue;


                localSearchDistance *= unit.transparencyLevel;
                if (localSearchDistance * localSearchDistance < toTargetSq * myUnit.distanceModificator)
                // transparency is reduced too much to see the target
                {
                    bObstaclesBlocksVision = true;
                    break;
                }
            }

            if (drawRayGizmos)
            {
                Debug.DrawRay(cachedTransform.position, toTarget, bObstaclesBlocksVision ? Color.yellow : Color.green, 0.25f);
            }

            return bObstaclesBlocksVision;
        }

        #endregion Search

        protected void OnEnable() => SightManager.Instance.AddSight(this);

        protected void OnDisable() => SightManager.UnsafeInstance?.RemoveSight(this);

        void OnDrawGizmosSelected()
        {
#if UNITY_EDITOR

            var collider = GetComponent<SphereCollider>();
            searchDistance = collider.radius;
            offset = collider.center.To2D();


            UnityEditor.Handles.matrix = Matrix4x4.Translate(transform.rotation * offset.To3D()) * transform.localToWorldMatrix;
            UnityEditor.Handles.DrawWireDisc(Vector3.zero, transform.up, searchDistance);

            UnityEditor.Handles.matrix = Matrix4x4.identity;
            UnityEditor.Handles.color = Color.black;
            UnityEditor.Handles.DrawLine(
                transform.position + transform.right * searchDistance,
                transform.position - transform.right * searchDistance,
                3);

            UnityEditor.Handles.matrix = Matrix4x4.identity;
            UnityEditor.Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;
            foreach (var it in visionAreas)
            {
                Color cl = it.gizmoColor;
                UnityEditor.Handles.color = cl;

                UnityEditor.Handles.matrix =
                    Matrix4x4.Translate(transform.position + transform.rotation * it.offset.To3D())
                  * Matrix4x4.Rotate(transform.rotation)
                  * Matrix4x4.Scale(new Vector3(it.scale.x, 1, it.scale.y))
                  * Matrix4x4.Rotate(transform.rotation).inverse;

                if (it.detected)
                {
                    UnityEditor.Handles.DrawSolidArc(
                        Vector3.zero,
                        transform.up,
                        Quaternion.Euler(0, -it.coneAngle / 2, 0) * transform.forward,
                        it.coneAngle,
                        it.searchDistance);
                    it.detected = false;
                }else
                {
                    cl.a = 1.0f;
                    UnityEditor.Handles.color = cl;
                    UnityEditor.Handles.DrawWireArc(
                           Vector3.zero,
                           transform.up,
                           Quaternion.Euler(0, -it.coneAngle / 2, 0) * transform.forward,
                           it.coneAngle,
                           it.searchDistance);
                }
            }
#endif
        }

    }
    /*
    public class SenseSight : SenseBase
    {
        public const string HEADER_VISION = "VisionSettings";
        public const string HEADER_SHAPE = "Shape";

        [BoxGroup(HEADER_SHAPE)] public List<VisionAreaRecord> visionAreas;
        [BoxGroup(HEADER_SHAPE)] public float searchDistance = 5.0f;
        [BoxGroup(HEADER_SHAPE)] public Vector2 offset;

        protected Transform cachedTransform;

        [BoxGroup(AttributeHelper.HEADER_DEBUG)] public bool drawRayGizmos;
        [BoxGroup(AttributeHelper.HEADER_DEBUG)] public Vector3 debugVector;

        [Serializable]
        public class VisionAreaRecord
        {
            [BoxGroup(AttributeHelper.HEADER_DEBUG)] public string DegugName;
            [BoxGroup(AttributeHelper.HEADER_DEBUG)] public Color gizmoColor = new Color(1, 0, 0, 0.25f);
            
            [Space]
            [BoxGroup(HEADER_VISION)] public bool trackEnemy = true;
            [BoxGroup(HEADER_VISION)] public bool trackAlly = false;
            [BoxGroup(HEADER_VISION)] public bool trackNeutrals = false;
            [Space]
            [BoxGroup(HEADER_SHAPE)] public float coneAngle = 170.0f;
            [BoxGroup(HEADER_SHAPE)] public float searchDistance = 5.0f;

            [Space]
            [BoxGroup(HEADER_SHAPE)] public Vector2 scale;
            [BoxGroup(HEADER_SHAPE)] public Vector2 offset;
            [BoxGroup(HEADER_SHAPE)] public bool onlyInFront;

            [BoxGroup(HEADER_VISION)] public TimedDetection timedDetection;
        }

        public bool CheckPointCollision(VisionAreaRecord visionRecord, Vector3 pointToCheck)
        {
            Vector3 pointToCheckWithOffset = pointToCheck - transform.rotation * visionRecord.offset.To3D();

            Vector2 toIt = (pointToCheckWithOffset - transform.position).To2D();

            toIt.x /= visionRecord.scale.x;
            toIt.y /= visionRecord.scale.y;

            if (toIt.sqrMagnitude > visionRecord.searchDistance.Sq())
                return false;

            float cosAngle = Vector2.Dot(toIt.normalized, transform.forward.To2D());
            float angle = Mathf.Acos(cosAngle) * 180 / Mathf.PI;
            //Debug.Log(angle);
            bool bProperAngle = angle < visionRecord.coneAngle * 0.5f;
            if (!bProperAngle)
                return false;

            if(visionRecord.onlyInFront)
            {
                Vector2 toPoint = (pointToCheckWithOffset - transform.position).To2D();

                float cosAnglePoint = Vector2.Dot(toPoint.normalized, transform.forward.To2D());
                if (cosAnglePoint < 0)
                    return false;
            }

            return true;
        }


        // insert given event to the storage specified by an attitiude
        void InsertEvent(MemoryEvent ev, Fraction.EAttitude attitude, VisionAreaRecord visionArea)
        {
            switch (attitude)
            {
                case Fraction.EAttitude.EEnemy:
                    if (visionArea.trackEnemy)
                        stimuliSettings.enemyStorage.PerceiveEvent(ev);
                    break;
                case Fraction.EAttitude.EFriendly:
                    if (visionArea.trackAlly)
                        stimuliSettings.allyStorage.PerceiveEvent(ev);
                    break;
                case Fraction.EAttitude.ENeutral:
                    if (visionArea.trackNeutrals)
                        stimuliSettings.neutralStorage.PerceiveEvent(ev);
                    break;
            }
        }
        MonoStimuliSettings.StorageSettings GetStorageSettings(Fraction.EAttitude attitude)
        {
            switch (attitude)
            {
                case Fraction.EAttitude.EEnemy:
                    return stimuliSettings.enemyStorageSettings;
                case Fraction.EAttitude.EFriendly:
                    return stimuliSettings.allyStorageSettings;
                case Fraction.EAttitude.ENeutral:
                    return stimuliSettings.neutralStorageSettings;
            }
            return null;
        }

        new void Awake()
        {
            base.Awake();

            // cache transform
            cachedTransform = base.transform;
        }

        #region Search

        public void PerformSearch()
        {
            Fraction myFraction = myUnit.fraction;

            if (!myFraction)
            {
#if UNITY_EDITOR
                Debug.LogWarning("No fraction in perceive unit but trying to use sight");
#endif
                // there's no way to determine where to put events
                return;
            }

            // perform cast
            int n = Physics.OverlapSphereNonAlloc(cachedTransform.position, searchDistance, StaticCacheLists.colliderCache,
                SightManager.Instance.memorableMask);

            // preselect targets
            // they have to be in proper angle and contain PerceiveUnit
            for (int i = 0; i < n; ++i)
            {
                var it = StaticCacheLists.colliderCache[i];
                Transform itTransform = it.transform;

                // ok, now check if it has PerceiveUnit component
                // we need it's fraction to determine our attitude
                PerceiveUnit perceiveUnit = it.GetComponent<PerceiveUnit>();
                if (perceiveUnit == myUnit)
                    // oh, come on do not look at yourself... don't be soo narcissistic
                    continue;

                if (!perceiveUnit)
                    // no perceive unit, this target is invisible to us
                    continue;

                Fraction itFraction = perceiveUnit.fraction;
                if (!itFraction)
                    // the same as above,
                    return;

                //// determine attitude
                Fraction.EAttitude attitude = myFraction.GetAttitude(itFraction);

                //// Check if obstacles blocks vision
                if (DoObstaclesBlockVision(itTransform.position))
                    continue;

                foreach(var itVisionArea in visionAreas)
                {
                    bool bCheck = CheckPointCollision(itVisionArea, itTransform.position);
                    if(bCheck)
                    {
                        var rb = it.attachedRigidbody;
                        MemoryEvent ev = new MemoryEvent
                        {
                            exactPosition = itTransform.position,
                            forward = itTransform.forward,

                            // if collider has rigidbody then take its velocity
                            // otherwise there is no simple way to determine event velocity
                            velocity = rb ? rb.velocity * GetStorageSettings(attitude).velocityPredictionScale : Vector3.zero,
                            // set up agent responsible for this event
                            perceiveUnit = perceiveUnit
                        };


                        // ensure event will tick from now on
                        ev.lifetimeTimer.Restart();

                        if (drawRayGizmos)
                        {
                            Debug.DrawRay(itTransform.position, Vector3.up, Color.blue, SightManager.Instance.searchTime * GetStorageSettings(attitude).nEvents);
                            Debug.DrawRay(itTransform.position, ev.velocity * SightManager.Instance.searchTime, Color.gray, SightManager.Instance.searchTime);
                        }

                        InsertEvent(ev, attitude, itVisionArea);
                        break;
                    }
                    
                }
            }
        }

        bool DoObstaclesBlockVision(Vector3 target)
        {
            // we will change searchDistance based on visibility of obstacles;
            float localSearchDistance = this.searchDistance;

            Vector3 toTarget = target - cachedTransform.position;
            float toTargetSq = toTarget.ToPlane().sqrMagnitude;


            int n = Physics.RaycastNonAlloc(cachedTransform.position, toTarget, StaticCacheLists.raycastHitCache,
                toTarget.magnitude, SightManager.Instance.obstacleMask);

            bool bObstaclesBlocksVision = false;
            for (int i = 0; i < n; ++i)
            {
                var it = StaticCacheLists.raycastHitCache[i];

                PerceiveUnit unit = it.collider.GetComponent<PerceiveUnit>();
                if (!unit)
                {
                    // we assume objects that do not have perceive unit will behave as non transparent
                    // so we can't see our target
                    bObstaclesBlocksVision = true;
                    break;
                }

                if (unit == myUnit)
                    // well, i'm not that fat ... i guess
                    continue;


                localSearchDistance *= unit.transparencyLevel;
                if (localSearchDistance * localSearchDistance < toTargetSq * myUnit.distanceModificator)
                // transparency is reduced too much to see the target
                {
                    bObstaclesBlocksVision = true;
                    break;
                }
            }

            if (drawRayGizmos)
            {
                Debug.DrawRay(cachedTransform.position, toTarget, bObstaclesBlocksVision ? Color.yellow : Color.green, 0.25f);
            }

            return bObstaclesBlocksVision;
        }

        #endregion Search

        protected void OnEnable() => SightManager.Instance.AddSight(this);

        protected void OnDisable() => SightManager.UnsafeInstance?.RemoveSight(this);

        void OnDrawGizmosSelected()
        {
#if UNITY_EDITOR

            UnityEditor.Handles.matrix = Matrix4x4.Translate(transform.rotation * offset.To3D()) * transform.localToWorldMatrix;
            UnityEditor.Handles.DrawWireDisc(Vector3.zero, transform.up, searchDistance);

            UnityEditor.Handles.matrix = Matrix4x4.identity;
            UnityEditor.Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;
            foreach (var it in visionAreas)
            {
                Color cl = it.gizmoColor;
                UnityEditor.Handles.color = cl;

                UnityEditor.Handles.matrix = 
                    Matrix4x4.Translate(transform.position + transform.rotation * it.offset.To3D())
                  * Matrix4x4.Rotate(transform.rotation)
                  * Matrix4x4.Scale(new Vector3(it.scale.x, 1, it.scale.y)) 
                  * Matrix4x4.Rotate(transform.rotation).inverse;

                UnityEditor.Handles.DrawSolidArc(Vector3.zero, transform.up, Quaternion.Euler(0, -it.coneAngle / 2, 0) * transform.forward, it.coneAngle, it.searchDistance);
            }
#endif
        }

    }



    /*public class SenseSight : SenseBase
    {
        public const string HEADER_VISION = "VisionSettings";
        public const string HEADER_SHAPE = "Shape";

        [Serializable]
        public class VisionAreaRecord
        {
            [BoxGroup(AttributeHelper.HEADER_DEBUG)] public string DegugName;

            [BoxGroup(HEADER_VISION)] public bool trackEnemy = true;
            [BoxGroup(HEADER_VISION)] public bool trackAlly = false;
            [BoxGroup(HEADER_VISION)] public bool trackNeutrals = false;

            [BoxGroup(HEADER_SHAPE)] public float coneAngle = 170.0f;
            [BoxGroup(HEADER_SHAPE)] public float searchDistance = 5.0f;

            [BoxGroup(HEADER_SHAPE)] public Vector2 scale;
            [BoxGroup(HEADER_SHAPE)] public Vector2 offset;


            [BoxGroup(AttributeHelper.HEADER_DEBUG)] public Color gizmoColor = new Color(1,0,0,0.25f);
            
        }
        public bool CheckPointCollision(VisionAreaRecord visionRecord, Vector3 pointToCheck)
        {
            pointToCheck -= transform.rotation * visionRecord.offset.To3D();

            Vector2 toIt = (pointToCheck - transform.position).To2D();

            toIt.x /= visionRecord.scale.x;
            toIt.y /= visionRecord.scale.y;

            if (toIt.sqrMagnitude > visionRecord.searchDistance.Sq())
                return false;

            float cosAngle = Vector2.Dot(toIt.normalized, transform.forward.To2D());
            float angle = Mathf.Acos(cosAngle) * 180 / Mathf.PI;
            //Debug.Log(angle);
            bool bProperAngle = angle < visionRecord.coneAngle * 0.5f;
            if (!bProperAngle)
                return false;


            return true;
        }


        
        [BoxGroup(HEADER_VISION)] public bool trackEnemy = true;
        [BoxGroup(HEADER_VISION)] public bool trackAlly = false;
        [BoxGroup(HEADER_VISION)] public bool trackNeutrals = false;

        [BoxGroup(HEADER_SHAPE)] public List<VisionAreaRecord> visionAreas;
        [BoxGroup(HEADER_SHAPE)] public float coneAngle = 170.0f;
        [BoxGroup(HEADER_SHAPE)] public float searchDistance = 5.0f;
        [BoxGroup(HEADER_SHAPE)] public Vector2 offset;

        protected Transform cachedTransform;

        [BoxGroup(AttributeHelper.HEADER_DEBUG)] public bool drawRayGizmos;
        [BoxGroup(AttributeHelper.HEADER_DEBUG)] public Vector3 debugVector;

        // insert given event to the storage specified by an attitiude
        void InsertEvent(MemoryEvent ev, Fraction.EAttitude attitude)
        {
            switch (attitude)
            {
                case Fraction.EAttitude.EEnemy:
                    if (trackEnemy)
                        stimuliSettings.enemyStorage.PerceiveEvent(ev);
                    break;
                case Fraction.EAttitude.EFriendly:
                    if (trackAlly)
                        stimuliSettings.allyStorage.PerceiveEvent(ev);
                    break;
                case Fraction.EAttitude.ENeutral:
                    if (trackNeutrals)
                        stimuliSettings.neutralStorage.PerceiveEvent(ev);
                    break;
            }
        }
        MonoStimuliSettings.StorageSettings GetStorageSettings(Fraction.EAttitude attitude)
        {
            switch (attitude)
            {
                case Fraction.EAttitude.EEnemy:
                    return stimuliSettings.enemyStorageSettings;
                case Fraction.EAttitude.EFriendly:
                    return stimuliSettings.allyStorageSettings;
                case Fraction.EAttitude.ENeutral:
                    return stimuliSettings.neutralStorageSettings;
            }
            return null;
        }

        new void Awake()
        {
            base.Awake();

            // cache transform
            cachedTransform = base.transform;
        }


        #region Search

        public void PerformSearch2D()
        {
            Fraction myFraction = myUnit.fraction;

            // there's no way to determine where to put events
            // do not even bother
            Debug.Assert(myFraction, "No fraction in perceive unit but trying to use sight");

            // perform cast
            int n = Physics2D.OverlapCircleNonAlloc(cachedTransform.position, searchDistance,
                StaticCacheLists.colliderCache2D, SightManager.Instance.memorableMask);

            // preselect targets
            // they have to be in proper angle and contain PerceiveUnit
            for (int i = 0; i < n; ++i)
            {
                var it = StaticCacheLists.colliderCache2D[i];
                Transform itTransform = it.transform;

                //// check if the target is in proper angle
                Vector2 toIt = itTransform.position - cachedTransform.position;
                float cosAngle = Vector2.Dot(toIt.normalized, cachedTransform.up);
                float angle = Mathf.Acos(cosAngle) * 180 / Mathf.PI;
                //Debug.Log(angle);
                bool bProperAngle = angle < coneAngle * 0.5f;
                if (!bProperAngle)
                    continue;

                // ok, now check if it has AiPerceiveUnit
                // we need it's fraction to determine our attitude

                PerceiveUnit perceiveUnit = it.GetComponent<PerceiveUnit>();
                if (perceiveUnit == myUnit)
                    // oh, come on do not look at yourself... don't be soo narcissistic
                    continue;

                if (!perceiveUnit)
                    // no perceive unit, this target is invisible to us
                    continue;

                Fraction itFraction = perceiveUnit.fraction;
                if (!itFraction)
                    // the same as above,
                    continue;

                //// determine attitude
                Fraction.EAttitude attitude = myFraction.GetAttitude(itFraction);

                //// Check if obstacles blocks vision
                foreach(var itObstacleTest in perceiveUnit.obstacleTestTargets)
                    if (DoObstaclesBlockVision2D(itObstacleTest.position))
                        continue;

                //// create event
                var rb = it.attachedRigidbody;
                MemoryEvent ev = new MemoryEvent
                {
                    exactPosition = itTransform.position,
                    forward = itTransform.up,
                    
                    // if collider has rigidbody then take its velocity
                    // otherwise there is no simple way to determine event velocity
                    velocity = rb ? rb.velocity * GetStorageSettings(attitude).velocityPredictionScale : Vector2.zero,
                  
                    // set up agent reponsible for this event
                    perceiveUnit = perceiveUnit
                };
                // ensure event will tick from now on
                ev.lifetimeTimer.Restart();

                if (drawRayGizmos)
                {
                    Debug.DrawRay(ev.exactPosition, Vector3.up, Color.blue, SightManager.Instance.searchTime * GetStorageSettings(attitude).nEvents);
                    Debug.DrawRay(ev.exactPosition, ev.velocity * SightManager.Instance.searchTime, Color.gray, SightManager.Instance.searchTime);
                }

                InsertEvent(ev, attitude);
            }
        }

        bool DoObstaclesBlockVision2D(Vector2 target)
        {
            // we will change searchDistance based on visibility of obstacles;
            float localSearchDistance = this.searchDistance;

            Vector2 toTarget = target - (Vector2) cachedTransform.position;
            float toTargetSq = toTarget.sqrMagnitude;


            int n = Physics2D.RaycastNonAlloc(cachedTransform.position, toTarget, StaticCacheLists.raycastHitCache2D,
                toTarget.magnitude, SightManager.Instance.obstacleMask);

            bool bObstaclesBlocksVision = false;
            for (int i = 0; i < n; ++i)
            {
                var it = StaticCacheLists.raycastHitCache2D[i];
                PerceiveUnit unit = it.collider.GetComponent<PerceiveUnit>();
                if (!unit)
                {
                    // we assume objects that do not have perceive unit will behave as non transparent
                    // so we can't see our target
                    bObstaclesBlocksVision = true;
                    break;
                }

                if (unit == myUnit)
                    // well, i'm not that fat ... i guess
                    continue;


                localSearchDistance *= unit.transparencyLevel;
                if (localSearchDistance * localSearchDistance < toTargetSq * myUnit.distanceModificator)
                    // transparency is reduced too much to see the target
                {
                    bObstaclesBlocksVision = true;
                    break;
                }
            }

            Debug.DrawRay(cachedTransform.position, toTarget, bObstaclesBlocksVision ? Color.yellow : Color.green, 0.25f);


            return bObstaclesBlocksVision;
        }

        public void PerformSearch()
        {
            Fraction myFraction = myUnit.fraction;

            if (!myFraction)
            {
#if UNITY_EDITOR
                Debug.LogWarning("No fraction in perceive unit but trying to use sight");
#endif
                // there's no way to determine where to put events
                return;
            }

            // perform cast
            int n = Physics.OverlapSphereNonAlloc(cachedTransform.position, searchDistance, StaticCacheLists.colliderCache,
                SightManager.Instance.memorableMask);

            // preselect targets
            // they have to be in proper angle and contain PerceiveUnit
            for (int i = 0; i < n; ++i)
            {
                var it = StaticCacheLists.colliderCache[i];
                Transform itTransform = it.transform;

                //// check if the target is in proper angle
                Vector3 toIt = itTransform.position - cachedTransform.position;
                toIt = toIt.To2D();

                float cosAngle = Vector2.Dot(toIt.normalized, cachedTransform.forward.To2D());
                float angle = Mathf.Acos(cosAngle) * 180 / Mathf.PI;
                //Debug.Log(angle);
                bool bProperAngle = angle < coneAngle * 0.5f;
                if (!bProperAngle)
                    continue;

                // ok, now check if it has PerceiveUnit component
                // we need it's fraction to determine our attitude

                PerceiveUnit perceiveUnit = it.GetComponent<PerceiveUnit>();
                if (perceiveUnit == myUnit)
                    // oh, come on do not look at yourself... don't be soo narcissistic
                    continue;

                if (!perceiveUnit)
                    // no perceive unit, this target is invisible to us
                    continue;

                Fraction itFraction = perceiveUnit.fraction;
                if (!itFraction)
                    // the same as above,
                    return;

                //// determine attitude
                Fraction.EAttitude attitude = myFraction.GetAttitude(itFraction);

                //// Check if obstacles blocks vision
                if (DoObstaclesBlockVision(itTransform.position))
                    continue;

                //// create event
                var rb = it.attachedRigidbody;
                MemoryEvent ev = new MemoryEvent
                {
                    exactPosition = itTransform.position,
                    forward = itTransform.forward,

                    // if collider has rigidbody then take its velocity
                    // otherwise there is no simple way to determine event velocity
                    velocity = rb ? rb.velocity * GetStorageSettings(attitude).velocityPredictionScale : Vector3.zero,
                    // set up agent responsible for this event
                    perceiveUnit = perceiveUnit
                };


                // ensure event will tick from now on
                ev.lifetimeTimer.Restart();

                if (drawRayGizmos)
                {
                    Debug.DrawRay(itTransform.position, Vector3.up, Color.blue, SightManager.Instance.searchTime * GetStorageSettings(attitude).nEvents);
                    Debug.DrawRay(itTransform.position, ev.velocity * SightManager.Instance.searchTime, Color.gray, SightManager.Instance.searchTime);
                }

                InsertEvent(ev, attitude);
            }
        }

        public void PerformSearchMultiple()
        {
            Fraction myFraction = myUnit.fraction;

            if (!myFraction)
            {
#if UNITY_EDITOR
                Debug.LogWarning("No fraction in perceive unit but trying to use sight");
#endif
                // there's no way to determine where to put events
                return;
            }

            // perform cast
            int n = Physics.OverlapSphereNonAlloc(cachedTransform.position + cachedTransform.rotation * offset.To3D(), searchDistance, 
                StaticCacheLists.colliderCache, SightManager.Instance.memorableMask);

            // preselect targets
            // they have to be in proper angle and contain PerceiveUnit
            for (int i = 0; i < n; ++i)
            {
                var it = StaticCacheLists.colliderCache[i];
                Transform itTransform = it.transform;


                // ok, now check if it has PerceiveUnit component
                // we need it's fraction to determine our attitude
                PerceiveUnit perceiveUnit = it.GetComponent<PerceiveUnit>();
                if (perceiveUnit == myUnit)
                    // oh, come on do not look at yourself... don't be soo narcissistic
                    continue;

                if (!perceiveUnit)
                    // no perceive unit, this target is invisible to us
                    continue;

                Fraction itFraction = perceiveUnit.fraction;
                if (!itFraction)
                    // the same as above,
                    return;

                //// determine attitude
                Fraction.EAttitude attitude = myFraction.GetAttitude(itFraction);

                foreach(var itVisionArea in visionAreas)
                {
                    bool valid = CheckPointCollision(itVisionArea, it.transform.position);
                    if (!valid)
                        continue;


                }
            }
        }


        bool DoObstaclesBlockVision(Vector3 target)
        {
            // we will change searchDistance based on visibility of obstacles;
            float localSearchDistance = this.searchDistance;

            Vector3 toTarget = target - cachedTransform.position;
            float toTargetSq = toTarget.ToPlane().sqrMagnitude;


            int n = Physics.RaycastNonAlloc(cachedTransform.position, toTarget, StaticCacheLists.raycastHitCache,
                toTarget.magnitude, SightManager.Instance.obstacleMask);

            bool bObstaclesBlocksVision = false;
            for (int i = 0; i < n; ++i)
            {
                var it = StaticCacheLists.raycastHitCache[i];

                PerceiveUnit unit = it.collider.GetComponent<PerceiveUnit>();
                if (!unit)
                {
                    // we assume objects that do not have perceive unit will behave as non transparent
                    // so we can't see our target
                    bObstaclesBlocksVision = true;
                    break;
                }

                if (unit == myUnit)
                    // well, i'm not that fat ... i guess
                    continue;


                localSearchDistance *= unit.transparencyLevel;
                if (localSearchDistance * localSearchDistance < toTargetSq * myUnit.distanceModificator)
                    // transparency is reduced too much to see the target
                {
                    bObstaclesBlocksVision = true;
                    break;
                }
            }

            if (drawRayGizmos)
            {
                Debug.DrawRay(cachedTransform.position, toTarget, bObstaclesBlocksVision ? Color.yellow : Color.green, 0.25f);
            }

            return bObstaclesBlocksVision;
        }

        #endregion Search

        protected void OnEnable() => SightManager.Instance.AddSight(this);
        
        protected void OnDisable() => SightManager.UnsafeInstance?.RemoveSight(this);


        void OnDrawGizmos()//Selected()
        {
#if UNITY_EDITOR
            UnityEditor.Handles.color = Color.red;
            //UnityEditor.Handles.matrix = transform.localToWorldMatrix;

            Vector3 start = Vector3.zero;
            Vector3 end = start + Quaternion.Euler(0, -coneAngle * 0.5f, 0) * Vector3.forward * searchDistance;
            UnityEditor.Handles.DrawLine(start, end);

            end = start + Quaternion.Euler(0, coneAngle * 0.5f, 0) * Vector3.forward * searchDistance;
            UnityEditor.Handles.DrawLine(start, end);


            end = start + Vector3.forward * searchDistance;
            UnityEditor.Handles.DrawLine(start, end);

            UnityEditor.Handles.DrawWireDisc(Vector3.zero, transform.up, searchDistance);


            UnityEditor.Handles.matrix = Matrix4x4.Translate(transform.rotation*offset.To3D()) * transform.localToWorldMatrix;
            UnityEditor.Handles.DrawWireDisc(Vector3.zero, transform.up, searchDistance);

            //UnityEditor.Handles.matrix = Matrix4x4.identity;
            //UnityEditor.Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
            //bool check = CheckPointCollision(visionAreas[1], transform.position + debugVector);


            //UnityEditor.Handles.color = check ? Color.blue : Color.red;
            //UnityEditor.Handles.DrawWireCube(transform.position + debugVector, Vector3.one * 3);

            UnityEditor.Handles.matrix = Matrix4x4.identity;
            UnityEditor.Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;
            foreach (var it in visionAreas)
            {
                UnityEditor.Handles.color = it.gizmoColor;

                UnityEditor.Handles.matrix = Matrix4x4.Translate(transform.position + transform.rotation * it.offset.To3D()) * Matrix4x4.Scale(new Vector3(it.scale.x, 1, it.scale.y));
                
                UnityEditor.Handles.DrawSolidArc(Vector3.zero, transform.up, Quaternion.Euler(0, -it.coneAngle/2, 0) * transform.forward, it.coneAngle, it.searchDistance);
            }
#endif
        }
    }*/
}


    