using System;
using UnityEngine;

[System.Serializable]
public class ImpactTags : MonoBehaviour
{  
	//Default tags for bullet impacts
	public string metalImpactStaticTag = "Metal (Static)";
	public string metalImpactTag = "Metal";
	public string woodImpactStaticTag = "Wood (Static)";
	public string woodImpactTag = "Wood";
	public string concreteImpactStaticTag = "Concrete (Static)";
	public string concreteImpactTag = "Concrete";
	public string dirtImpactStaticTag = "Dirt (Static)";
	public string dirtImpactTag = "Dirt";
	public string explosiveBarrelImpactTag = "ExplosiveBarrel";
    public string enemyImpactStaticTag = "Enemy (Static)";
    public string enemyImpactTag = "Enemy";
}