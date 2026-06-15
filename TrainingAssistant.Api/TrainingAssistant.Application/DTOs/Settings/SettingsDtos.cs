namespace TrainingAssistant.Application.DTOs.Settings;

/// <summary>
/// Запрос на удаление аккаунта с подтверждением пароля
/// </summary>
public class DeleteAccountRequest
{
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Результат очистки недельных планов
/// </summary>
public class ClearWeeksResultDto
{
    public int DeletedCount { get; set; }
}
