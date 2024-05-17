using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int maxSpeed;
    [SerializeField] private int healthPoints = 1;

    [SerializeField] private int rangeDamage;
    [SerializeField] private int meleeDamage;
    [SerializeField] private int heal;

    [SerializeField] private float maxRangeAttackDistance = 1.55f;
    [SerializeField] private float maxRangeHealDistance = 1.3f;

    [SerializeField] private bool canOnlyHealSelf = false;
    [SerializeField] private bool canRangeAttack = false;
    [SerializeField] private bool isEnemy = false;

    [SerializeField] private GameObject icon;
    [SerializeField] private Transform statBox;

    public event Action onHPChange = delegate { };
    public event Action onDead = delegate { };

    public bool IsEnemy => isEnemy;

    public int HealthPoints => healthPoints;

    private void Awake()
    {
        if (!icon)
        {
            Debug.LogError($"{name}: {nameof(icon)} is null!" +
                           $"\nCheck and assign one.");
            enabled = false;
            return;
        }

        if (!statBox)
        {
            Debug.LogError($"{name}: {nameof(statBox)} is null!" +
                           $"\nCheck and assign one.");
            enabled = false;
            return;
        }
    }

    public int GetMaxSpeed()
    {
        return maxSpeed;
    }

    public float GetMaxAttackRange()
    {
        return maxRangeAttackDistance;
    }

    public float GetMaxHealRange()
    {
        return maxRangeHealDistance;
    }

    public bool GetCanOnlyHealSelf()
    {
        return canOnlyHealSelf;
    }

    public bool GetCanRangeAttack()
    {
        return canRangeAttack;
    }

    public Vector2 GetStatBoxPosition()
    {
        return statBox.position;
    }

    public void SetIconActive(bool active)
    {
        if (!icon) return;
        icon.SetActive(active);
    }

    public void MeleeAttack(Player targetHP)
    {
        targetHP.ReceiveDamage(meleeDamage);
    }

    public void RangeAttack(Player targetHP)
    {
        targetHP.ReceiveDamage(rangeDamage);
    }

    public void Heal(Player targetHP)
    {
        targetHP.Heal(heal);
    }

    private void ReceiveDamage(int damage)
    {
        healthPoints -= damage;

        if (healthPoints <= 0)
        {
            healthPoints = 0;
            onHPChange?.Invoke();

            Die();

            return;
        }

        onHPChange?.Invoke();
    }

    private void Heal(int addedHP)
    {
        healthPoints += addedHP;
        onHPChange?.Invoke();
    }

    private void Die()
    {
        onDead?.Invoke();
        healthPoints = -1;

        Destroy(icon);

        gameObject.SetActive(false);
    }
}
