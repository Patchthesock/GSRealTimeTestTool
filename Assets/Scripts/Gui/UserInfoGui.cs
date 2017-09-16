using UnityEngine;
using UnityEngine.UI;
using GameSparks.Api.Responses;

namespace Gui
{
    public class UserInfoGui : MonoBehaviour
    {
        public Text UserId;
        public Text ConnectionStatus;

        public void Initialize()
        {
            UserId.text = "No User Logged In...";
            ConnectionStatus.text = "Connecting To GameSparks...";
        }
        
        public void OnGsAvailable(bool isAvailable)
        {
            ConnectionStatus.text = isAvailable ? "GameSparks Connected..." : "GameSparks Disconnected...";
        }

        public void OnEndSession()
        {
            UserId.text = "No User Logged In...";
            ConnectionStatus.text = "GameSparks Connected...";
        }
        
        public void OnRegistration(RegistrationResponse resp)
        {
            UserId.text = resp.DisplayName + " (" + resp.UserId + ")";
            ConnectionStatus.text = "New User Registered...";
        }

        public void OnAuthentication(AuthenticationResponse resp)
        {
            UserId.text = resp.DisplayName + " (" + resp.UserId + ")";
            ConnectionStatus.text = "User Authenticated...";
        }
    }
}