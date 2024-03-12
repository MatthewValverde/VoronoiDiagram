using System;

public class RandomFloatGenerator
{
    // Random number generator
    private static readonly System.Random random = new System.Random();

    public static float GetRandomFloat()
    {
        // Possible increments in the range [-1.0, 1.0] with 0.5 steps
        float[] increments = new float[] { -1.0f, -0.5f, 0.0f, 0.5f, 1.0f };

        // Get a random index for the increments array
        int index = random.Next(increments.Length);

        // Return the random increment
        return increments[index];
    }
}
