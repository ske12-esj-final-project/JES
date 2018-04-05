using UnityEngine;

public class ImpactTargetStrategy : ImpactStrategy {

	public override void makeImpact(Prefabs prefabs, RaycastHit hit) {
		Object.Instantiate (prefabs.concreteImpactStaticPrefab, hit.point, 
			Quaternion.FromToRotation (Vector3.forward, hit.normal));

		//Toggle the isHit bool on the target object
		hit.transform.gameObject.GetComponent<TargetScript>().isHit = true;
	}
}