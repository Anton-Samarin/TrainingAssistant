using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using TrainingAssistant.Application.Options;
using TrainingAssistant.Domain.Entities;
using TrainingAssistant.Infrastructure.Helpers;

namespace TrainingAssistant.Infrastructure.Ml;

/// <summary>
/// HTTP-клиент к Python ML-сервису тренировок
/// </summary>
public class MlTrainingClient
{
    private readonly HttpClient _httpClient;
    private readonly MlServiceOptions _options;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public MlTrainingClient(HttpClient httpClient, IOptions<MlServiceOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    /// <summary>
    /// Запрашивает у ML-сервиса программу тренировок на неделю
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="profile">Профиль для подбора нагрузки</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Тип программы и дни с упражнениями</returns>
    public async Task<MlGenerateWeekResponse> GenerateWeekAsync(
        Guid userId,
        UserProfile profile,
        IReadOnlyList<UserExercisePreference> preferences,
        CancellationToken cancellationToken = default)
    {
        var equipment = JsonListHelper.Deserialize(profile.EquipmentJson);
        var injuries = JsonListHelper.Deserialize(profile.InjuriesJson);
        var request = MlGenerateWeekRequest.FromProfile(
            userId, profile, equipment, injuries, preferences.ToList());

        using var message = new HttpRequestMessage(HttpMethod.Post, "v1/training/generate-week")
        {
            Content = JsonContent.Create(request)
        };
        message.Headers.Add("X-Api-Key", _options.ApiKey);

        var response = await _httpClient.SendAsync(message, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"ML service error {(int)response.StatusCode}: {body}");
        }

        var result = await response.Content.ReadFromJsonAsync<MlGenerateWeekResponse>(JsonOptions, cancellationToken);
        return result ?? throw new InvalidOperationException("ML service returned empty response.");
    }
}
