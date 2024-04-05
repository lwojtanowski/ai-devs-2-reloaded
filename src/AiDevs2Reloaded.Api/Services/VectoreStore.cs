using AiDevs2Reloaded.Api.Services.Abstractions;
using System.Text.Json;

namespace AiDevs2Reloaded.Api.Services;

public class VectoreStore : IVectoreStore
{
    private readonly ILogger<VectoreStore> _logger;

    public VectoreStore(ILogger<VectoreStore> logger)
    {
        _logger = logger;
    }

    public async Task StoreAsync(string collectionName, IEnumerable<VectoreStoreDto> vectors, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(vectors);
        await File.WriteAllTextAsync($"{collectionName}.json", json, cancellationToken);
    }

    public async Task<IReadOnlyList<VectoreStoreDto>> TryGetCollectionAsync(string collectionName, CancellationToken cancellationToken = default)
    {
        if (File.Exists($"{collectionName}.json"))
        {
            var json = await File.ReadAllTextAsync($"{collectionName}.json", cancellationToken);
            var vectors = JsonSerializer.Deserialize<List<VectoreStoreDto>>(json);

            if (vectors is null)
            {
                _logger.LogError("Failed to deserialize the collection {CollectionName}", collectionName);
                return Enumerable.Empty<VectoreStoreDto>().ToList().AsReadOnly();
            }

            return vectors.AsReadOnly();
        }

        return Enumerable.Empty<VectoreStoreDto>().ToList().AsReadOnly();
    }

    public async Task<IReadOnlyList<VectoreStoreDto>> SearchAsync(string collectionName, float[] queryVector, int topK = 1, CancellationToken cancellationToken = default)
    {
        var collection = await TryGetCollectionAsync(collectionName, cancellationToken);
        if (!collection.Any())
        {
            throw new Exception("Check if collection exists");
        }

        var results = collection
            .AsParallel()
            .Select(v => new { v, Similarity = CosineSimilarity(v.Vector, queryVector) })
            .OrderByDescending(x => x.Similarity)
            .Take(topK)
            .Select(x => x.v)
            .ToList();

        return results.AsReadOnly();
    }

    private static double CosineSimilarity(float[] vector1, float[] vector2)
    {
        if (vector1.Length != vector2.Length)
        {
            throw new Exception("Vectors must be the same length");
        }

        var dotProduct = vector1.Zip(vector2, (a, b) => a * b).Sum();
        var magnitude1 = Math.Sqrt(vector1.Sum(v => v * v));
        var magnitude2 = Math.Sqrt(vector2.Sum(v => v * v));

        return dotProduct / (magnitude1 * magnitude2);
    }
}

public sealed record VectoreStoreDto(float[] Vector, IDictionary<string, string> Metadata);