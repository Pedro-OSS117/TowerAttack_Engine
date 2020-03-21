using UnityEngine;

// On force le fait que l'entity ai un collider
[RequireComponent(typeof(CapsuleCollider))]
public class Entity : MonoBehaviour
{
    [Header("Global Props")]
    public Alignment alignment;

    public int startLife = 1;
    [SerializeField]
    private int m_CurrentLife = 1;

    public int popAmount = 1;

    [Header("Attack Props")]
    public Alignment typeTarget;
    public GameObject attackContainer;
    public int damageAttack = 1;
    public int rangeDetect = 1;
    [Header("Projectile Props")]
    public bool isProjectileAttack = false;
    public GameObject prefabProjectile;

    [Header("Time Next Attack")]
    [Range(0, 10)]
    public float timeWaitNextAttack = 1;
    private float m_CurrentTimeBeforeNextAttack = 0;
    private bool m_CanAttack = true;

    [Header("Creator Props")]
    public bool isCreatorEntity;
    public GameObject toCreate;
    public int nbrToCreate = 1;
    public float timeWaitNextCreate = 1;
    private float m_CurrentTimeBeforeNextCreate = 0;

    public static Vector3 myPoint = Vector3.zero;

    public void Awake()
    {
        InitEntity();
    }

    // Initialisation - Construction de l'entité
    public virtual void InitEntity()
    {
        RestartEntity();
    }

    // Set de l'entité lorsqu'elle est activée
    // Elle est reset à ses valeurs de depart
    public virtual void RestartEntity()
    {
        CapsuleCollider colliderAttack;
        colliderAttack = attackContainer.GetComponent<CapsuleCollider>();
        colliderAttack.radius = rangeDetect;
        
        m_CurrentLife = startLife;
    }

    public virtual void Update()
    {
        UpdateAttack();

        UpdateCreator();
    }

    #region LIFE
    // Life
    private void SetLife(int amountLife)
    {
        m_CurrentLife = amountLife;
    }

    public void DamageEntity(int damage)
    {
        m_CurrentLife -= damage;
        if(m_CurrentLife <= 0)
        {
            // Entity Die
            //GameObject.Destroy(gameObject);
            PoolManager.Instance.PoolElement(gameObject);
        }
    }

    public bool IsValidEntity()
    {
        return gameObject.activeSelf && m_CurrentLife > 0;
    }
    #endregion LIFE

    #region ATTACK
    // Attack
    private void UpdateAttack()
    {
        if (!m_CanAttack)
        {
            if (m_CurrentTimeBeforeNextAttack < timeWaitNextAttack)
            {
                m_CurrentTimeBeforeNextAttack += Time.deltaTime;
            }
            else
            {
                m_CanAttack = true;
            }
        }
    }

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
            if (entity && entity.alignment == typeTarget)
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
            if (isProjectileAttack)
            {
                GameObject projectile = PoolManager.Instance.GetElement(prefabProjectile);
                Projectile projectileCompo = projectile.GetComponent<Projectile>();
                projectile.transform.position = attackContainer.transform.position;
                projectileCompo.InitTarget(targetEntity);
                projectile.SetActive(true);
            }
            else
            {
                // On applique les degats
                targetEntity.DamageEntity(damageAttack);
            }

            // On set les variables pour l'attente de l'attaque
            m_CanAttack = false;
            m_CurrentTimeBeforeNextAttack = 0;

            SoundManager.Instance.PlayOneShotGlobalSound();
            return true;
        }
        return false;
    }
    #endregion ATTACK

    #region CREATOR
    // Creator 
    private void UpdateCreator()
    {
        if(isCreatorEntity)
        {
            if (m_CurrentTimeBeforeNextCreate < timeWaitNextCreate)
            {
                m_CurrentTimeBeforeNextCreate += Time.deltaTime;
            }
            else
            {
                CreateNewEntity();
            }
        }
    }

    private void CreateNewEntity()
    {
        if (toCreate != null)
        {
            for (int i = 0; i < nbrToCreate; i++)
            {
                EntityManager.Instance.PopElement(toCreate, transform.position);
            }
            m_CurrentTimeBeforeNextCreate = 0;
        }
        else
        {
            Debug.LogError("NO PREFAB SETTED !", gameObject);
        }
    }
    #endregion CREATOR
}
