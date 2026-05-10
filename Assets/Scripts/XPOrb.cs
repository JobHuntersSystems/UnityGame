using UnityEngine;

public class XPOrb : MonoBehaviour
{    
    public enum TypeXP
    {
        Basic,
        Enhanced,
        Superior
    }
    [Header("variables")]
    public TypeXP OrbType;
    public int XPValue;
    public float attractionRange;
    public float moveSpeed = 5;
    private Transform playerTransform;
    private PlayerXP playerXP;

    void Awake()
    {
        switch (OrbType)
        {
            case TypeXP.Basic:
                XPValue = 1;
            break;
            case TypeXP.Enhanced:
                XPValue = 5;
            break;
            case TypeXP.Superior:
                XPValue = 10;
            break;
        }
    }

    void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        playerXP = playerTransform.GetComponent<PlayerXP>();
    }
    void Update()
    {
        float distance = Vector2.Distance(transform.position, playerTransform.position);
        if(distance < attractionRange)
        {
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerXP.AddXP(XPValue);
            Destroy(gameObject);
        }
    }
}
