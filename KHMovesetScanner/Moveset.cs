using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KHMovesetMemory
{
    public class Moveset
    {
        private List<int> _addressReferences;
        private int _barStart;
        private MovesetAnimation _anim;
        private MovesetEffect _effect;
        private string _name;
        private int _slotnr;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public MovesetAnimation Animation
        {
            get
            {
                return _anim;
            }
            set
            {
                _anim = value;
            }
        }

        public MovesetEffect Effect
        {
            get
            {
                return _effect;
            }
            set
            {
                _effect = value;
            }
        }

        public List<int> AddressReferences
        {
            get
            {
                return _addressReferences;
            }
        }

        public int ANBAddress
        {
            get
            {
                return _barStart;
            }
            set
            {
                _barStart = value;
            }
        }

        public int SlotNumber
        {
            get
            {
                return _slotnr;
            }
            set
            {
                _slotnr = value;
            }
        }

        public Moveset(string name, MovesetAnimation anim, MovesetEffect effect)
        {
            _name = name;
            _anim = anim;
            _effect = effect;
            _addressReferences = new List<int>();
        }

        public Moveset()
        {
            _addressReferences = new List<int>();
        }
    }

    public struct MovesetAnimation
    {
        public int Address;
        public int Value;
    }

    public struct MovesetEffect
    {
        public int Address;
        public int Value;
        public MovesetEffectBoneStructure BoneStructure;
        public List<MovesetEffectDataModifierRaw> DataModifiers;
        public ECasterList EffectCasterList;
    }

    public struct MovesetEffectBoneStructure
    {
        public int Address;
        public int Value;
    }

    public struct MovesetEffectDataModifierRaw
    {
        public int Address { get; set; }
        public int Value { get; set; }

        public int DisplayNumber { get; set; }
    }

    public struct ECasterList
    {
        public byte[] Header { get; set; }
        public int Group1Entries { get; set; }
        public int Group2Entries { get; set; }
        public int Offset4Group2 { get; set; }
        public List<ECaster> Children;
    }

    public struct ECaster
    {
        public int StartAddress { get; set; }
        public ECasterGroup Group { get; set; }
        public short StartAnimationFrame { get; set; }
        public short EndAnimationFrame { get; set; }
        public byte EffectType { get; set; }
        public byte[] AdditionalBytes { get; set; }
        public int Length { get; set; }
    }

    public enum ECasterGroup
    {
        Group1,
        Group2
    }
    
    public enum Group1Effects : byte
    {
        Undefined = 0x00,
        AllowControlsIdle = 0x00,
        AllowControlsBlock = 0x01,
        AllowControls = 0x02,
        BlockANBDisableGravity = 0x03,
        BlockControlsAllowGravity = 0x04,
        ActivateHitbox = 0x0A,
        PerformRCCurrentModel = 0x14,
        DrawAdditionalTexture = 0x17,
        PerformRCAnotherModel = 0x19,
        BlockEverything = 0x1E,
        MakeInvincible = 0x1B,
        BlockRC = 0x22,
        AllowControlsDisableModelRotation = 0x29,
        Unknown
    }

    public enum Group2Effects : byte
    {
        Undefined = 0x00,
        PlayPAXSprite = 0x01,
        PlayFootstepSound = 0x02,
        Play628thANBIndex = 0x03,
        PlayEnemyVSBVoice = 0x0D,
        PlayAllyVSBVoice = 0x0E,
        LetKeybladeAppear = 0x16,
        DecreaseModelOpacity = 0x17,
        LetMashAppear = 0x1A,
        LetMashDisappear = 0x1B,
        PlayKeybladeAppearanceSprite = 0x1D,
        Unknown
    }
}
