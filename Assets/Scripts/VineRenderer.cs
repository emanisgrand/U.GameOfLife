using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum VineState { Growing, Alive, Withering, Dead };

public class VineRenderer : MonoBehaviour {
    // Line Renderer properties & values
    [SerializeField] LineRenderer lineRenderer;
		[SerializeField] float segmentLength = 1.0f;
		[SerializeField] float angle = 25f; // angle for rotations in degrees
		[SerializeField] float growthSpeed = 0.05f;

		[SerializeField] Material witheringMat;
    float currentLerpTime = 0.0f;
    float witheringDuration = 5.0f;
    
    struct TransformInfo{
        public Vector3 position;
        public float angle;

        public TransformInfo(Vector3 pos, float angle){
            this.position = pos;
            this.angle = angle;
        }
    }

    /// <summary>
    /// Call to start the withering effect.
    /// </summary>
    public IEnumerator WitherVineOverTime()
    {
        while(currentLerpTime < witheringDuration){
            currentLerpTime += Time.deltaTime;
            float lerpValue = currentLerpTime / witheringDuration;

            witheringMat.SetFloat("_LerpValue", lerpValue);
						
            yield return null;
				}
        
        // Ensure it's fully withered by the end.
        witheringMat.SetFloat("_LerpValue", 1.0f);
    }

    public IEnumerator AnimateVineGrowth(string pattern){
        GameManager.instance.onVineGrowthStart?.Invoke();

        for (int i=0; i <pattern.Length; i++){
            RenderPattern(pattern, i);
            yield return new WaitForSeconds(growthSpeed); // Adjust value for faster/slower growth
        }

        GameManager.instance.onVineGrowthEnd?.Invoke();
    }

    public void RenderPattern(string pattern, int segmentIndex) {
        Stack<TransformInfo> transformStack = new Stack<TransformInfo>();    
        Vector3 currentPos = Vector3.zero;
        float currentAngle = 0f;

        List<Vector3> linePositions = new List<Vector3>();
        linePositions.Add(currentPos);

        int currentSegment = 0;
        foreach (char cmd in pattern) {
            if (currentSegment > segmentIndex) {
                break;
            }

            switch(cmd){
                case 'F': 
                    currentSegment++;
                    // Move forward
                    currentPos += new Vector3( 
                    0, 
                    segmentLength * Mathf.Sin(currentAngle * Mathf.Deg2Rad), 
                    segmentLength * Mathf.Cos(currentAngle * Mathf.Deg2Rad));
                    
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

                default://Handle unexpected chars or do nothing.
                        break;
            }
        }

        lineRenderer.positionCount = linePositions.Count;
        lineRenderer.SetPositions(linePositions.ToArray());
    }
}
