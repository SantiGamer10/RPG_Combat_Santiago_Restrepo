using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ActionController : MonoBehaviour
{
    [SerializeField] private GameManager manager;
    [SerializeField] private GameObject rangeButton;

    [SerializeField] private Transform border;
    [SerializeField] private Transform enemyBorder;

    [SerializeField] private float meleeDistance = 1f;
    [SerializeField] private float enemyTimeoutDuration = 3f;

    [SerializeField] private bool enableLogs = true;

    private List<Player> allPlayers = new List<Player>();
    private Player actualPlayer;
    private Player target;

    private Coroutine currentActionCoroutine;

    private IconSetter iconSetter;

    private int round = 0;

    public bool HasChosenAction { private set; get; }

    private void Awake()
    {
        if (!manager)
        {
            Debug.LogError($"{name}: {nameof(manager)} is null!" +
                           $"\nCheck and assign one.");
            enabled = false;
            return;
        }

        if (!rangeButton)
        {
            Debug.LogError($"{name}: {nameof(rangeButton)} is null!" +
                           $"\nCheck and assign one.");
            enabled = false;
            return;
        }

        if (!border)
        {
            Debug.LogError($"{name}: {nameof(border)} is null!" +
                           $"\nCheck and assign one.");
            enabled = false;
            return;
        }

        if (!enemyBorder)
        {
            Debug.LogError($"{name}: {nameof(enemyBorder)} is null!" +
                           $"\nCheck and assign one.");
            enabled = false;
            return;
        }
    }

    public void Initialize(List<Player> players)
    {
        allPlayers.Clear();
        allPlayers.AddRange(players);

        iconSetter = new IconSetter(allPlayers, meleeDistance);
    }

    public void SetCurrentPlayer(Player currentPlayer)
    {
        this.actualPlayer = currentPlayer;
        HasChosenAction = false;

        iconSetter.ResetIcons();

        border.gameObject.SetActive(!currentPlayer.IsEnemy);
        enemyBorder.gameObject.SetActive(currentPlayer.IsEnemy);

        if (currentPlayer.IsEnemy)
        {
            enemyBorder.position = currentPlayer.GetStatBoxPosition();
            StartCoroutine(EnemyAction());
            return;
        }

        border.position = currentPlayer.GetStatBoxPosition();
        rangeButton.SetActive(currentPlayer.GetCanRangeAttack());
    }


    public void OnRangeAttack()
    {
        iconSetter.SetIconsInRange(actualPlayer);

        if (currentActionCoroutine != null)
            StopCoroutine(currentActionCoroutine);

        currentActionCoroutine = StartCoroutine(PlayerAction(actualPlayer.RangeAttack));
    }

    public void OnMeleeAttack()
    {
        iconSetter.SetIconsInMeleeRange(actualPlayer);

        if (currentActionCoroutine != null)
            StopCoroutine(currentActionCoroutine);

        currentActionCoroutine = StartCoroutine(PlayerAction(actualPlayer.MeleeAttack));
    }

    public void OnHeal()
    {
        iconSetter.SetIconsInHealRange(actualPlayer);

        if (currentActionCoroutine != null)
            StopCoroutine(currentActionCoroutine);

        currentActionCoroutine = StartCoroutine(PlayerAction(actualPlayer.Heal));
    }

    public void OnChooseTarget(Player target)
    {
        this.target = target;
    }

    private IEnumerator PlayerAction(Action<Player> action)
    {
        target = null;
        manager.IsWaitingForMovement = false;

        while (target == null)
        {
            yield return new WaitForEndOfFrame();
        }

        action(target);
        HasChosenAction = true;
        currentActionCoroutine = null;
    }

    private IEnumerator EnemyAction()
    {
        float startTime = Time.time;

        yield return new WaitForSeconds(1);

        manager.IsWaitingForMovement = false;

        yield return new WaitForSeconds(1);

        OnMeleeAttack();

        yield return WaitUntilTargetSelectedOrTimeout(enemyTimeoutDuration, startTime);
        yield return new WaitForSeconds(1);
    }

    private IEnumerator WaitUntilTargetSelectedOrTimeout(float duration, float startTime)
    {
        while (target == null && Time.time - startTime < duration)
        {
            ChooseRandomTarget();
            yield return new WaitForEndOfFrame();
        }

        if (target == null)
        {
            if(enableLogs)
                Debug.Log("Target selection timed out.");

            HasChosenAction = true;
            currentActionCoroutine = null;
        }
    }

    private void ChooseRandomTarget()
    {
        Player closestTarget = FindClosestTarget(meleeDistance);

        if (closestTarget == null)
        {
            OnRangeAttack();
            closestTarget = FindClosestTarget(actualPlayer.GetMaxAttackRange());
        }

        if (closestTarget != null)
        {
            round++;

            if (enableLogs)
                Debug.Log(round + " - " + actualPlayer.name + " has Chosen Target: " + closestTarget.name);
        }

        target = closestTarget;
    }

    private Player FindClosestTarget(float maxRange)
    {
        Player closestTarget = null;
        float minDistance = float.MaxValue;
        List<Player> closestTargets = new List<Player>();

        foreach (Player targetPlayer in allPlayers)
        {
            if (!IsValidTarget(targetPlayer))
                continue;

            float distance = Vector2.Distance(actualPlayer.transform.position, targetPlayer.transform.position);

            if (distance <= maxRange)
            {
                UpdateClosestTargets(distance, targetPlayer, ref minDistance, ref closestTargets);
            }
        }

        if (closestTargets.Count > 0)
        {
            closestTarget = closestTargets[Random.Range(0, closestTargets.Count)];
        }

        return closestTarget;
    }

    private bool IsValidTarget(Player player)
    {
        return !player.IsEnemy;
    }

    private void UpdateClosestTargets(float distance, Player targetPlayer, ref float minDistance, ref List<Player> closestTargets)
    {
        if (distance < minDistance)
        {
            minDistance = distance;
            closestTargets.Clear();
            closestTargets.Add(targetPlayer);
        }

        else if (distance == minDistance)
        {
            closestTargets.Add(targetPlayer);
        }
    }
}
