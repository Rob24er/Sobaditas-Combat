using UnityEngine;
using TMPro;

public class CombatAttackUI : MonoBehaviour
{
    public PlayerAttack playerAttack;
    public TMP_Text playerAttackText;

    public EnemyAttack enemyAttack;
    public TMP_Text enemyAttackText;

    int lastPType = 0;
    int lastPHeight = 0;

    int lastEType = 0;
    int lastEHeight = 0;

    void Awake()
    {
        if (playerAttackText != null) playerAttackText.text = "";
        if (enemyAttackText != null) enemyAttackText.text = "";
    }

    void Update()
    {
        UpdateOne(playerAttack, playerAttackText, ref lastPType, ref lastPHeight);
        UpdateOne(enemyAttack, enemyAttackText, ref lastEType, ref lastEHeight);
    }

    void UpdateOne(object attackObj, TMP_Text text, ref int lastType, ref int lastHeight)
    {
        if (attackObj == null || text == null) return;

        int type;
        int height;

        if (attackObj is PlayerAttack pa)
        {
            type = pa.CurrentAttackType;
            height = pa.CurrentAttackHeight;
        }
        else if (attackObj is EnemyAttack ea)
        {
            type = ea.CurrentAttackType;
            height = ea.CurrentAttackHeight;
        }
        else return;

        if (type == lastType && height == lastHeight) return;

        lastType = type;
        lastHeight = height;

        text.text = GetAttackLabel(type, height);
    }

    string GetAttackLabel(int currentAttackType, int currentAttackHeight)
    {
        if (currentAttackType == 0)
        {
            if (currentAttackHeight == 0) return "LeftHandDown";
            if (currentAttackHeight == 1) return "LeftHandMid";
            return "LeftHandUp";
        }
        else if (currentAttackType == 1)
        {
            if (currentAttackHeight == 0) return "RightHandDown";
            if (currentAttackHeight == 1) return "RightHandMid";
            return "RightHandUp";
        }
        else
        {
            if (currentAttackHeight == 0) return "LegDown";
            if (currentAttackHeight == 1) return "LegMid";
            return "LegUp";
        }
    }
}