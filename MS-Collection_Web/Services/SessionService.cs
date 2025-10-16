using Microsoft.JSInterop;

public class SessionService
{
    public event Action OnChange;

    private bool _isAuthenticated;
    public bool IsAuthenticated
    {
        get => _isAuthenticated;
        private set
        {
            if (_isAuthenticated != value)
            {
                _isAuthenticated = value;
                NotifyStateChanged();
            }
        }
    }

    private string _nombreUsuario;
    public string NombreUsuario
    {
        get => _nombreUsuario;
        private set
        {
            if (_nombreUsuario != value)
            {
                _nombreUsuario = value;
                NotifyStateChanged();
            }
        }
    }

    private readonly IJSRuntime _jsRuntime;

    public SessionService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
        _isAuthenticated = false;
        _nombreUsuario = string.Empty;
    }

    private void NotifyStateChanged() => OnChange?.Invoke();

    public async Task LoginAsync(string token, string nombreUsuario)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", token);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "nombreUsuario", nombreUsuario);

        IsAuthenticated = true;
        NombreUsuario = nombreUsuario;
    }

    public async Task LogoutAsync()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "nombreUsuario");

        IsAuthenticated = false;
        NombreUsuario = string.Empty;
    }


    public async Task CheckSessionAsync()
    {
        var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
        var nombre = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "nombreUsuario");

        if (!string.IsNullOrEmpty(token))
        {
            IsAuthenticated = true;
            NombreUsuario = nombre ?? string.Empty;
        }
        else
        {
            IsAuthenticated = false;
            NombreUsuario = string.Empty;
        }
    }


}
