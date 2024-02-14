namespace Dim.Clients.Extensions;

public static class HttpAsyncResponseMessageExtension
{
    public static async ValueTask<HttpResponseMessage> CatchingIntoServiceExceptionFor(this Task<HttpResponseMessage> response, string systemName, RecoverOptions recoverOptions = RecoverOptions.None, Func<HttpResponseMessage, ValueTask<(bool Ignore, string? Message)>>? handleErrorResponse = null)
    {
        try
        {
            var message = await response.ConfigureAwait(false);
            var requestUri = message.RequestMessage?.RequestUri?.AbsoluteUri;
            if (message.IsSuccessStatusCode)
            {
                return message;
            }

            string? errorMessage;
            try
            {
                (var ignore, errorMessage) = (int)message.StatusCode switch
                {
                    < 500 when handleErrorResponse != null => await handleErrorResponse(message).ConfigureAwait(false),
                    >= 500 => (false, await message.Content.ReadAsStringAsync().ConfigureAwait(false)),
                    _ => (false, null)
                };
                if (ignore)
                    return message;
            }
            catch (Exception)
            {
                errorMessage = null;
            }

            throw new ServiceException(
                string.IsNullOrWhiteSpace(errorMessage)
                    ? $"call to external system {requestUri ?? systemName} failed with statuscode {(int)message.StatusCode}"
                    : $"call to external system {requestUri ?? systemName} failed with statuscode {(int)message.StatusCode} - Message: {errorMessage}",
                message.StatusCode,
                (recoverOptions & RecoverOptions.RESPONSE_RECEIVED) == RecoverOptions.RESPONSE_RECEIVED);
        }
        catch (HttpRequestException e)
        {
            throw e.StatusCode == null
                ? new ServiceException($"call to external system {systemName} failed", e, (recoverOptions & RecoverOptions.REQUEST_EXCEPTION) == RecoverOptions.REQUEST_EXCEPTION)
                : new ServiceException($"call to external system {systemName} failed with statuscode {(int)e.StatusCode.Value}", e, e.StatusCode.Value, (recoverOptions & RecoverOptions.REQUEST_EXCEPTION) == RecoverOptions.REQUEST_EXCEPTION);
        }
        catch (TaskCanceledException e)
        {
            throw new ServiceException($"call to external system {systemName} failed due to timeout", e, (recoverOptions & RecoverOptions.TIMEOUT) == RecoverOptions.TIMEOUT);
        }
        catch (Exception e) when (e is not ServiceException and not SystemException)
        {
            throw new ServiceException($"call to external system {systemName} failed", e, (recoverOptions & RecoverOptions.OTHER_EXCEPTION) == RecoverOptions.OTHER_EXCEPTION);
        }
    }

    [Flags]
    public enum RecoverOptions
    {
        None = 0b_0000_0000,
        RESPONSE_RECEIVED = 0b_0000_0001,
        REQUEST_EXCEPTION = 0b_0000_0010,
        TIMEOUT = 0b_0000_0100,
        OTHER_EXCEPTION = 0b_0000_1000,
        INFRASTRUCTURE = REQUEST_EXCEPTION | TIMEOUT,
        ALLWAYS = RESPONSE_RECEIVED | REQUEST_EXCEPTION | TIMEOUT | OTHER_EXCEPTION
    }
}
