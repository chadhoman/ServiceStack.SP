using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System.Collections.ObjectModel;
using System.Threading;

namespace ServiceStack.SP.Features.ServiceStack.SP.WebConfig
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("af27c4bc-753e-42b7-9307-3e50bad49e2f")]
    public class ServiceStackSPEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPWebApplication webApp = properties.Feature.Parent as SPWebApplication;
            RemoveAllCustomizations(webApp);

            SPWebConfigModification locationMod = new SPWebConfigModification();
            locationMod.Path = "configuration";
            locationMod.Name = "location[@path='_layouts/api']";
            locationMod.Sequence = 0;
            locationMod.Owner = "ServiceStack.SP";
            locationMod.Type = SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode;
            locationMod.Value = @"
            <location path='_layouts/api'>
    <system.web>
      <customErrors mode='Off'></customErrors>
      <httpHandlers>
        <add path='*' type='ServiceStack.HttpHandlerFactory, ServiceStack, Version=4.0.0.0, Culture=neutral, PublicKeyToken=e06fbc6124f57c43' verb='*' />
      </httpHandlers>
    </system.web>
    <system.webServer>
      <modules runAllManagedModulesForAllRequests='true' />
      <validation validateIntegratedModeConfiguration='false' />
      <handlers>
        <add path='*' name='ServiceStack.Factory' type='ServiceStack.HttpHandlerFactory, ServiceStack, Version=4.0.0.0, Culture=neutral, PublicKeyToken=e06fbc6124f57c43' verb='*' preCondition='integratedMode' resourceType='Unspecified' allowPathInfo='true' />
      </handlers>
    </system.webServer>
  </location>
  ";
            webApp.WebConfigModifications.Add(locationMod);

            //removed in favor of web activator
//            SPWebConfigModification moduleMod = new SPWebConfigModification();
//            moduleMod.Path = "configuration/system.webServer/modules";
//            moduleMod.Name = "add[@name='ServiceStackHandler']";
//            moduleMod.Sequence = 0;
//            locationMod.Owner = "ServiceStack.SP";
//            locationMod.Type = SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode;
//            moduleMod.Value = @"
//           <add name='ServiceStackHandler' type='ServiceStack.SP.ServiceStackHandler, ServiceStack.SP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=7beed51edcd64865' />";
//            webApp.WebConfigModifications.Add(moduleMod);

            webApp.Update();
            WaitForOneTimeJobToFinish(webApp.Farm);
            webApp.Farm.Services.GetValue<SPWebService>().ApplyWebConfigModifications();
        }


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            SPWebApplication webApp = properties.Feature.Parent as SPWebApplication;
            RemoveAllCustomizations(webApp);

            webApp.Update();
            WaitForOneTimeJobToFinish(webApp.Farm);
            webApp.Farm.Services.GetValue<SPWebService>().ApplyWebConfigModifications();
        }

        private void RemoveAllCustomizations(SPWebApplication webApp)
        {
            if (webApp != null)
            {
                Collection<SPWebConfigModification> collection = webApp.WebConfigModifications;
                int iStartCount = collection.Count;

                // Remove any modifications that were originally created by the owner.
                for (int c = iStartCount - 1; c >= 0; c--)
                {
                    SPWebConfigModification configMod = collection[c];

                    if (configMod.Owner == "ServiceStack.SP")
                    {
                        collection.Remove(configMod);
                    }
                }

                // Apply changes only if any items were removed.
                if (iStartCount > collection.Count)
                {
                    webApp.Update();
                    webApp.Farm.Services.GetValue<SPWebService>().ApplyWebConfigModifications();
                }
            }
        }

        private string jobTitle = "job-webconfig-modification";

        private bool IsJobDefined(SPFarm farm)
        {
            SPServiceCollection services = farm.Services;

            foreach (SPService service in services)
            {
                foreach (SPJobDefinition job in service.JobDefinitions)
                {
                    if (string.Compare(job.Name, jobTitle, StringComparison.OrdinalIgnoreCase) == 0)
                        return true;
                }
            }

            return false;
        }

        private bool IsJobRunning(SPFarm farm)
        {
            SPServiceCollection services = farm.Services;

            foreach (SPService service in services)
            {
                foreach (SPRunningJob job in service.RunningJobs)
                {
                    if (string.Compare(job.JobDefinition.Name, jobTitle, StringComparison.OrdinalIgnoreCase) == 0)
                        return true;
                }
            }

            return false;
        }

        private void WaitForOneTimeJobToFinish(SPFarm farm)
        {
            float waitTime = 0;

            do
            {
                if (!IsJobDefined(farm) && !IsJobRunning(farm))
                    break;

                const int sleepTime = 500; // milliseconds

                Thread.Sleep(sleepTime);
                waitTime += (sleepTime / 1000.0F); // seconds

            } while (waitTime < 20);
        }



        // Uncomment the method below to handle the event raised after a feature has been installed.

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is uninstalled.

        //public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        //{
        //}

        // Uncomment the method below to handle the event raised when a feature is upgrading.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}
    }
}
