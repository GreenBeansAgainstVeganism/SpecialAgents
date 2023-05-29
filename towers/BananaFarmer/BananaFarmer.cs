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
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.PlacementBehaviors;
using Il2CppAssets.Scripts.Models.Map;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors;

namespace SpecialAgents.towers.BananaFarmer
{
  class BananaFarmer : ModTower<SpecialAgentSet>
  {
    public override string BaseTower => TowerType.BananaFarmer;
    public override int Cost => 300;

    public override int TopPathUpgrades => 0;
    public override int MiddlePathUpgrades => 1;
    public override int BottomPathUpgrades => 0;
    public override string Description => "Tired of picking up all those bananas yourself? So is this guy but at least he's paid to do it. Farmer will auto-gather all bananas in his radius so you don't have to.";

    public override string Icon => "BananaFarmer-000";

    public override bool Use2DModel => true;

    public override void ModifyBaseTowerModel(TowerModel towerModel)
    {
      towerModel.displayScale = 2f;
    }
  }

  class BananaFarmerPro : ModUpgrade<BananaFarmer>
  {
    public override int Path => MIDDLE;
    public override int Tier => 1;
    public override int Cost => 480;
    public override string Icon => "BananaFarmer-010";

    public override string Description => "Banana Cannon! Farmer collect bananas from nearby Farms, then shoots the skins onto the track, making bloons slide back toward the entrance.\n(To prevent regrow farms, they will stop shooting bananas for the round after 1 minute.)";

    public override void ApplyUpgrade(TowerModel towerModel)
    {
      var attack = Game.instance.model.GetTower(TowerType.SpikeFactory).GetAttackModel().Duplicate();

      attack.weapons[0].projectile.RemoveBehavior<SetSpriteFromPierceModel>();
      attack.weapons[0].projectile.GetBehavior<ClearHitBloonsModel>().interval = 1.0f;
      attack.weapons[0].projectile.pierce = 4f;
      attack.weapons[0].projectile.RemoveBehavior<DamageModel>();
      attack.weapons[0].projectile.AddBehavior(new WindModel("WindModel_", 50f, 100f, 1.0f, false, ""));
      attack.weapons[0].projectile.ApplyDisplay<PeelDisplay>();
      attack.AddBehavior(new RotateToTargetModel("RotateToTargetModel_",true,false,false,0,true,false));

      towerModel.AddBehavior(attack);

      // Make the banana farmer attack 1000 times slower after 30 seconds to prevent regrow farming
      attack.weapons[0].rate *= 1000f;
      towerModel.AddBehavior(new StartOfRoundRateBuffModel("StartOfRoundRateBuffModel_",0.001f,60f));
    }
  }

  class PeelDisplay : ModDisplay
  {
    public override string BaseDisplay => Generic2dDisplay;
    public override float Scale => 1.2f;

    public override void ModifyDisplayNode(UnityDisplayNode node)
    {
      Set2DTexture(node, "peel");
    }
  }
}
