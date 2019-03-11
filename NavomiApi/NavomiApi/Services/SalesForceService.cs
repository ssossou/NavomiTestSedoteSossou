using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SalesForceApi;
using NavomiApi.Models;
using NavomiApi.Interfaces;



namespace NavomiApi.Services
{
    public class SalesForceService : ISalesForceService
    {
       
        SalesForceConfiguration SConfig { get; }
        IConfiguration Configuration { get; }
        SalesForceApi.SoapClient Client;
        SalesForceApi.SoapClient LoginClient;
        private  SalesForceApi.SessionHeader sessionHeader;
        
        public SalesForceService(IConfiguration configuration, SalesForceConfiguration sconfig)
        {
            
            Configuration = configuration;
            SConfig = sconfig;
        }
       
        
      

        public async Task<bool> loginToSalesForce()
        {
            CallOptions callOptions = new CallOptions();
            LoginScopeHeader loginScopeHeader = new LoginScopeHeader() { organizationId = SConfig.OrganizationId, portalId = SConfig.PortalId};
            //callOptions.client = "";
            LoginClient = new SoapClient();
            //loginResponse res =  await  LoginClient.loginAsync(loginScopeHeader, callOptions, SConfig.UserName, SConfig.Password);
            try
            {

            
            loginResponse res = await LoginClient.loginAsync(null,null, SConfig.UserName, SConfig.Password);
            var sessionId = res.result.sessionId;
            if (!string.IsNullOrEmpty(sessionId))
            {
                Client.Endpoint.Address = new System.ServiceModel.EndpointAddress(res.result.serverUrl);
                sessionHeader = new SessionHeader() { sessionId = sessionId };
                return true;
            }
            else
            {

                return false;
            }
            }
            catch(Exception e )
            {
                var mes = e;
                return false;
            }


        }


        public async Task<IEnumerable<Record>> GetRecords(string objectType)
        {
            Client = new SoapClient();
            CallOptions callOptions = new CallOptions();
            LoginScopeHeader loginScopeHeader = new LoginScopeHeader() { organizationId = SConfig.OrganizationId, portalId = SConfig.PortalId };
            LocaleOptions lOptions = new LocaleOptions();
            //callOptions.client = "";
            var packageVersionsHeader = new List<PackageVersion>();

            List<Record> Records = new List<Record>();
            var lr = loginToSalesForce();
            if(lr.Result)
            {
                //var sObjectArray = Client.describeSObjectsAsync(sessionHeader, callOptions, packageVersionsHeader.ToArray(), lOptions, new string[] { objectType });
                var sObjectArray = Client.describeSObjectsAsync(sessionHeader, null, null,null, new string[] { objectType });
                foreach (var item in sObjectArray.Result.result)
                {
                    Record rec = new Record();
                    for( var i = 0; i<item.fields.Count(); i++)
                    {
                        rec.Name = item.fields[i].name;
                        Records.Add(rec);
                    }
                }
                
            }


            return Records;
        }

    }
}
