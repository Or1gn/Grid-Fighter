using Entities.Grid;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Characters
{
    [Serializable]
    public class AnimationSequence
    {
        public CharacterState State;
        public Vector2Int Direction;
        public Sprite[] Frames;
        public float FramesPerSecond = 8f;
        public bool Loop = true;
    }

    [CreateAssetMenu(fileName = "CharacterAnimationSO", menuName = "Game/CharacterAnimationSO")]
    public class CharacterAnimationSO : ScriptableObject
    {
        public List<AnimationSequence> Sequences = new List<AnimationSequence>();

        public AnimationSequence GetSequence(CharacterState state, Vector2Int direction)
        {
            return Sequences.Find(seq => seq.State == state && seq.Direction == direction);
        }
    }
}
