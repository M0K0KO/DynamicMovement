using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatManager : MonoBehaviour
{
    private PlayerManager playerManager;

    public GameObject leftHandHitBox, rightHandHitBox, leftLegHitBox, rightLegHitBox, groundImpactHitBox;


    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
    }

    public void EnableHitBox(string hitBoxName)
    {
        switch (hitBoxName)
        {
            case "LeftHand":
                leftHandHitBox.SetActive(true);
                break;
            case "RightHand":
                rightHandHitBox.SetActive(true);
                break;
            case "LeftLeg":
                leftLegHitBox.SetActive(true);
                break;
            case "RightLeg":
                rightLegHitBox.SetActive(true);
                break;
            case "GroundImpact":
                groundImpactHitBox.SetActive(true);
                break;
            default:
                Debug.Log("Invalid hit box name");
                break;
        }
    }
    
    public void DisableHitBox(string hitBoxName)
    {
        switch (hitBoxName)
        {
            case "LeftHand":
                leftHandHitBox.SetActive(false);
                break;
            case "RightHand":
                rightHandHitBox.SetActive(false);
                break;
            case "LeftLeg":
                leftLegHitBox.SetActive(false);
                break;
            case "RightLeg":
                rightLegHitBox.SetActive(false);
                break;
            case "GroundImpact":
                groundImpactHitBox.SetActive(false);
                break;
            default:
                Debug.Log("Invalid hit box name");
                break;
        }
    }

    public void DisableAllHitBox()
    {
        leftHandHitBox.SetActive(false);
        rightHandHitBox.SetActive(false);
        leftLegHitBox.SetActive(false);
        rightLegHitBox.SetActive(false);
        groundImpactHitBox.SetActive(false);
    }

    private void OnDrawGizmos()
    {
            Gizmos.color = Color.red;

            Matrix4x4 originalMatrix = Gizmos.matrix;

            DrawRotatedWireCube(leftHandHitBox);
            DrawRotatedWireCube(rightHandHitBox);
            DrawRotatedWireCube(leftLegHitBox);
            DrawRotatedWireCube(rightLegHitBox);

            if (groundImpactHitBox.activeSelf)
            {
                Gizmos.DrawWireSphere(groundImpactHitBox.transform.position, groundImpactHitBox.transform.localScale.x);
            }

            Gizmos.matrix = originalMatrix;
    }
    
    void DrawRotatedWireCube(GameObject obj)
    {
        if (obj != null && obj.activeSelf)
        {
            Gizmos.matrix = obj.transform.localToWorldMatrix;

            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }
    }
}
