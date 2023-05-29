using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Api.Towers;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using MelonLoader;
using Il2CppAssets.Scripts.Unity.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2Cpp;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Utils;
using HarmonyLib;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors.Abilities;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Simulation.Towers.Weapons;
using Il2CppAssets.Scripts.Models.Towers.Filters;

namespace SpecialAgents.towers.AngrySquirrel
{
  class AngrySquirrel : ModTower<SpecialAgentSet>
  {
    public override string BaseTower => TowerType.DartMonkey;
    public override int Cost => 360;

    public override int TopPathUpgrades => 0;
    public override int MiddlePathUpgrades => 1;
    public override int BottomPathUpgrades => 0;
    public override string Description => "Armed with sharp acorns, this special agent goes berserk when bloons leak. For a few seconds, he attacks super fast, can spot camo, and can pop lead.";

    public override string Icon => "AngrySquirrel-000";

    public override bool Use2DModel => true;
    public override string Get2DTexture(int[] tiers)
    {
      return "AngrySquirrel-000";
    }

    public override void ModifyBaseTowerModel(TowerModel towerModel)
    {
      towerModel.radius = 5;
      var attack = towerModel.GetAttackModel();
      attack.weapons[0].projectile.pierce = 1;
      attack.weapons[0].rate = 0.5f;
      attack.weapons[0].projectile.ApplyDisplay<AcornDisplay>();

      var ability = Game.instance.model.GetTower("Alchemist", 0, 4, 0).GetAbility().Duplicate();

      ability.activateOnLivesLost = true;
      ability.cooldown = 10f;
      ability.animationOffset = 0f;
      ability.GetBehavior<ActivateAttackModel>().lifespan = 7f;
      ability.GetBehavior<IncreaseRangeModel>().lifespanFrames = 420;
      ability.GetBehavior<SwitchDisplayModel>().lifespan = 7f;

      ability.GetBehavior<IncreaseRangeModel>().addative = 20f;
      ability.GetBehavior<SwitchDisplayModel>().display = new PrefabReference(GetDisplayGUID<RageDisplay>());

      var abilityAttack = towerModel.GetAttackModel().Duplicate();
      abilityAttack.weapons[0].rate *= 0.33f;
      abilityAttack.weapons[0].projectile.pierce += 2f;
      abilityAttack.weapons[0].projectile.GetDamageModel().damage++;
      abilityAttack.weapons[0].projectile.GetDamageModel().immuneBloonProperties = BloonProperties.None;
      abilityAttack.weapons[0].projectile.SetHitCamo(true);
      abilityAttack.RemoveBehavior<AttackFilterModel>();

      ability.GetBehavior<ActivateAttackModel>().attacks = new AttackModel[1] {abilityAttack};

      towerModel.AddBehavior(ability);
    }
  }

  class AngrySquirrelPro : ModUpgrade<AngrySquirrel>
  {
    public override int Path => MIDDLE;
    public override int Tier => 1;
    public override int Cost => 900;
    public override string Icon => "AngrySquirrel-RagePro";

    public override string Description => "Anger Mismanagement! Despite counselling the squirrel agent is even angrier. He gets so worked up that every 50 acorns he goes berserk even if bloons haven't leaked.";

    public static Dictionary<string, int> AcornCounter = new();
    public override void ApplyUpgrade(TowerModel towerModel)
    {
      towerModel.GetAttackModel().weapons[0].rate *= 0.8f;
      towerModel.GetAbility().GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].rate *= 0.8f;
      towerModel.GetAbility().GetBehavior<SwitchDisplayModel>().display = new PrefabReference(GetDisplayGUID<RageDisplayPro>());
    }

  }

  class AcornDisplay : ModDisplay
  {
    public override string BaseDisplay => Generic2dDisplay;
    public override float Scale => 2f;

    public override void ModifyDisplayNode(UnityDisplayNode node)
    {
      Set2DTexture(node, "acorn");
    }
  }
  class RageDisplay : ModDisplay
  {
    public override string BaseDisplay => Generic2dDisplay;

    public override void ModifyDisplayNode(UnityDisplayNode node)
    {
      Set2DTexture(node, "AngrySquirrel-Rage");
    }
  }
  class RageDisplayPro : ModDisplay
  {
    public override string BaseDisplay => Generic2dDisplay;

    public override void ModifyDisplayNode(UnityDisplayNode node)
    {
      Set2DTexture(node, "AngrySquirrel-RagePro");
    }
  }

  // Patch to count the acorns pro squirrel throws and trigger rage every 50 acorns
  [HarmonyPatch(typeof(Weapon), "Emit")]
  class WeaponEmit_Patch
  {
    [HarmonyPostfix]
    internal static void Postfix(Tower owner)
    {
      //MelonLogger.Msg("patch triggered");
      //MelonLogger.Msg(owner.towerModel.baseId);
      //MelonLogger.Msg(owner.towerModel.tiers);
      //MelonLogger.Msg(owner.uniqueId);
      if (owner.towerModel.baseId == "SpecialAgents-AngrySquirrel" && owner.towerModel.tiers[1] > 0)
      {
        //MelonLogger.Msg("Acorn detected");
        if(!AngrySquirrelPro.AcornCounter.ContainsKey(owner.uniqueId))
        {
          AngrySquirrelPro.AcornCounter[owner.uniqueId] = 1;
        }
        else if(AngrySquirrelPro.AcornCounter[owner.uniqueId] >= 50)
        {
          //MelonLogger.Msg("Ability activated");
          owner.GetTowerBehavior<Ability>().Activate(true);
          AngrySquirrelPro.AcornCounter[owner.uniqueId] = 0;
        }
        else
        {
          AngrySquirrelPro.AcornCounter[owner.uniqueId]++;
        }
        //MelonLogger.Msg("Acorn Counter: "+AngrySquirrelPro.AcornCounter[owner.uniqueId]);
      }
    }
  }
}
