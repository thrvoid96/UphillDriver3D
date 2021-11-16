using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Combine same behaviours between the Player and AI.

namespace Behaviours
{
    public abstract class CommonBehaviours : MonoBehaviour
    {
        #region SerializeFields
        [SerializeField] private int playerNum;
        [SerializeField] private int currentGrid;
        private Stack<GameObject> blockStack = new Stack<GameObject>();


        #endregion


        #region Components       

        protected Animator animator;

        #endregion


        #region Variables

        public int getCurrentGrid
        {
            get { return currentGrid; }
        }

        public int getPlayerNum
        {
            get { return playerNum; }
        }


        public int blockStackCount
        {
            get { return blockStack.Count; }
        }


        #endregion


        protected virtual void Awake()
        {
            animator = transform.parent.GetComponent<Animator>();
        }

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {

        }

    }           
    
}