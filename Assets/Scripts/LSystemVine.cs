using System.Collections.Generic;
using System.Text;

public class LSystemVine
{
    string axiom = "F"; // Our starting point, or "seed"
    Dictionary<char, string> rules = new Dictionary<char, string>();
    int iterations = 0;
    string currentPattern;

    public LSystemVine() { 
        // Define rules. Here's one rule.
        // F means move forward; + and - are used for turns
        rules.Add('F', "F+F-F+F"); // Simple rule that replaces 'F' with "F+F-F+F"
    }

    public string GeneratePattern(int maxIterations){
        if (iterations > maxIterations) { return currentPattern; }

        if (currentPattern == null) {
            currentPattern = axiom;
        }
        
        StringBuilder newPattern = new StringBuilder();

        foreach (char c in currentPattern) {
            if (rules.ContainsKey(c)) { 
                newPattern.Append(rules[c]);
            } else {
                newPattern.Append(c);
            }
        }

        currentPattern = newPattern.ToString();
        iterations++;
        return currentPattern;
    }
}
