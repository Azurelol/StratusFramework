using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus.Gameplay
{
	public static class StratusFPS
	{
	}

	public enum StratusFPSWeaponType
	{
		Hitscan,
		Projectile
	}

	public abstract class StratusFPSWeapon : StratusScriptable
	{	
	}

	public abstract class StratusRangedWeapon : StratusFPSWeapon
	{
		public StratusFPSWeaponType weaponType;
	}

	public abstract class StratusProjectileWeapon : StratusFPSWeapon
	{
		public GameObject projectilePrefab;
		public int clipConsumption;
		public int clipSize;
	}

	public abstract class StratusHitscanWeapon : StratusFPSWeapon
	{
	}

}