using UnityEngine;

public class Rocket : MonoBehaviour{
    
    //public float mass;
    //public float dipForce;

    public float lifetime;

    public int damage;
    public float explosionRadius;

    public GameObject obj;

    Rigidbody rigidbody;
    public Transform owner;

    public void Start(){
        rigidbody = GetComponent<Rigidbody>();
        //rigidbody.mass = mass;
        Destroy(gameObject, lifetime);
    }

    public void Update(){
        //rigidbody.AddForce(Vector3.down * mass * dipForce * Time.deltaTime, ForceMode.Impulse);
        transform.forward = transform.TransformDirection(transform.InverseTransformDirection(rigidbody.velocity));
    }

    void OnDrawGizmos(){
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    public void OnCollisionEnter(Collision col){
        Vector3 hitPoint = col.GetContact(0).point;
        Instantiate(obj, hitPoint, Quaternion.identity);
        var hits = Physics.OverlapSphere(hitPoint, explosionRadius);
        var mCol = GetComponent<Collider>();
        foreach(var c in hits){
            if(c != mCol){
                var ent = c.transform.GetComponent<LivingEntity>();
                if(ent != null && c.transform != owner){
                    ent.TakeDamage(damage);
                }
                Vector3 forceDir = (c.transform.position - owner.position).normalized;
                if(c.transform.GetComponent<Rigidbody>() != null)
                    c.transform.GetComponent<Rigidbody>().AddForce(forceDir * 400f, ForceMode.Impulse);
            }
        }
        Destroy(gameObject);
    }

}