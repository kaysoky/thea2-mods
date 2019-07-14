using DBDef;
using System.Collections.Generic;
using TheHoney;
using Thea2.Common;
using Thea2.General;

namespace GameScript
{
    public class ActivationScripts : ScriptBase
    {
        // Helper that applies normal damage and shield leech (affected by leechFactor).
        static public object Act_ShieldLeech_Helper(NetBattlefield bf, NetQueueItem q, FInt leechFactor)
        {
            NetSkill ns = q.GetNetSkill(bf);
            NetCard owner = bf.GetCardByID(ns.OwnerCardID);

            NetCard target = null;
            if (NetType.IsNullOrEmpty(q.Targets)) return null;

            FInt damage = owner.GetSkillCastingStrength(ns);
            int shieldLeeched = 0;

            // Primary targets.
            foreach (var v in q.Targets.value)
            {
                target = bf.ConvertPositionIDToCard(v);
                if (target != null)
                {
                    target.ReciveNormalDamage(damage, bf, q, v);
                    shieldLeeched += (damage * leechFactor).ToInt();
                }
            }

            // Secondary targets.
            if (!NetType.IsNullOrEmpty(q.SecondaryTargets))
            {
                damage *= 0.5f;

                foreach (var v in q.SecondaryTargets.value)
                {
                    target = bf.ConvertPositionIDToCard(v);
                    if (target != null)
                    {
                        target.ReciveNormalDamage(damage, bf, q, v);
                        shieldLeeched += (damage * leechFactor).ToInt();
                    }
                }
            }

            if (shieldLeeched > 0)
            {
                FInt currentShields = owner.GetCA_SHIELD();
                owner.SetCA_SHIELD(currentShields + shieldLeeched);
            }

            return null;
        }

        static public object Act_ShieldLeech_Essence(NetBattlefield bf, NetQueueItem q, List<NetQueueItem> stack, List<NetQueueItem> previousItems, MHRandom random)
        {
            return Act_ShieldLeech_Helper(bf, q, 0.5f);
        }

        static public object Act_ShieldLeech_Ancient(NetBattlefield bf, NetQueueItem q, List<NetQueueItem> stack, List<NetQueueItem> previousItems, MHRandom random)
        {
            return Act_ShieldLeech_Helper(bf, q, 1.0f);
        }

        // Ancient version of the built-in script Act_DrainHealthEssence, which leeches 80% instead.
        // Note: Built-in script Act_DrainHealthAncient does not appear to do this.
        static public object Act_LifeLeech_Ancient(NetBattlefield bf, NetQueueItem q, List<NetQueueItem> stack, List<NetQueueItem> previousItems, MHRandom random)
        {
            NetSkill ns = q.GetNetSkill(bf);
            NetCard owner = bf.GetCardByID(ns.OwnerCardID);

            NetCard target = null;
            if (NetType.IsNullOrEmpty(q.Targets)) return null;

            FInt damage = owner.GetSkillCastingStrength(ns);
            int lifeLeeched = 0;

            // Primary targets.
            foreach (var v in q.Targets.value)
            {
                target = bf.ConvertPositionIDToCard(v);
                if (target != null)
                {
                    target.ReciveNormalDamage(damage, bf, q, v);
                    lifeLeeched += (damage * 0.8f).ToInt();
                }
            }

            // Secondary targets.
            if (!NetType.IsNullOrEmpty(q.SecondaryTargets))
            {
                damage *= 0.5f;

                foreach (var v in q.SecondaryTargets.value)
                {
                    target = bf.ConvertPositionIDToCard(v);
                    if (target != null)
                    {
                        target.ReciveNormalDamage(damage, bf, q, v);
                        lifeLeeched += (damage * 0.8f).ToInt();
                    }
                }
            }

            if (lifeLeeched > 0)
            {
                owner.ReciveHealthNormal(lifeLeeched, bf, q, t);
            }

            return null;
        }

        // TODO: I'm not sure if this needs a special import to reference the built-in script.
        static public object Act_LifeAndShieldLeech_Essence(NetBattlefield bf, NetQueueItem q, List<NetQueueItem> stack, List<NetQueueItem> previousItems, MHRandom random)
        {
            Act_DrainHealthEssence(bf, q, stack, previousItems, random);
            return Act_ShieldLeech_Essence(bf, q, stack, previousItems, random);
        }

        static public object Act_LifeAndShieldLeech_Ancient(NetBattlefield bf, NetQueueItem q, List<NetQueueItem> stack, List<NetQueueItem> previousItems, MHRandom random)
        {
            Act_LifeLeech_Ancient(bf, q, stack, previousItems, random);
            return Act_ShieldLeech_Ancient(bf, q, stack, previousItems, random);
        }
    }
}
