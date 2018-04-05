using UnityEngine;

public class ImpactDirtStrategy : ImpactStrategy {

	public override void makeImpact(Prefabs prefabs, RaycastHit hit) {
		Object.Instantiate (prefabs.dirtImpactPrefab, hit.point, 
			Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
	}
}