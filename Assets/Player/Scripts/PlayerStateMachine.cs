using UnityEngine;

namespace Player.Scripts
{
    public static class PlayerStateMachine
    {
        public enum PlayerState
        {
            Default,
            BuildMode
        }
        
        public static PlayerState State { get; private set; } = PlayerState.Default;

        public static void ChangeState(PlayerState newPlayerState)
        {
            State = newPlayerState;
            
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            Debug.Log("Changed PlayerState to " + newPlayerState);
        }
    }
}