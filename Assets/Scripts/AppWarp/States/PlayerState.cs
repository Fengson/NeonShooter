﻿using com.shephertz.app42.gaming.multiplayer.client.SimpleJSON;
using NeonShooter.AppWarp.Json;
using NeonShooter.Players;
using NeonShooter.Players.Weapons;
using UnityEngine;

namespace NeonShooter.AppWarp.States
{
    public class PlayerState : IState
    {
        public bool Changed
        {
            get
            {
                return
                    Position != null &&
                    Rotations != null &&
                    SelectedWeapon != null &&
                    ContinousFire != null &&
                    LaunchedProjectiles.Changed;
            }
        }

        public Vector3? Position { get; private set; }
        public Vector2? Rotations { get; private set; }

        public Weapon SelectedWeapon { get; private set; }
        public bool? ContinousFire { get; private set; }

        public ListState<Projectile, ProjectileState> LaunchedProjectiles { get; private set; }

        public PlayerState(Player player)
        {
            player.Position.OnValueChanged += Position_OnValueChanged;
            player.Rotations.OnValueChanged += Rotations_OnValueChanged;

            player.SelectedWeapon.OnValueChanged += SelectedWeapon_OnValueChanged;
            player.ContinuousFire.OnValueChanged += ContinuousFire_OnValueChanged;

            LaunchedProjectiles = new ListState<Projectile, ProjectileState>(player.LaunchedProjectiles, p => new ProjectileState(p));
        }

        public void Clear()
        {
            Position = null;
            Rotations = null;
            SelectedWeapon = null;
            ContinousFire = null;

            LaunchedProjectiles.Clear();
        }

        public JsonObject ToJson()
        {
            var json = new JsonObject();
            if (Position.HasValue) json.Append(new JsonPair("Position", Position.Value.ToJson()));
            if (Rotations.HasValue) json.Append(new JsonPair("Rotations", Rotations.Value.ToJson()));
            if (ContinousFire.HasValue) json.Append(new JsonPair("ContinousFire", ContinousFire.Value));
            return json;
        }

        void Position_OnValueChanged(Vector3 oldValue, Vector3 newValue)
        {
            Position = newValue;
        }

        void Rotations_OnValueChanged(Vector2 oldValue, Vector2 newValue)
        {
            Rotations = newValue;
        }

        void SelectedWeapon_OnValueChanged(Weapon oldValue, Weapon newValue)
        {
            SelectedWeapon = newValue;
        }

        void ContinuousFire_OnValueChanged(bool oldValue, bool newValue)
        {
            ContinousFire = newValue;
        }
    }
}
