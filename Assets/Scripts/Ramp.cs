using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ramp : MonoBehaviour
{
    [SerializeField] private int blocksNeededToClimb;

    public int getBlocksNeededToClimb => blocksNeededToClimb;
}
