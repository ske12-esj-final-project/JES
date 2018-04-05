using UnityEngine;

public class ImpactConcreteStrategy : ImpactStrategy {

	public override void makeImpact(Prefabs prefabs, RaycastHit hit) {
		Object.Instantiate (prefabs.concreteImpactPrefab, hit.point, 
			Quaternion.FromToRotation (Vector3.forward, hit.normal)); 
	}
}