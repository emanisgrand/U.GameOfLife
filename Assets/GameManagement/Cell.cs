using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
		[SerializeField] Transform weedTransform;
		
		private Vector3 aliveScale = new Vector3(10.9f, 10.9f, 10.9f);
		private Vector3 deadScale = Vector3.one;
		
		private float lerpSpeed = 3f;
		
		private bool isTransitioning = false;
		private bool targetAliveStatus = false;

		public bool IsAlive { get; set; }
    public int Neighbors { get; set; }
		public int Age { get; private set; } = 0;

		public void SetAlive(bool isAlive){
        if (IsAlive != isAlive){
						isTransitioning = true;
            targetAliveStatus = isAlive;
        }

				// Reset age if cell becomes alive.
				if (isAlive){
						Age = 0;
				}
    }

    public void CheckStatus()
    {
				//Debug.Log($"Checking cell with {Neighbors} neighbors.");
				if (IsAlive) {
            if (Neighbors < 2 || Neighbors > 3){
                SetAlive(false); 
            }
        }else if (Neighbors == 3) {
            SetAlive(true); 
        }
    }

		private void Update() {
				if (isTransitioning){
						Vector3 targetScale = targetAliveStatus ? aliveScale : deadScale;
						weedTransform.localScale = Vector3.Lerp(weedTransform.localScale, targetScale, lerpSpeed * Time.deltaTime);

            float distanceToTarget = Vector3.Distance(weedTransform.localScale, targetScale);

						if (distanceToTarget < 0.1f){
								EndTransition(targetScale);
						}
        }
		}
		private void EndTransition(Vector3 targetScale) {
				weedTransform.localScale = targetScale;
				IsAlive = targetAliveStatus;
				isTransitioning = false;
		}

		internal void InitializeCell() {
				weedTransform.localScale = IsAlive ? aliveScale : deadScale;
		}

		public void IncrementAge(){
				if (IsAlive){ 
						Age++;
				}
		}
}
