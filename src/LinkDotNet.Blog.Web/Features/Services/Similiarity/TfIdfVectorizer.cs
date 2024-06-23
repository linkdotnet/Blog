using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkDotNet.Blog.Web.Features.Services.Similiarity;

public class TfIdfVectorizer
{
    private readonly IReadOnlyCollection<IReadOnlyCollection<string>> documents;
    private readonly Dictionary<string, double> idfScores;

    public TfIdfVectorizer(IReadOnlyCollection<IReadOnlyCollection<string>> documents)
    {
        this.documents = documents;
        idfScores = CalculateIdfScores();
    }

    public Dictionary<string, double> ComputeTfIdfVector(IReadOnlyCollection<string> targetDocument)
    {
        ArgumentNullException.ThrowIfNull(targetDocument);

        var termFrequency = targetDocument.GroupBy(t => t).ToDictionary(g => g.Key, g => g.Count());
        var tfidfVector = new Dictionary<string, double>();

        foreach (var term in termFrequency.Keys)
        {
            var tf = termFrequency[term] / (double)targetDocument.Count;
            var idf = idfScores.TryGetValue(term, out var score) ? score : 0;
            tfidfVector[term] = tf * idf;
        }

        return tfidfVector;
    }

    private Dictionary<string, double> CalculateIdfScores()
    {
        var termDocumentFrequency = new Dictionary<string, int>();
        var scores = new Dictionary<string, double>();

        foreach (var term in documents.Select(document => document.Distinct()).SelectMany(terms => terms))
        {
            if (!termDocumentFrequency.TryGetValue(term, out var value))
            {
                value = 0;
                termDocumentFrequency[term] = value;
            }
            termDocumentFrequency[term] = ++value;
        }

        foreach (var term in termDocumentFrequency.Keys)
        {
            scores[term] = Math.Log(documents.Count / (double)termDocumentFrequency[term]);
        }

        return scores;
    }
}
