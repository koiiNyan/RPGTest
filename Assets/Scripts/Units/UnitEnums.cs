using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Units
{
    public enum StatsType : byte
    {
        MoveAcceleration = 0,
        SprintAcceleration = 1,
        MoveVelocity = 2,
        SprintVelocity = 3,
        ForceJump = 4,
        Health = 5,
        Mana = 6,
        Stamina = 7,
        HPRegInSec = 8,
        MPRegInSec = 9,
        SPRegInSec = 10,
        Damage = 11,
        CriticalChance = 12,
        ArmorMult = 13,
        RotateSpeed = 14
    }

    public enum UnitAnimationKey : byte
    {
        //todo
	}
}
