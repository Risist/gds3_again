using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Ai
{

    public class Herd : MonoBehaviour
    {
        [SerializeField] private GameObject[] initialMembers;

        // blackboard storing informations commonly known by the whole heard
        // basic way of communication
        public readonly GenericBlackboard blackboard = new GenericBlackboard();

        // Agents controlled by the heard
        public readonly List<BehaviourController> members = new List<BehaviourController>();

        GameObject player;

        protected void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player");

            foreach(var it in initialMembers)
            {
                var controller = it.GetComponentInChildren<BehaviourController>();
                if(controller)
                {
                    AddNewMember(controller);
                }
            }
            initialMembers = null;
        }

        public BehaviourController GetClosestMember(Vector3 position, float maxDistance = 999999)
        {
            float closestDistanceSq = float.MaxValue;
            BehaviourController closestController = null;
            foreach (var it in members)
            {
                if (!it)
                    continue;

                float currentDistSq = (position - it.transform.position).sqrMagnitude;
                if(currentDistSq < closestDistanceSq)
                {
                    closestDistanceSq = currentDistSq;
                    closestController = it;
                }
            }


            return closestDistanceSq < maxDistance.Sq() ? closestController : null;
        }


        public virtual void AddNewMember(BehaviourController member)
        {
            members.Add(member);
            member.blackboard.InitValue<Herd>("herd", () => this );
        }

        public Vector3 GetOutDirection(BehaviourController member, float avoidDistance)
        {
            Vector3 localInfluence = Vector3.zero;
            foreach (var it in members)
            {
                if (it == null || it == member)
                    continue;

                Vector3 toIt = it.transform.position - member.transform.position;
                float sqrMagnitude = toIt.sqrMagnitude;
                if (sqrMagnitude < avoidDistance.Sq())
                {
                    localInfluence += -toIt / sqrMagnitude;
                }
            }
            return localInfluence;
        }
    }

    /*public class AiHerd
    {
        // blackboard storing informations commonly known by the whole heard
        // basic way of communication
        public readonly GenericBlackboard blackboard = new GenericBlackboard();

        // Agents controlled by the heard
        public readonly List<BehaviourController> agents = new List<BehaviourController>();

        public void RegisterHerdMember(BehaviourController agent)
        {
            agents.Add(agent);
        }
        
        public void UnregisterHerdMember(BehaviourController agent)
        {
            agents.Remove(agent);
        }

        public void PerceiveEvent(MemoryEvent perceivedEvent)
        {

        }

        // Patrol path peending

        //TODO some HeardScript mechanism?
        // how to set particular heard behaviours
        // Some "Scripting" Language

        // heard being reactional?



        // Examples of behaviours:

        public enum EAgentRole
        {
            EIdle,      // stands up still or does some unimportant task
            EPatrol,    // walk through a predefined or auto generated path
            ESearch,    // move to the noise source
            EFlee,      // move away from the given place to the safe spot
            ESouround,  // try to move behind the provided agent
            EAttack,    // Attack provided agent
        }

        struct AgentRoleData_Idle
        {
            Vector3 requestedPlace;
            float stayProximity;
        }

        struct AgentRoleData_Patrol
        {
            PatrolPath patrolPath;
        }

        struct AgentRoleData_Search
        {
            MemoryEvent originatedEvent;

            Vector2 requestedPosition;

            float MaxSearchDistance;
        }

        struct AgentRoleData_Flee
        {
            MemoryEvent originatedEvent;


        }

        struct AgentRoleData_Souround
        {
            PerceiveUnit requestedUnit;

            Vector3 requestedPositionOffset;


        }

        struct AgentRoleData_Attack
        {
            PerceiveUnit requestedUnit;

            Vector3 requestedDirectionOffset;
        }

    }*/
}
