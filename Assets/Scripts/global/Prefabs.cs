using System;
using UnityEngine;

public class Prefabs : MonoBehaviour {
	[Header("Metal")]
	[Header("Bullet Impacts & Tags")]
	public Transform metalImpactStaticPrefab;
	public Transform metalImpactPrefab;
	[Header("Wood")]
	public Transform woodImpactStaticPrefab;
	public Transform woodImpactPrefab;
	[Header("Concrete")]
	public Transform concreteImpactStaticPrefab;
	public Transform concreteImpactPrefab;
	[Header("Dirt")]
	public Transform dirtImpactStaticPrefab;
	public Transform dirtImpactPrefab;
    [Header("Enemy")]
    public Transform enemyImpactStaticPrefab;
    public Transform enemyImpactPrefab;
}
