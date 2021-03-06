﻿using NeonShooter.Players.Cube;

namespace NeonShooter.Players.Weapons
{
    public class Damage
    {
        public BasePlayer Source { get; private set; }
        public BasePlayer Target { get; private set; }
        public int Amount { get; private set; }
        public CubelingSpawnEffect Effect { get; private set; }

        public Damage(BasePlayer source, BasePlayer target, int amount, CubelingSpawnEffect effect)
        {
            Source = source;
            Target = target;
            Amount = amount;
            Effect = effect;
        }
    }
}
