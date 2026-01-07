using System;

namespace Gameplay.Core
{
    [Serializable]
    public class HeroData
    {
        public int HeroID;
        public string Name;
        public int BaseHP;
        public int BaseAtk;
        public float BaseSpeed;
        public string Description;
        public int BaseMana;
    }

    [Serializable]
    public class SkillData
    {
        public int SkillID;
        public string Name;
        public float Cooldown;
        public int ManaCost;
        public int SkillSlot; // 1=Q, 2=W, 3=E
    }
}