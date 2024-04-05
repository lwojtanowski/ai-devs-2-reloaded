namespace AiDevs2Reloaded.Api.Services.Abstractions;

public interface IVectoreStore
{
    Task StoreAsync(string collectionName, IEnumerable<VectoreStoreDto> vectors, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<VectoreStoreDto>> TryGetCollectionAsync(string collectionName, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<VectoreStoreDto>> SearchAsync(string collectionName, float[] queryVector, int topK = 1, CancellationToken cancellationToken = default);
}
