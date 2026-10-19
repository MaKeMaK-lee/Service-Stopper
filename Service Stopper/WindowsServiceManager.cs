using System;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsServices;

/// <summary>
/// Класс для управления службами Windows.
/// </summary>
public sealed class WindowsServiceManager
{
    /// <summary>
    /// Останавливает службу Windows по её имени.
    /// </summary>
    /// <param name="serviceName">Имя службы (не отображаемое имя, а ServiceName).</param>
    /// <param name="timeout">Максимальное время ожидания остановки.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>true — служба успешно остановлена; false — служба уже была остановлена.</returns>
    /// <exception cref="ArgumentException">Имя службы пустое.</exception>
    /// <exception cref="InvalidOperationException">Служба не найдена или не может быть остановлена.</exception>
    /// <exception cref="TimeoutException">Превышено время ожидания остановки.</exception>
    public async Task<bool> StopServiceAsync(
        string serviceName,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
            throw new ArgumentException("Имя службы не может быть пустым.", nameof(serviceName));

        timeout ??= TimeSpan.FromSeconds(30);

        using var service = new ServiceController(serviceName);

        // Проверка существования службы — обращение к Status выбросит исключение,
        // если служба не найдена.
        ServiceControllerStatus status;
        try
        {
            status = service.Status;
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidOperationException(
                $"Служба \"{serviceName}\" не найдена.", ex);
        }

        // Если служба уже остановлена
        if (status == ServiceControllerStatus.Stopped)
            return false;

        // Если служба не может быть остановлена (например, уже останавливается)
        if (!service.CanStop)
            throw new InvalidOperationException(
                $"Служба \"{serviceName}\" не поддерживает остановку (CanStop = false).");

        service.Stop();

        // Ожидание остановки
        var startTime = DateTime.UtcNow;
        while (service.Status != ServiceControllerStatus.Stopped)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (DateTime.UtcNow - startTime > timeout.Value)
                throw new System.ServiceProcess.TimeoutException(
                    $"Превышено время ожидания остановки службы \"{serviceName}\" " +
                    $"(текущее состояние: {service.Status}).");

            await Task.Delay(500, cancellationToken).ConfigureAwait(false);
            service.Refresh();
        }

        return true;
    }

    /// <summary>
    /// Синхронная версия метода остановки службы.
    /// </summary>
    public bool StopService(string serviceName, TimeSpan? timeout = null)
        => StopServiceAsync(serviceName, timeout).GetAwaiter().GetResult();
}