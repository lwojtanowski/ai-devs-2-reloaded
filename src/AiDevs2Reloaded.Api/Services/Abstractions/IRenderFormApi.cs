using AiDevs2Reloaded.Api.Contracts.RenderForm;
using Refit;

namespace AiDevs2Reloaded.Api.Services.Abstractions;

public interface IRenderFormApi
{
    [Post("/api/v2/render")]
    Task<RenderImageResponse> RenderAsync([Body(BodySerializationMethod.Serialized)] RenderImageRequest data, CancellationToken cancellationToken = default);
}
