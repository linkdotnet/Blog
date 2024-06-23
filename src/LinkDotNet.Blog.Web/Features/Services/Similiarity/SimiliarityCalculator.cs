using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkDotNet.Blog.Web.Features.Services.Similiarity;

public static class SimilarityCalculator
{
    public static double CosineSimilarity(Dictionary<string, double> vectorA, Dictionary<string, double> vectorB)
    {
        ArgumentNullException.ThrowIfNull(vectorA);
        ArgumentNullException.ThrowIfNull(vectorB);

        var dotProduct = 0d;
        var magnitudeA = 0d;

        foreach (var term in vectorA.Keys)
        {
            if (vectorB.TryGetValue(term, out var value))
            {
                dotProduct += vectorA[term] * value;
            }
            magnitudeA += Math.Pow(vectorA[term], 2);
        }

        var magnitudeB = vectorB.Values.Sum(value => Math.Pow(value, 2));

        return dotProduct / (Math.Sqrt(magnitudeA) * Math.Sqrt(magnitudeB));
    }
}
