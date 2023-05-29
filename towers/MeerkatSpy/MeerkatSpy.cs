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
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using Il2CppAssets.Scripts.Models.GenericBehaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;

namespace SpecialAgents.towers.MeerkatSpy
{
  class MeerkatSpy : ModTower<SpecialAgentSet>
  {
    public override string BaseTower => TowerType.MonkeyVillage;
    public override int Cost => 1300;

    public override int TopPathUpgrades => 0;
    public override int MiddlePathUpgrades => 1;
    public override int BottomPathUpgrades => 0;
    public override string Description => "Meerkat spy has no attack, but uses his super keen senses to spot Camo Bloons, granting Camo Detection for all towers within his radius.";

    public override string Icon => "MeerkatSpy-000";

    public override bool Use2DModel => true;

    public override void ModifyBaseTowerModel(TowerModel towerModel)
    {
      towerModel.range = 20f;
      towerModel.radius = 6;
      towerModel.RemoveBehavior<RangeSupportModel>();
      towerModel.AddBehavior(new VisibilitySupportModel("VisibilitySupportModel_", true, "MeerkatSpy:Visibility", null, "RadarScannerBuff", "BuffIconVillagex2x"));
      towerModel.GetAttackModel().RemoveBehavior<AttackFilterModel>();
    }
  }

  class MeerkatSpyPro : ModUpgrade<MeerkatSpy>
  {
    public override int Path => MIDDLE;
    public override int Tier => 1;
    public override int Cost => 1050;
    public override string Icon => "MeerkatSpy-010";

    public override string Description => "No longer weaponless, the Meerkat Spy Pro can now blast any single bloon into dust and put the hurt into MOAB-class bloons. Grants camo detection to all towers in his radius.";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
      towerModel.displayScale = 1.5f;

      var attack = Game.instance.model.GetTower("MonkeyVillage", 5, 0, 0).GetAttackModel("AttackModel_Attack_");
      attack.RemoveBehavior<DisplayModel>();
      attack.range = 32f;
      attack.GetBehavior<RotateToTargetModel>().rotateTower = true;

      var laser = Game.instance.model.GetTowerFromId("Adora 10").GetAbilities().Find(a => a.name == "AbilityModel_BallOfLight").GetBehavior<AbilityCreateTowerModel>().towerModel.GetAttackModel().weapons[0].Duplicate();

      laser.rate = 0.8f;
      laser.projectile.pierce = 1;
      laser.projectile.GetDamageModel().damage = 6;
      laser.projectile.SetHitCamo(true);

      // Deal double damage to fortified
      laser.projectile.GetBehavior<DamageModifierForTagModel>().damageAddative = 0;
      laser.projectile.GetBehavior<DamageModifierForTagModel>().damageMultiplier = 2f;

      // Deal bonus damage to moabs
      laser.projectile.AddBehavior(new DamageModifierForTagModel("DamageModifierForTagModel_moabs", "Moabs", 1f, 12f, false, false));

      attack.weapons = new WeaponModel[1] { laser };
      towerModel.AddBehavior(attack);
    }
  }
}
