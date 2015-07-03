## Building ##

  1. Grab the latest from SVN (or tag)
  1. Open Solution in Visual Studio 2010
  1. Build the Solution
  1. Build the RazorInstaller project
  1. Copy the MSI that is created from building the RazorMediatorInstaller project.


## Deploying ##

Deploying your custom build or deploying the pre-built MSI in the downloads section follows the same steps.

  1. Copy the MSI to the Tridion CMS Server and run.
  1. Follow the steps until installation is complete.
  1. Restart the Tridion COM+ service.

## Redeploying ##

  1. Uninstall the Razor Mediator (Control Panel -> Programs -> Uninstall Program
  1. Copy the MSI to the Tridion CMS Server and run.
  1. Follow the steps until installation is complete.
  1. If you had any custom Razor Mediator configuration set, copy your configuration settings to the Razor Mediator section in Tridion.ContentManager.config.  Remember, Installing and Uninstalling creates a backup of the config file automatically.
  1. Restart the Tridion COM+ service, the Tridion Publishing Service, and the Tridion Service Host Service.