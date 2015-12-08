using com.shephertz.app42.gaming.multiplayer.client.SimpleJSON;
using NeonShooter.AppWarp.Json;
using NeonShooter.Players;
using NeonShooter.Players.Cube;
using NeonShooter.Players.Weapons;
using NeonShooter.Utils;
using System;
using System.IO;
using UnityEngine;

namespace NeonShooter.AppWarp.States
{
    public class CubelingState : IState
    {
        public const string IdKey = "Id";
        public const string DontLerpKey = "DontLerp";
        public const string PositionKey = "Position";
        public const string RotationKey = "Rotation";
        public const string VelocityKey = "Velocity";

        double cubelingSyncTimer;
        DateTime cubelingSyncLastTime;
        bool useSyncTimer;

        public bool Changed
        {
            get
            {
                if (useSyncTimer)
                {
                    var now = DateTime.Now;
                    var dt = (now - cubelingSyncLastTime).TotalSeconds;
                    cubelingSyncTimer += dt;
                    cubelingSyncLastTime = now;

                    if (cubelingSyncTimer < Globals.DefaultCubelingSyncInterval) return false;
                    
                    cubelingSyncTimer %= Globals.DefaultCubelingSyncInterval;
                }
                return
                    DontLerp.HasValue ||
                    Position.Changed ||
                    Rotation.Changed ||
                    Velocity.Changed;
            }
        }

        public IJsonObject RelativeJson
        {
            get
            {
                var json = new JsonObject();
                json.Append(new JsonPair(IdKey, Id));
                if (DontLerp.HasValue) json.Append(new JsonPair(DontLerpKey, DontLerp.Value));
                if (Position.Changed) json.Append(new JsonPair(PositionKey, Position.RelativeJson));
                if (Rotation.Changed) json.Append(new JsonPair(RotationKey, Rotation.RelativeJson));
                if (Velocity.Changed) json.Append(new JsonPair(VelocityKey, Velocity.RelativeJson));
                return json;
            }
        }

        public IJsonObject AbsoluteJson
        {
            get
            {
                var json = new JsonObject();
                json.Append(new JsonPair(IdKey, Id));
                json.Append(new JsonPair(DontLerpKey, true));
                json.Append(new JsonPair(PositionKey, Position.AbsoluteJson));
                json.Append(new JsonPair(RotationKey, Rotation.AbsoluteJson));
                json.Append(new JsonPair(VelocityKey, Velocity.AbsoluteJson));
                return json;
            }
        }

        // bool (has position) + bool (has rotation) + long (id) + bool (dont lerp) + Vector3 (position) + Quaternion (rotation)
        public int AbsoluteBinarySize { get { return 2 + 8 + 1 + 3 * 4 + 4 * 4; } }

        EnemyPlayer parentEnemy;

        public long Id { get; private set; }

        public bool? DontLerp { get; private set; }

        public BasePropertyState<Vector3, Vector3> Position { get; private set; }
        public BasePropertyState<Quaternion, Quaternion> Rotation { get; private set; }
        public BasePropertyState<Vector3, Vector3> Velocity { get; private set; }

        public CubelingState(JSONNode jsonNode, EnemyPlayer enemy)
        {
            parentEnemy = enemy;

            if (jsonNode != null)
            {
                Id = jsonNode[IdKey].AsLong();

                var jsonDontLerp = jsonNode[DontLerpKey];
                DontLerp = jsonDontLerp == null ? false : jsonDontLerp.AsBool;

                var jsonPosition = jsonNode[PositionKey];
                Position = new PropertyVector3State<Vector3>(
                    jsonPosition, js => js.AsVector3(), (p, s) => p.Value = s);

                var jsonRotations = jsonNode[RotationKey];
                Rotation = new PropertyQuaternionState<Quaternion>(
                    jsonRotations, js => js.AsQuaternion(), (p, s) => p.Value = s);

                var jsonVelocity = jsonNode[VelocityKey];
                Velocity = new PropertyVector3State<Vector3>(
                    jsonVelocity, js => js.AsVector3(), (p, s) => p.Value = s);
            }
        }

        public CubelingState(BinaryReader br, EnemyPlayer enemy)
        {
            parentEnemy = enemy;

            bool hasPosition = br.ReadBoolean();
            bool hasRotation = br.ReadBoolean();

            Id = br.ReadInt64();
            DontLerp = br.ReadBoolean();

            Position = new PropertyVector3State<Vector3>(hasPosition,
                br, _br => _br.ReadVector3(), (p, s) => p.Value = s);
            Rotation = new PropertyQuaternionState<Quaternion>(hasRotation,
                br, _br => _br.ReadQuaternion(), (p, s) => p.Value = s);
        }

        public CubelingState(Cubeling cubeling)
        {
            Id = cubeling.Id;

            DontLerp = true;

            Position = new PropertyVector3State<Vector3>(cubeling.Position, p => p, s => s.ToJson());
            Velocity = new PropertyVector3State<Vector3>(cubeling.Velocity, p => p, s => s.ToJson());
            Rotation = new PropertyQuaternionState<Quaternion>(cubeling.Rotation, p => p, s => s.ToJson());

            cubelingSyncTimer = 0;
            cubelingSyncLastTime = DateTime.Now;
            useSyncTimer = true;
        }

        public void WriteRelativeBinaryTo(BinaryWriter bw)
        {
            bool hasPosition = Position.Changed;
            bool hasRotation = Rotation.Changed;

            bw.Write(hasPosition);
            bw.Write(hasRotation);

            bw.Write(Id);
            bw.Write(DontLerp.HasValue && DontLerp.Value);
            if (hasPosition) bw.WriteRelative(Position);
            if (hasRotation) bw.WriteRelative(Rotation);
        }

        public void WriteAbsoluteBinaryTo(BinaryWriter bw)
        {
            bw.Write(true);
            bw.Write(true);

            bw.Write(Id);
            bw.Write(true);
            bw.WriteAbsolute(Position);
            bw.WriteAbsolute(Rotation);
        }

        public void ClearChanges()
        {
            DontLerp = null;

            Position.ClearChanges();
            Rotation.ClearChanges();
            Velocity.ClearChanges();
        }

        public void ApplyTo(object o)
        {
            var cubeling = o as EnemyCubeling;
            if (cubeling != null) ApplyTo(cubeling);
        }

        public void ApplyTo(EnemyCubeling cubeling)
        {
            cubeling.DontLerp = DontLerp.HasValue && DontLerp.Value;
            if (Position.Changed) Position.ApplyTo(cubeling.Position);
            if (Rotation.Changed) Rotation.ApplyTo(cubeling.Rotation);
            if (Velocity.Changed) Velocity.ApplyTo(cubeling.Velocity);
        }

        public EnemyCubeling CreateCubeling()
        {
            if (parentEnemy == null || !DontLerp.HasValue || DontLerp.Value == false) return null;

            var cubelingObject = parentEnemy.SpawnCubeling(Position.Value, Rotation.Value, Velocity.Value);
            var cubeling = cubelingObject.GetComponent<EnemyCubeling>();
            cubeling.Id = Id;
            parentEnemy.CubelingsById[Id] = cubeling;
            return cubeling;
        }
    }
}
