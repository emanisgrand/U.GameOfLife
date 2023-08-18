using System;using System.Collections;using System.Collections.Generic;using UnityEngine;

/// <summary>
/// This class is used to represent a cell in a game board. 
/// It contains information about the cell's position, state, and other related data. 
/// </summary>
public class Cell : MonoBehaviour {

		[SerializeField] VineRenderer vineRenderer;

		[SerializeField] int maxIterations = 4; // Adjust as needed.

		LSystemVine lSystemVine;

		/// <summary>
		/// Gets or sets a value indicating whether this instance is alive.
		/// </summary>
		public bool IsAlive { get; set; }
		/// <summary>
		/// Property to get and set the number of neighbors.
		/// </summary>
		public int Neighbors { get; set; }

		/// <summary>
		/// Sets the IsAlive property to the given value.
		/// </summary>
		/// <param name="isAlive">The value to set IsAlive to.</param>
		public void SetAlive(bool isAlive) {				IsAlive = isAlive;		}


		/// <summary>
		/// Checks the status of a cell and sets its alive status accordingly.
		/// </summary>
		public void CheckStatus() {				if (IsAlive) {						if (Neighbors < 2 || Neighbors > 3) {								SetAlive(false);						}				} else if (Neighbors == 3) {						SetAlive(true);				}		}}