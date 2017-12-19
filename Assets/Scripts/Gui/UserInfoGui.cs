using UnityEngine;
using UnityEngine.UI;
using GameSparks.Api.Responses;

namespace Gui
{
    public class UserInfoGui : MonoBehaviour
    {
        public Text UserId;
        public Text ConnectionStatus;

        /**
         * <summary>Initialize the UserInfoGui</summary>
         **/
        public void Initialize()
        {
            UserId.text = "No User Logged In...";
            ConnectionStatus.text = "Connecting To GameSparks...";
        }
        
        /**
         * <summary>On GS Available</summary>
         * <param name="state">The Gs availabity state</param>
         **/
        public void OnGsAvailable(bool state)
        {
            ConnectionStatus.text = state ? "GameSparks Connected..." : "GameSparks Disconnected...";
        }

        /**
         * <summary>On End Session</summary>
         **/
        public void OnEndSession()
        {
            UserId.text = "No User Logged In...";
            ConnectionStatus.text = "GameSparks Connected...";
        }
        
        /**
         * <summary>On Registration, display reg details</summary>
         * <param name="r">RegistrationResponse received</param>
         **/
        public void OnRegistration(RegistrationResponse r)
        {
            UserId.text = r.DisplayName + " (" + r.UserId + ")";
            ConnectionStatus.text = "New User Registered...";
        }

        /**
         * <summary>On Authentication, display auth details</summary>
         * <param name="r">AuthenticationResponse received</param>
         **/
        public void OnAuthentication(AuthenticationResponse r)
        {
            UserId.text = r.DisplayName + " (" + r.UserId + ")";
            ConnectionStatus.text = "User Authenticated...";
        }
    }
}