using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Completed
{
    public class Loader : MonoBehaviour
    {
        public GameObject gameController;

        void Awake()
        {
            if (GameManager.instance == null)
            {
                Instantiate(gameController);
            }
        }
    }
}