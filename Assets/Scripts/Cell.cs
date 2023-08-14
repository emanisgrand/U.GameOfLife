using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour {
		[SerializeField] VineRenderer vineRenderer;

		[SerializeField] int maxIterations = 4; // Adjust as needed.

		LSystemVine lSystemVine;

		private float lerpSpeed = 3f;

		private bool isTransitioning = false;
		private bool targetAliveStatus = false;

		public bool IsAlive { get; set; }
		public int Neighbors { get; set; }
		public int Age { get; private set; } = 0;
		const int maxAge = 5;

		public void SetAlive(bool isAlive) {
				if (IsAlive != isAlive) {
						isTransitioning = true;
						targetAliveStatus = isAlive;
				}

				// Reset age if cell becomes alive.
				if (isAlive) {
						Age = 0;
						GrowVine();
				}else {
						WitherVine();
				}
		}

		private void WitherVine() {
				if (vineRenderer == null) return;		
				StartCoroutine(vineRenderer.WitherVineOverTime());
		}

		private void GrowVine() {
				if (vineRenderer == null || lSystemVine == null) return;

				string pattern = lSystemVine.GeneratePattern(Age + 1);
				StartCoroutine(vineRenderer.AnimateVineGrowth(pattern));
		}

		public void CheckStatus() {
				//Debug.Log($"Checking cell with {Neighbors} neighbors.");
				if (IsAlive) {
						if (Neighbors < 2 || Neighbors > 3) {
								SetAlive(false);
						}
				} else if (Neighbors == 3) {
						SetAlive(true);
				}
		}

		private void Update() {
				if (isTransitioning) {
						
				}
		}
		
		internal void InitializeCell() {
				//weedTransform.localScale = IsAlive ? aliveScale : deadScale;

		}

		public void IncrementAge() {
				if (IsAlive && Age < maxAge) {
						Age++;
				}
		}
}
