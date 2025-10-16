using Microsoft.JSInterop;

public class AuthService
{
    private readonly IJSRuntime _jsRuntime;

    public AuthService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<bool> IsUserLoggedIn()
    {
        var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
        return !string.IsNullOrEmpty(token);
    }

    public async Task<string?> GetToken()
    {
        return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
    }

    public async Task Logout()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
    }
    public async Task<string?> GetUserName()
    {
        return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "userName");
    }
}
