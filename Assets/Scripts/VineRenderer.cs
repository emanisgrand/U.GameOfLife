using System;
using System.Collections;using System.Collections.Generic;using UnityEngine;

/// <summary>
/// Enum representing the different states of a vine.
/// </summary>
public enum VineState { Growing, Alive, Withering, Dead };

/// <summary>
/// This class is responsible for rendering the vine object in the game.
/// </summary>
public class VineRenderer : MonoBehaviour {
		public VineState CurrentState = VineState.Dead;

		// Line Renderer properties & values
		[SerializeField] LineRenderer lineRenderer;		[SerializeField] float segmentLength = 1.0f;		[SerializeField] float growthSpeed = 0.05f;
		[SerializeField] float angle = 1f; // angle for rotations in degrees

		/// <summary>
		/// Struct to store a position and angle.
		/// </summary>
		/// <param name="pos">The position of the transform.</param>
		/// <param name="angle">The angle of the transform.</param>
		/// <returns>
		/// A TransformInfo object containing the position and angle.
		/// </returns>
		struct TransformInfo {
				public Vector3 position;
				public float angle;

				/// <summary>
				/// Constructor for TransformInfo class.
				/// </summary>
				/// <param name="pos">Position of the object.</param>
				/// <param name="angle">Angle of the object.</param>
				/// <returns>
				/// A TransformInfo object.
				/// </returns>
				public TransformInfo(Vector3 pos, float angle) {
						this.position = pos;
						this.angle = angle;
				}
		}

		public IEnumerator SimulateVine(string pattern, VineState newState){
				if (newState == VineState.Growing){
						CurrentState = VineState.Growing;
						yield return AnimateVineGrowth(pattern);
				} else if (newState == VineState.Withering){
						CurrentState = VineState.Withering;
						yield return ReverseGrowth(pattern);
				}
		}

		private IEnumerator ReverseGrowth(string pattern) {
				for (int i=pattern.Length; i >= 0; i--){
						ReverseRenderPattern(pattern, i);
						yield return new WaitForSeconds(growthSpeed * 2);
				}
		}

		public IEnumerator AnimateVineGrowth(string pattern) {
				for (int i = 0; i < pattern.Length; i++) {
						RenderPattern(pattern, i);
						yield return new WaitForSeconds(growthSpeed); // Adjust value for faster/slower growth
				}
		}

		private void ReverseRenderPattern(string pattern, int segmentIndex) {
				if (segmentIndex < 0) {
						return; // Skip rendering if segmentIndex is below zero.
				}

				Stack<TransformInfo> transformStack = new Stack<TransformInfo>();
				Vector3 currentPos = Vector3.zero;
				float currentAngle = 0f;

				List<Vector3> linePositions = new List<Vector3>();
				linePositions.Add(currentPos);

				int currentSegment = 0;

				foreach (char cmd in pattern){
						if (currentSegment >= segmentIndex) {
								break; // Skip rendering if segmentIndex is beyond pattern length
						}

						switch(cmd){
								// Reverse the commands to "undo" the growth pattern.
								case 'F':
										// Move backward										
										currentPos -= new Vector3(
 										0, 
										segmentLength * Mathf.Sin(currentAngle * Mathf.Deg2Rad),
										-segmentLength * Mathf.Cos(currentAngle * Mathf.Deg2Rad)
										);

										linePositions.Add(currentPos);
										currentSegment--;
										break;
								
								case '+':
										// Turn left instead of right
										currentAngle -= angle;
										break;

								case '-':
										// Turn right instead of left
										currentAngle += angle;
										break;
								case '[':
										// Restore last transform 
										TransformInfo ti = transformStack.Pop();
										currentPos = ti.position;
										currentAngle = ti.angle;
										linePositions.Add(currentPos);
										break;

										case ']':
										// Save current transform
										transformStack.Push(new TransformInfo(currentPos, currentAngle));
										break;
								default: //Handle unexpected chars or do nothing.
										break;
						}
				}

				lineRenderer.positionCount = linePositions.Count;
				lineRenderer.SetPositions(linePositions.ToArray());
		}

		/// <summary>
		/// Renders a pattern based on a given string and segment index.
		/// </summary>
		/// <param name="pattern">The pattern to render.</param>
		/// <param name="segmentIndex">The index of the segment to render.</param>
		public void RenderPattern(string pattern, int segmentIndex) {
				if (segmentIndex >= pattern.Length) {
						return; // Skip rendering if segmentIndex is beyond pattern length
				}

				Stack<TransformInfo> transformStack = new Stack<TransformInfo>();
				Vector3 currentPos = Vector3.zero;
				float currentAngle = 0f;

				List<Vector3> linePositions = new List<Vector3>();
				linePositions.Add(currentPos);

				int currentSegment = 0;

				foreach (char cmd in pattern) {
						if (currentSegment >= segmentIndex) {
								return; // Skip rendering if segmentIndex is beyond pattern length
						}

						switch (cmd) {
								case 'F':
										currentSegment++;
										// Move forward
										currentPos += new Vector3(
												0,
												segmentLength * Mathf.Sin(currentAngle * Mathf.Deg2Rad),
												-segmentLength * Mathf.Cos(currentAngle * Mathf.Deg2Rad));

										linePositions.Add(currentPos);
										break;

								case '+':
										// Turn right
										currentAngle += angle;
										break;

								case '-':
										// Turn left
										currentAngle -= angle;
										break;

								case '[':
										// Save current transform
										transformStack.Push(new TransformInfo(currentPos, currentAngle));
										break;

								case ']':
										// Restore last transform
										TransformInfo ti = transformStack.Pop();
										currentPos = ti.position;
										currentAngle = ti.angle;
										linePositions.Add(currentPos);
										break;

								default: //Handle unexpected chars or do nothing.
										break;
						}
				}

				lineRenderer.positionCount = linePositions.Count;
				lineRenderer.SetPositions(linePositions.ToArray());
		}}