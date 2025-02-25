using UnityEngine;
using UnityEngine.Serialization;

public class UnitRagdoll : MonoBehaviour
{
    [SerializeField] private Transform ragdollRootBoon;

    public void Setup(Transform originalRootBone)
    {
        MatchAllChildrenTransforms(originalRootBone,ragdollRootBoon);

        Vector3 randomDir = new Vector3(Random.Range(-1f,1f),0,Random.Range(-1f,1f));
        ApplyExplosionToRagdoll(ragdollRootBoon, 300f, transform.position + randomDir, 10f);
    }

    private void MatchAllChildrenTransforms(Transform root, Transform clone)
    {
        foreach (Transform child in root)
        {
            Transform cloneChild = clone.Find(child.name);
            if (cloneChild != null)
            {
                cloneChild.position = child.position;
                cloneChild.rotation = child.rotation;
                
                MatchAllChildrenTransforms(child, cloneChild);
            }
        }
    }

    private void ApplyExplosionToRagdoll(Transform root , float explosionForce, Vector3 explosionPosition, float explosionRange)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
            {
                childRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);   
            }
            
            ApplyExplosionToRagdoll(child, explosionForce, explosionPosition, explosionRange);
        }
    }
    
}
