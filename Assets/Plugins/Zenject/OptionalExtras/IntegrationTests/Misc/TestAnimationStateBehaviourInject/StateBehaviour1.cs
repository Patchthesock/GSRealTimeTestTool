using UnityEngine;
using ModestTree;

namespace Zenject.Tests.TestAnimationStateBehaviourInject
{
    public class StateBehaviour1 : StateMachineBehaviour
    {
        public static int OnStateEnterCalls = 0;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            OnStateEnterCalls++;
        }
    }
}
