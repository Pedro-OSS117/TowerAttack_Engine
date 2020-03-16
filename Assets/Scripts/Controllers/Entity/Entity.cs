using UnityEngine;

// On force le fait que l'entity ai un collider
[RequireComponent(typeof(CapsuleCollider))]
public class Entity : MonoBehaviour
{
    [Header("Props")]
    public Alignment alignment;

    public int life = 1;

    public int popAmount = 1;

    [Header("AttackProps")]
    public GameObject attackContainer;
    public int damageAttack = 1;
    public int rangeDetect = 1;

    [Header("Time Next Attack")]
    [Range(0, 10)]
    public float timeWaitNextAttack = 1;
    private float m_CurrentTimeBeforeNextAttack = 0;
    private bool m_CanAttack = true;

    public virtual void Awake()
    {
        InitEntity();
    }

    public void InitEntity()
    {
        CapsuleCollider colliderAttack = attackContainer.GetComponent<CapsuleCollider>();
        colliderAttack.radius = rangeDetect;
    }

    public virtual void Update()
    {        
        if(!m_CanAttack)
        {
            if(m_CurrentTimeBeforeNextAttack < timeWaitNextAttack)
            {
                m_CurrentTimeBeforeNextAttack += Time.deltaTime;
            }
            else
            {
                m_CanAttack = true;
            }
        }
    }

    // Life
    private void SetLife(int amountLife)
    {
        life = amountLife;
    }

    private void DamageEntity(int damage)
    {
        life -= damage;
        if(life <= 0)
        {
            // Entity Die
            GameObject.Destroy(gameObject);
        }
    }

    private bool IsValidEntity()
    {
        return gameObject.activeSelf && life > 0;
    }

    // Attack
    private void OnTriggerStay(Collider other)
    {
        if (m_CanAttack)
        {
            //Debug.Log($"Ontrigger {name}: ", other.gameObject);
            DetectTarget(other.gameObject);
        }
    }

    private void DetectTarget(GameObject target)
    {
        // Verification si bon layer
        if(target.gameObject.layer == LayerMask.NameToLayer("Damageable"))
        {
            // Recuperation de l'entity pour tester l'alignement
            Entity entity = target.GetComponent<Entity>();
            if (entity && entity.alignment != alignment)
            {
                //Debug.Log("Can Hit This");
                DoAttack(entity);
            }
        }
    }

    protected virtual bool DoAttack(Entity targetEntity)
    {
        // On verifie si l'entity est valide
        if(targetEntity.IsValidEntity())
        {
            // On applique les degats
            targetEntity.DamageEntity(damageAttack);

            // On set les variables pour l'attente de l'attaque
            m_CanAttack = false;
            m_CurrentTimeBeforeNextAttack = 0;
            return true;
        }
        return false;
    }
}
