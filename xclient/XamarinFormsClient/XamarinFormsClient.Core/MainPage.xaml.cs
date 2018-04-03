using IdentityModel.Client;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XamarinFormsClient.Core
{
	//[XamlCompilation(XamlCompilationOptions.Skip)]
	public partial class MainPage : ContentPage
    {
        OidcClient _client;
        LoginResult _result;
        IBrowser _browser;
        
        Lazy<HttpClient> _apiClient = new Lazy<HttpClient>(()=>new HttpClient());
        
        public MainPage ()
		{
			InitializeComponent ();

            Login.Clicked += Login_Clicked;
            Logout.Clicked += Logout_Clicked;
            CallApi.Clicked += CallApi_Clicked;

            _browser = DependencyService.Get<IBrowser>();

            var options = new OidcClientOptions
            {
                // 10.0.2.2 is an alias for localhost on the computer when using the android emulator
                Authority = "http://10.0.2.2:50405/", 
                ClientId = "native.hybrid",
                Scope = "openid profile email idserverapi offline_access",
                RedirectUri = "xamarinformsclients://callback",
                PostLogoutRedirectUri = "xamarinformsclients://callback",
                Browser = _browser,
                ResponseMode = OidcClientOptions.AuthorizeResponseMode.Redirect,
                Policy = new Policy()
                {
                    Discovery = new DiscoveryPolicy() { RequireHttps = false }
                      
                }   
            };

            _client = new OidcClient(options);
        }

        private async void Logout_Clicked(object sender, EventArgs e)
        {
            var logoutRequest =  new LogoutRequest();
            await _client.LogoutAsync(logoutRequest);  
        }

        private async void Login_Clicked(object sender, EventArgs e)
        {
            _result = await _client.LoginAsync(new LoginRequest());

            if (_result.IsError)
            {
                OutputText.Text = _result.Error;
                return;
            }

            var sb = new StringBuilder(128);
            foreach (var claim in _result.User.Claims)
            {
                sb.AppendFormat("{0}: {1}\n", claim.Type, claim.Value);
            }

            sb.AppendFormat("\n{0}: {1}\n", "refresh token", _result?.RefreshToken ?? "none");
            sb.AppendFormat("\n{0}: {1}\n", "access token", _result.AccessToken);

            OutputText.Text = sb.ToString();

            _apiClient.Value.SetBearerToken(_result?.AccessToken ?? "");
            // 10.0.2.2 is an alias for localhost on the computer when using the android emulator
            _apiClient.Value.BaseAddress = new Uri("http://10.0.2.2:50405/");

        }

        private async void CallApi_Clicked(object sender, EventArgs e)
        {
            
            var result = await _apiClient.Value.GetAsync("api/identity");

            if (result.IsSuccessStatusCode)
            {
                OutputText.Text = JArray.Parse(await result.Content.ReadAsStringAsync()).ToString();
            }
            else
            {
                OutputText.Text = result.ReasonPhrase;
            }
        }
    }
}