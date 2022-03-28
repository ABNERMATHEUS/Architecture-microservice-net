// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using GeekShopping.IdentityServer.MainModule.Consent;

namespace GeekShopping.IdentityServer.MainModule.Device
{
    public class DeviceAuthorizationInputModel : ConsentInputModel
    {
        public string UserCode { get; set; }
    }
}